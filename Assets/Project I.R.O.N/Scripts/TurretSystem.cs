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
    [SerializeField] private int turretDamage = 1;

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
        Aim();

        // Botão esquerdo do mouse dispara o tiro
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
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
        // Instancia a bala na cena e guarda os dados aqui no código para alteração.
        GameObject bulletGo = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (bulletGo.TryGetComponent<Projectile>(out Projectile proj))
        {
            // Calcula o dano total do projétil (fórmula temporária: dano = nível x dano do canhão x 10)
            int damageCalculated = 10 * turretDamage * mainTank.level;

            // Configura a bala com o dano atualizado
            proj.Setup(damageCalculated);
        }
    }

    // --- NOVA HABILIDADE ---
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
        int damageCalculated = 5 * turretDamage * mainTank.level;

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
