using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleUI : MonoBehaviour
{
    [Header("Textos")]
    [SerializeField] public TextMeshProUGUI lvlValue;
    [SerializeField] public TextMeshProUGUI ammoText;
    [SerializeField] public TextMeshProUGUI scrapText;
    [SerializeField] public TextMeshProUGUI countdownText;

    [Header("Barras de Progresso")]
    [SerializeField] public Image hpBarFill; 
    [SerializeField] public Image gasBarFill;
    [SerializeField] public Image xpBarFill; 

    [Header("Crescimento Dinâmico")]
    [SerializeField] public RectTransform hpBarBackground; 
    [SerializeField] public float widthPerHp = 2f; // Quantos pixels de largura vale cada 1 de HP

    public MainTank mainTank;
    public WaveController waveController;
  
    private void Update()
    {
        // Atualiza HP e Level
        lvlValue.text = mainTank.level.ToString("00");
        ammoText.text = mainTank.ammo.ToString("00");
        scrapText.text = mainTank.coins.ToString("00");


        // --- BARRA DE HP COM CRESCIMENTO FÍSICO ---
        if (mainTank.hpMax > 0)
        {
            // 1. Atualiza o preenchimento (A porcentagem)
            hpBarFill.fillAmount = (float)mainTank.currentHp / mainTank.hpMax;

            // 2. Atualiza o tamanho físico do Fundo na tela (Apenas a largura X muda)
            if (hpBarBackground != null)
            {
                hpBarBackground.sizeDelta = new Vector2(mainTank.hpMax * widthPerHp, hpBarBackground.sizeDelta.y);
            }
        }

        // --- BARRA DE GASOLINA (NOVO) ---
        if (mainTank.maxGas > 0)
        {
            gasBarFill.fillAmount = mainTank.gas / mainTank.maxGas;
        }

        // --- BARRA DE XP ---
        // Checamos se a instância existe para evitar erros caso o XpManager demore 1 frame para nascer
        if (XpManager.instance != null && XpManager.instance.nextLevelXP > 0)
        {
            xpBarFill.fillAmount = XpManager.instance.currentXP / XpManager.instance.nextLevelXP;
        }

        // Atualiza o Cronômetro
        if (waveController != null)
        {
            float tempoRestante = waveController.stageDuration - waveController.gameTimer;
            tempoRestante = Mathf.Max(0, tempoRestante);

            int minutos = Mathf.FloorToInt(tempoRestante / 60);
            int segundos = Mathf.FloorToInt(tempoRestante % 60);

            countdownText.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }
}