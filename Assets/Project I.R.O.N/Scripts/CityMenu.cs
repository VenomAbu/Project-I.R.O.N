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

        // Volta à cena 1 (Menu Principal)
        SceneManager.LoadScene(1);
    }
}