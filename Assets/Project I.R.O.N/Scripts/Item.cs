using UnityEngine;

public class Item
{
    // Lista todos os tipos de item
    public enum ItemType
    {
        RepairKit,
        Gas,
        Bomb,
        Ammo,
        Coin,
    }

    public ItemType itemType;
    public int amount; // usado para armazenar a quantidade

    // Puxa o sprite correto dos Assets
    public Sprite GetSprite()
    {
        switch (itemType)
        {
            default:
            case ItemType.RepairKit: return ItemAssets.instance.repairKit;
            case ItemType.Gas: return ItemAssets.instance.gas;
            case ItemType.Bomb: return ItemAssets.instance.bomb;
            case ItemType.Ammo: return ItemAssets.instance.ammo;
            case ItemType.Coin: return ItemAssets.instance.coin;

        }
    }

    // Checa se o item é "stackable" (por enquanto todos são)
    public bool isStackable()
    {
        switch (itemType)
        {
            default:
            case ItemType.Gas:
            case ItemType.Bomb:
            case ItemType.RepairKit:
            case ItemType.Ammo:
                return true;
        }
    }

    // Usa o item com seu efeito desejado
    public void UseItem(MainTank tank)
    {
        switch (itemType)
        {
            case ItemType.Gas:
                Debug.Log("O tanque foi reabastecido!");
                tank.gas = Mathf.Clamp(tank.gas + 50f, 0f, tank.maxGas);
                break;

            case ItemType.RepairKit:
                Debug.Log("Consertando a blindagem do tanque!");
                tank.HealPercentage(0.20f);
                break;

            case ItemType.Ammo:
                Debug.Log("Recarregando!");
                tank.ammo += 20;
                break;

            case ItemType.Bomb:
                Debug.Log("Olha a BOMBA!");
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemyInScene in enemies)
                {
                    // Pega o componente de vida do inimigo
                    if (enemyInScene.TryGetComponent<Enemy>(out Enemy enemy))
                    {
                        // Mata o inimigo
                        enemy.Die();
                    }
                }
                // Instanciar um efeito visual de explosão aqui, depois
                break;
        }
    }
}
