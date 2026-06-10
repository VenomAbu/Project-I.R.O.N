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

    public virtual void Heal(int amount)
    {
        // Soma a cura ao HP atual
        currentHp += amount;

        // Impede que o HP passe do limite máximo do tanque
        if (currentHp > hpMax)
        {
            currentHp = hpMax;
        }

        Debug.Log($"{gameObject.name} foi curado em {amount} pontos. HP Atual: {currentHp}");
    }

    public virtual void HealPercentage(float percentage)
    {
        // Calcula o valor baseado no HP Máximo
        int calculatedAmount = Mathf.RoundToInt(hpMax * percentage);

        // Usa a função Heal para curar
        Heal(calculatedAmount);
    }

    public abstract void Die();
}