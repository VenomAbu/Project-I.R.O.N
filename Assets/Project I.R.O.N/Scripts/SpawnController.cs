using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public Transform[] spawnPoints;

    [Header("Prefabs dos Inimigos")]
    public GameObject redEnemyPrefab;   // Custo 1
    public GameObject blueEnemyPrefab;  // Custo 2
    public GameObject greenEnemyPrefab; // Custo 4

    // O WaveController vai chamar essa função
    public void ExecuteWave(int currentBudget)
    {
        int remainingBudget = currentBudget;
        int tentativasDeSeguranca = 0; // Evita loops infinitos caso não haja budget

        // Enquanto ainda houver budget, tenta comprar algo
        while (remainingBudget >= 1 && tentativasDeSeguranca < 100)
        {
            tentativasDeSeguranca++;

            // Sorteia um dos 3 inimigos
            int choice = Random.Range(0, 3);
            GameObject selectedPrefab = null;
            int cost = 0;

            // Define o custo do inimigo sorteado
            switch (choice)
            {
                case 0: // Vermelho
                    selectedPrefab = redEnemyPrefab;
                    cost = 1;
                    break;
                case 1: // Azul
                    selectedPrefab = blueEnemyPrefab;
                    cost = 2;
                    break;
                case 2: // Verde
                    selectedPrefab = greenEnemyPrefab;
                    cost = 4;
                    break;
            }

            // Checa se é possível comprar o inimigo sorteado e, se possível, o instanceia.
            if (remainingBudget >= cost && selectedPrefab != null)
            {
                Spawn(selectedPrefab);
                remainingBudget -= cost;

                // Se comprar, reseta a trava de segurança
                tentativasDeSeguranca = 0;
            }

            // Se o sorteado for muito caro, o loop roda de novo e sorteia outro até achar um que caiba ou o budget zerar.
        }
    }

    // Função interna que cuida apenas da posição e instanciação
    private void Spawn(GameObject prefab)
    {
        if (spawnPoints.Length == 0) return;

        // Escolhe o Spawn Point aleatóriamente e pega a posição dele
        int index = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPos = spawnPoints[index].position;
        // Reseta o Z para 0
        spawnPos.z = 0f;
        // Instancia o prefab do parâmetro
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}