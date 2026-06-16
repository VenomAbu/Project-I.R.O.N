using UnityEngine;

public class PassiveManager : MonoBehaviour
{
    [SerializeField] private MainTank tank;

    // Níveis das Passivas (Começam no 0)
    public int damageBoostLevel = 0;
    public int projectileSizeLevel = 0;
    public int lifeStealLevel = 0;
    public int regenLevel = 0;
    public int hpBoostLevel = 0;

    // Variáveis de controle interno
    private float regenTimer = 0f;
    private int currentHpBonus = 0;

    public enum PassiveType
    {
        DamageBoost,
        ProjectileSize,
        LifeSteal,
        Regeneration,
        HpBoost
    }

    private void Update()
    {
        // --- Passiva: Regeneração de HP ---
        // Só funciona se o nível for maior que 0
        if (regenLevel > 0)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= 1f) // A cada 1 segundo
            {
                // Cura 1% (0.01f) multiplicado pelo nível da passiva
                tank.HealPercentage(0.01f * regenLevel);
                regenTimer -= 1f;
            }
        }
    }

    // --- Passiva: Aumento de HP ---
    private void ApplyHpBoost()
    {
        // Descobre o qual é o HP "puro" do tanque
        int pureBaseHp = tank.hpMax - currentHpBonus;

        // Calcula o novo bônus de HP baseado no nível da passiva (10% por nível)
        float extraHpPercentage = 0.10f * hpBoostLevel;
        int newBonus = Mathf.RoundToInt(pureBaseHp * extraHpPercentage);

        // Calcula a diferença para saber o quanto o HP aumentou
        int hpDifference = newBonus - currentHpBonus;

        // Atualiza a memória do bônus e o HP máximo do tanque
        currentHpBonus = newBonus;
        tank.hpMax = pureBaseHp + currentHpBonus;

        // Cura o tanque pela diferença, para que o aumento de HP seja sentido imediatamente
        if (hpDifference > 0)
        {
            tank.Heal(hpDifference);
        }
    }

    // A Tela de Level Up vai chamar esta função!
    public void UpgradePassive(PassiveType type)
    {
        switch (type)
        {
            case PassiveType.DamageBoost:
                damageBoostLevel++;
                Debug.Log($"Dano subiu para Nível {damageBoostLevel}");
                break;

            case PassiveType.ProjectileSize:
                projectileSizeLevel++;
                Debug.Log($"Projétil subiu para Nível {projectileSizeLevel}");
                break;

            case PassiveType.LifeSteal:
                lifeStealLevel++;
                Debug.Log($"Roubo de Vida subiu para Nível {lifeStealLevel}");
                break;

            case PassiveType.Regeneration:
                regenLevel++;
                Debug.Log($"Regeneração subiu para Nível {regenLevel}");
                break;

            case PassiveType.HpBoost:
                hpBoostLevel++;
                ApplyHpBoost(); // Precisa aplicar na mesma hora
                Debug.Log($"HP Max subiu para Nível {hpBoostLevel}");
                break;
        }
    }
}