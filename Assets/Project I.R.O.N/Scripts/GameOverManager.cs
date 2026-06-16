using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;

    private void Start()
    {
        // Garante que o painel comece invisível e o tempo rodando normal
        gameOverPanel.SetActive(false);
    }

    // O Tanque vai chamar esta função quando o HP zerar
    public void ShowGameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    // A rotina secreta que faz a contagem
    private IEnumerator GameOverRoutine()
    {
        // Espera exatos 2 segundos no tempo normal do jogo
        yield return new WaitForSeconds(2f);

        // Após os 2 segundos, exibe a tela e congela tudo
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // O Botão de "Tentar Novamente" vai chamar esta função
    public void RestartGame()
    {
        // Retorna o tempo ao normal ANTES de recarregar a cena, senão o jogo nasce pausado!
        Time.timeScale = 1f;

        // Recarrega a cena atual (independente de qual seja o nome dela)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}