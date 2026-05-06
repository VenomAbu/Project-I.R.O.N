using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Base Stats")]
    public int level = 1;
    public int hpMax = 100;
    public int currentHp;

    protected virtual void Awake()
    {
        // Deixa a vida cheia ao iniciar o jogo
        currentHp = hpMax;
    }

    public virtual void TakeDamage(int amount)
    {
        // Diminui o HP atual
        currentHp -= amount;
        Debug.Log($"{gameObject.name} recebeu {amount} de dano. HP: {currentHp}");

        // Chama Die() para o character com 0 ou menos de HP
        if (currentHp <= 0)
        {
            Die();
        }
    }

    public abstract void Die();
}