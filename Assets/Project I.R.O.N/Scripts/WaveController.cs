using UnityEngine;

public class WaveController : MonoBehaviour
{
    public float gameTimer = 0f;
    public float stageDuration = 300f;
    public float pressure;
    public int budget;

    [SerializeField] private SpawnController spawner;

    public float waveDelay;

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        if (gameTimer < stageDuration)
        {
        // vai passando o tempo
        gameTimer += Time.deltaTime;

        // calcula o budget e a pressão atual baseado no tempo
        DifficultyCalculator();

        if (Time.time >= waveDelay)
        {
                // Manda o comando para o spawner com o Budget calculado e o tempo de jogo
                spawner.ExecuteWave(budget, gameTimer);

                // Define o próximo intervalo
                waveDelay = Time.time + GetCurrentDelay();
        }

        }
        else
        {
            // Avisa no console
            Debug.Log("VITÓRIA! Você sobreviveu aos 300 segundos.");

            // Chama a tela de vitória
            VictoryManager manager = FindFirstObjectByType<VictoryManager>();
            if (manager != null)
            {
                manager.ShowVictory();
            }

            // Desativa o spawner para parar de criar inimigos
            this.enabled = false;
        }

    }

    void DifficultyCalculator()
    {
        // calcula a pressão e o budget atual
        pressure = Mathf.FloorToInt(1f + (gameTimer / 60f));
        budget = Mathf.FloorToInt(4 * pressure);
    }

    float GetCurrentDelay()
    {
        // Calcula o delay entre waves baseado no tempo da partida, seguindo a tabela do game designer Diego.
        if (gameTimer <= 61f) return Random.Range(1.2f, 2.0f);
        if (gameTimer <= 181f) return Random.Range(0.9f, 1.5f);
        return Random.Range(0.6f, 1.2f);
    }
}
