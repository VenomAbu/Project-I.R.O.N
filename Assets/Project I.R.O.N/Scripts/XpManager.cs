using UnityEngine;

public class XpManager : MonoBehaviour
{
    public static XpManager instance;

    public float currentXP = 0;
    public float nextLevelXP = 100;

    [SerializeField] LevelUpManager levelUpManager;
    [SerializeField] MainTank player;


    private void Awake()
    {
        // Garante que só exista um XpManager e inicia o Singleton
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void GainXp(float xp)
    {
        Debug.Log("Recebi XP: " + xp);
        // Adiciona o XP
        currentXP += xp;

        // Sobe de nível caso o XP seja maior que o requerimento.
        if (currentXP >= nextLevelXP) LevelUp();
    }

    private void LevelUp()
    {
        currentXP -= nextLevelXP; // Sobra de XP vai para o próximo nível

        // Aumenta a dificuldade do próximo nível (ex: +20%)
        nextLevelXP = Mathf.RoundToInt(30 + 20 * player.level + Mathf.Pow(3 * player.level, 1.6f));

        // Melhora Atributos
        player.level++;
        player.hpMax += 5;      // Aumenta vida máxima em 5

        Debug.Log($"SUBIU DE NÍVEL! Agora o Tanque é Nível {player.level}");

        // Dispara o LevelUpPanel para o jogador escolher um bônus
        if (levelUpManager != null)
        {
            levelUpManager.ShowLevelUpOptions();
        }
        else
        {
            // Trava de segurança: se não estiver no inspector, tenta encontrar o LevelUpManager na cena.
            LevelUpManager manager = FindFirstObjectByType<LevelUpManager>();
            if (manager != null) manager.ShowLevelUpOptions();
        }
    }
}
