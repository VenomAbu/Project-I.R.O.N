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

        // Tenta dropar um Item (15% de chance)
        TryDropItem();

        // O inimigo desaparece
        Destroy(gameObject);
    }

    private void TryDropItem()
    {
        // Sorteia um número entre 0.0 e 100.0
        float roll = Random.Range(0f, 100f);

        // Se o número for menor ou igual a 15, o drop acontece!
        if (roll <= 15f)
        {
            // Pega todos os tipos possíveis de itens no script Item
            // Isso retorna um Array contendo [RepairKit, Gas, Bomb]
            System.Array itemTypes = System.Enum.GetValues(typeof(Item.ItemType));

            // Sorteia um índice aleatório dentro do tamanho desse Array
            int randomIndex = Random.Range(0, itemTypes.Length);

            // Converte o número sorteado de volta para o tipo do Enum
            Item.ItemType randomType = (Item.ItemType)itemTypes.GetValue(randomIndex);

            // Cria o objeto focado no mundo exatamente na posição onde o inimigo morreu
            ItemWorld.SpawnItemWorld(transform.position, new Item { itemType = randomType, amount = 1 });
        }
    }
}
