using TMPro;
using UnityEngine;

public class SimpleUI : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI hpValue;
    [SerializeField] public TextMeshProUGUI lvlValue;
    [SerializeField] public TextMeshProUGUI countdownText;

    public MainTank mainTank;
    public WaveController waveController;

    private void Update()
    {
        // Atualiza HP e Level
        hpValue.text = mainTank.currentHp.ToString("00");
        lvlValue.text = mainTank.level.ToString("00");

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