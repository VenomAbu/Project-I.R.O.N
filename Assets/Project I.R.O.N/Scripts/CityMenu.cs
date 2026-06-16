using UnityEngine;
using UnityEngine.SceneManagement;

public class CityMenu : MonoBehaviour
{
    // Função para o botão Iniciar Missão
    public void StartMission()
    {
        Debug.Log("Iniciando a primeira missão.");

        // Carrega a cena número 2
        SceneManager.LoadScene(2);
    }

    // Função para o botão SAIR
    public void QuitCityMenu()
    {
        Debug.Log("Voltando ao menu principal...");

        // Volta à cena 0 (Menu Principal)
        SceneManager.LoadScene(0);
    }

    // Função para o botão Loja
    public void OpenShop()
    {
        Debug.Log("Abrindo a loja...");

        // Carrega a cena da loja (ex: cena 3)
        SceneManager.LoadScene(3);
    }
}