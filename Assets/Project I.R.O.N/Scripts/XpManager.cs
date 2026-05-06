using UnityEngine;

public class XpManager : MonoBehaviour
{
    public static XpManager instance;

    public float currentXP = 0;
    public float nextLevelXP = 100;

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
        nextLevelXP = Mathf.RoundToInt(nextLevelXP * 1.2f);

        // Melhora Atributos
        player.level++;
        player.hpMax += 20;      // Aumenta vida máxima
        player.currentHp = player.hpMax; // Cura o tanque ao subir de nível

        Debug.Log($"SUBIU DE NÍVEL! Agora o Tanque é Nível {player.level}");
    }
}
