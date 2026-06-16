using UnityEngine;
using System.Collections;

public class TurretSystem : MonoBehaviour
{
    [SerializeField] private float rotationOffset;
    public float rotationSpeed;

    [Header("Combat Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private MainTank mainTank;
    [SerializeField] private PassiveManager passiveManager;
    [SerializeField] private int turretDamage = 1;
    public float shootCooldown = 0.5f;
    private float nextFireTime = 0f;

    [Header("Ricochete Skill")]
    [SerializeField] private GameObject ricochetePrefab;
    [SerializeField] private float ricochetInterval = 5f;
    [SerializeField] private int ricochetQuantity = 4;

    public void Start()
    {
        // Inicia a contagem automática dos ricochetes assim que o jogo começa
        StartCoroutine(RicochetRoutine());
    }

    public void Update()
    {
        if (Time.timeScale == 0f) return;
        
        Aim();

        // Botão esquerdo do mouse dispara o tiro se o cooldown já tiver passado
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Shoot();
            // reajusta o cooldown para o próximo tiro
            nextFireTime = Time.time + shootCooldown;
        }
    }

    public void Aim()
    {
        // Pega a posição do mouse e acha essa direção
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (Vector2)mousePosition - (Vector2)transform.position;

        // Regula a movimentação da torreta de forma suave até a direção apontada
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;
        float currentAngle = transform.eulerAngles.z;
        float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // --- Tiro Padrão ---
    public void Shoot()
    {
        // Checa se há munição e - se não houver - retorna
        if (mainTank.ammo <= 0)
        {
            Debug.Log("Sem munição!");
            return; // O 'return' faz a função parar de rodar
        }

        // Instancia a bala na cena e guarda os dados aqui no código para alteração.
        GameObject bulletGo = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (bulletGo.TryGetComponent<Projectile>(out Projectile proj))
        {
            // Calcula o dano total do projétil
            int damageCalculated = 10 * turretDamage + mainTank.level * 8;

            // PASSIVA: AUMENTO DE DANO (10% por nível)
            float damageMultiplier = 1f + (0.10f * passiveManager.damageBoostLevel);
            damageCalculated = Mathf.RoundToInt(damageCalculated * damageMultiplier);

            // PASSIVA: AUMENTO DE TAMANHO (10% por nível)
            float sizeMultiplier = 1f + (0.10f * passiveManager.projectileSizeLevel);
            bulletGo.transform.localScale = bulletGo.transform.localScale * sizeMultiplier;

            // Configura a bala passando o Dano, o Tanque (para curar) e o nível do Life Steal
            proj.Setup(damageCalculated, mainTank, passiveManager.lifeStealLevel);

            // Diminui a munição em 1
            mainTank.ammo--;
        }
    }

    // --- Ricochete ---
    // IEnumerador serve para criar códigos que podem ser usados em intervalos de tempo.
    IEnumerator RicochetRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(ricochetInterval);
            LaunchRicochets();
        }
    }

    void LaunchRicochets()
    {
        // Calcula o dano
        int damageCalculated = 5 * turretDamage + mainTank.level * 8;
        // 5*turretDamage+ 2*ricochetDamage + maintank.level*8

        // Instanceia 4 projéteis
        for (int i = 0; i < ricochetQuantity; i++)
        {
            GameObject ric = Instantiate(ricochetePrefab, transform.position, Quaternion.identity);

            // Passa o dano para o script do ricochete
            if (ric.TryGetComponent<BounceProjectile>(out BounceProjectile rbScript))
            {
                rbScript.Setup(damageCalculated);
            }
        }
    }
}
