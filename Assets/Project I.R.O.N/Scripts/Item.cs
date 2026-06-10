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
                tank.gas += 50;
                break;

            case ItemType.RepairKit:
                Debug.Log("Consertando a blindagem do tanque!");
                tank.HealPercentage(20);
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
                    // Pega o componente de vida do inimigo (vou usar o Enemy como exemplo)
                    if (enemyInScene.TryGetComponent<Enemy>(out Enemy enemy))
                    {
                        // Ao invés de Destroy, causamos um dano absurdo!
                        enemy.Die();
                    }
                }
                // Instanciar um efeito visual de explosão aqui, depois
                break;
        }
    }
}
