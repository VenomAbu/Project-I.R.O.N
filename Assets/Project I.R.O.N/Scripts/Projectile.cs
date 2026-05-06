using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public int damage;
    public Rigidbody2D rb;

    // A Turreta vai chamar isso e passar o dano calculado
    public void Setup(int damageValue)
    {
        damage = damageValue;
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

            Destroy(gameObject); // Destrói ao bater
        }
    }
}