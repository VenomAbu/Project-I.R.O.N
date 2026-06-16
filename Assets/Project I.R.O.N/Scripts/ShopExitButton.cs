using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopExitButton : MonoBehaviour
{
    // Função para o botão SAIR
    public void QuitShop()
    {
        Debug.Log("Voltando à cidade...");

        // Volta à cena 1 (Cidade)
        SceneManager.LoadScene(1);
    }
}
