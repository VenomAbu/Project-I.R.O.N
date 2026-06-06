using System.Collections.Generic; // Precisamos disso para usar as Lists
using UnityEngine;

// Nossa estrutura modular para configurar inimigos no Inspector
[System.Serializable]
public struct EnemySpawnConfig
{
    public GameObject enemyPrefab;
    public float unlockTimeSeconds; // Ex: 0 para o começo, 120 para 2 min, 240 para 4 min
}

public class SpawnController : MonoBehaviour
{
    public Transform[] spawnPoints;

    [Header("Configuração do Catálogo de Inimigos")]
    public EnemySpawnConfig[] enemyConfigs; // Substitui as variáveis antigas

    // O WaveController agora envia o Budget E o Tempo Atual
    public void ExecuteWave(int currentBudget, float currentTime)
    {
        int remainingBudget = currentBudget;

        // Enquanto ainda houver budget, tenta comprar algo
        while (remainingBudget >= 1)
        {
            // 1. Cria um "carrinho de compras" apenas com quem pode ser invocado AGORA
            List<EnemySpawnConfig> availableEnemies = new List<EnemySpawnConfig>();

            foreach (EnemySpawnConfig config in enemyConfigs)
            {
                // Lê o custo acessando diretamente a sua classe Enemy!
                // (Nota da Rika: Se a sua variável não se chamar 'cost', mude aqui embaixo)
                int cost = config.enemyPrefab.GetComponent<Enemy>().budgetCost;

                // Se o jogo já passou do tempo de desbloqueio E temos budget para comprar...
                if (currentTime >= config.unlockTimeSeconds && cost <= remainingBudget)
                {
                    availableEnemies.Add(config); // Adiciona na lista de possíveis sorteios
                }
            }

            // 2. Trava de Segurança Definitiva:
            // Se a lista estiver vazia (o budget acabou ou sobrou 1 mas só tem monstro de custo 2 liberado)
            if (availableEnemies.Count == 0)
            {
                break; // Encerra o loop instantaneamente sem travar o Unity!
            }

            // 3. Sorteia UM inimigo válido
            int randomIndex = Random.Range(0, availableEnemies.Count);
            EnemySpawnConfig selectedConfig = availableEnemies[randomIndex];

            // Pega o custo dele para cobrar
            int selectedCost = selectedConfig.enemyPrefab.GetComponent<Enemy>().budgetCost;

            // 4. Instancia e debita o valor
            Spawn(selectedConfig.enemyPrefab);
            remainingBudget -= selectedCost;
        }
    }

    private void Spawn(GameObject prefab)
    {
        if (spawnPoints.Length == 0) return;

        int index = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPos = spawnPoints[index].position;
        spawnPos.z = 0f;
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}