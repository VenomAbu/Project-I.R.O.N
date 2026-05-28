using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Função para o botão JOGAR
    public void PlayGame()
    {
        Debug.Log("Iniciando o Projeto I.R.O.N!");

        // Carrega a cena número 1
        SceneManager.LoadScene(1);
    }

    // Função para o botão SAIR
    public void QuitGame()
    {
        Debug.Log("Fechando o jogo...");

        // Fecha o aplicativo (isso só funciona de verdade quando o jogo estiver exportado/buildado)
        Application.Quit();
    }
}