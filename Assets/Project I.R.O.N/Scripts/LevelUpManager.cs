using System.Collections.Generic;
using UnityEngine;
// 1. --- NOVO: As categorias que o designer pediu ---
public enum SkillCategory
{
    Passiva,
    Arma
}

[System.Serializable]
public class PassiveUIData
{
    public PassiveManager.PassiveType passiveType;
    public SkillCategory category;
    public string passiveName;
    [TextArea] public string description;
    public Sprite icon;
}

public class LevelUpManager : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private PassiveManager passiveManager;
    [SerializeField] private PassiveCardUI[] cards; // Arraste as 3 cartas da UI para cá

    [Header("Banco de Dados das Passivas")]
    [SerializeField] private List<PassiveUIData> passiveDatabase;

    private void Start()
    {
        levelUpPanel.SetActive(false);
    }

    // O Tanque (ou o sistema de XP) vai chamar esta função!
    public void ShowLevelUpOptions()
    {
        // 1. Pausa o jogo e mostra a tela
        Time.timeScale = 0f;
        levelUpPanel.SetActive(true);

        // 2. Cria uma lista temporária com todas as passivas disponíveis para sortear
        List<PassiveUIData> availablePassives = new List<PassiveUIData>(passiveDatabase);

        // 3. Sorteia e configura cada uma das 3 cartas
        foreach (PassiveCardUI card in cards)
        {
            // Sorteia um índice aleatório
            int randomIndex = Random.Range(0, availablePassives.Count);
            PassiveUIData drawnPassive = availablePassives[randomIndex];

            // Configura o visual da carta
            card.currentPassive = drawnPassive.passiveType;
            card.typeText.text = drawnPassive.category.ToString();
            card.nameText.text = drawnPassive.passiveName;
            card.descriptionText.text = drawnPassive.description;
            card.iconImage.sprite = drawnPassive.icon;

            // Busca o nível atual lá no PassiveManager para mostrar "Nível X"
            int currentLevel = GetPassiveLevel(drawnPassive.passiveType);
            card.levelText.text = $"Nível {currentLevel + 1}"; // Mostra o próximo nível

            // Remove a passiva sorteada da lista temporária para não repetir nas próximas cartas!
            availablePassives.RemoveAt(randomIndex);

            // Limpa funções antigas do botão e adiciona a função de clique nova
            card.cardButton.onClick.RemoveAllListeners();
            card.cardButton.onClick.AddListener(() => OnCardClicked(card.currentPassive));
        }
    }

    // Função ativada quando o jogador clica em qualquer uma das 3 cartas
    private void OnCardClicked(PassiveManager.PassiveType chosenType)
    {
        // 1. Envia a escolha para o PassiveManager aplicar a matemática
        passiveManager.UpgradePassive(chosenType);

        // 2. Esconde a tela e retoma o jogo
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // Função auxiliar só para pegar o número do nível do manager
    private int GetPassiveLevel(PassiveManager.PassiveType type)
    {
        switch (type)
        {
            case PassiveManager.PassiveType.DamageBoost: return passiveManager.damageBoostLevel;
            case PassiveManager.PassiveType.ProjectileSize: return passiveManager.projectileSizeLevel;
            case PassiveManager.PassiveType.LifeSteal: return passiveManager.lifeStealLevel;
            case PassiveManager.PassiveType.Regeneration: return passiveManager.regenLevel;
            case PassiveManager.PassiveType.HpBoost: return passiveManager.hpBoostLevel;
            default: return 0;
        }
    }
}