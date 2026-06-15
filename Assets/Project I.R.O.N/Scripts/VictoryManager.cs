using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;

    private void Start()
    {
        // Garante que o painel de vitória comece escondido
        victoryPanel.SetActive(false);
    }

    // O WaveController vai chamar esta função quando o tempo acabar
    public void ShowVictory()
    {
        victoryPanel.SetActive(true);

        // Pausa o jogo para o jogador respirar e comemorar
        Time.timeScale = 0f;
    }

    // O botão "Voltar para a Cidade" vai chamar esta função
    public void ReturnToCity(){
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
}