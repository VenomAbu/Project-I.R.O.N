using UnityEditor.PackageManager;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public int damage;
    public Rigidbody2D rb;

    private MainTank sourceTank;
    private int lifeStealLvl;

    // A Turreta vai chamar isso e passar o dano calculado
    public void Setup(int damageValue, MainTank tank, int lsLevel)
    {
        damage = damageValue;
        sourceTank = tank;
        lifeStealLvl = lsLevel;
    }

    private void Start()
    {
        // Adiciona movimento ao projétil
        rb.linearVelocity = speed * transform.up;

        // Destrói a bala após 5 segundos para não pesar o jogo
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character character = collision.GetComponent<Character>();

        // Checa se a colisão foi com um character.
        if (character != null)
        {
            // Chama a função TakeDamage de Character.
            character.TakeDamage(damage);

            // --- PASSIVA: LIFE STEAL ---
            // Se a passiva estiver pelo menos no nível 1
            if (sourceTank != null && lifeStealLvl > 0)
            {
                // Calcula 10% do dano causado por nível
                int healAmount = Mathf.RoundToInt(damage * 0.10f * lifeStealLvl);

                // Garante que cure pelo menos 1 de HP se o dano for muito baixo
                if (healAmount < 1) healAmount = 1;

                sourceTank.Heal(healAmount);
            }

            Destroy(gameObject); // Destrói ao bater
        }
    }
}