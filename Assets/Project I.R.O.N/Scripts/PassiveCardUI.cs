using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PassiveCardUI : MonoBehaviour
{
    [Header("Referências Visuais")]
    public Image iconImage;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI descriptionText;
    public Button cardButton;

    // Memória interna para saber qual passiva esta carta representa agora
    [HideInInspector] public PassiveManager.PassiveType currentPassive;
}