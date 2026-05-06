using UnityEngine;

public class Enemy : Character
{
    private Transform mainTankTransform;
    public int xpReward = 1;
    public int contactDamage = 1;
    public float damageInterval = 1f;
    public float moveSpeed;
    private float nextDamageTime;

    private void Start()
    {
        // Tenta encontrar o objeto que tem o script MainTank
        MainTank playerScript = Object.FindFirstObjectByType<MainTank>();

        if (playerScript != null)
        {
            mainTankTransform = playerScript.transform;
        }
    }

    private void Update()
    {
        // Move o monstro se achar o tanque no mapa
        if (mainTankTransform != null) MoveMonster();
    }

    public void MoveMonster()
    {
        // Acha a direção para onde está o tanque
        Vector2 direction = (mainTankTransform.position - transform.position).normalized;

        // Move na direção do tanque
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Checa se há cooldown entre os danos
        if (Time.time >= nextDamageTime)
        {
            Character character = other.GetComponentInParent<Character>();

            // Se for o player, chamar TakeDamage e dar dano
            if (character != null && other.CompareTag("Player"))
            {
                character.TakeDamage(contactDamage);

                // Inicia o cooldown
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }

    public override void Die()
    {
        Debug.Log("Inimigo derrotado!");

        // Passa o XP para o Manager
        if (XpManager.instance != null)
        {
            XpManager.instance.GainXp(xpReward);
        }

        // O inimigo desaparece
        Destroy(gameObject);
    }
}
