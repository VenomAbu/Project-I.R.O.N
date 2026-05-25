using UnityEngine;

public class Item
{
    // Lista todos os tipos de item
    public enum ItemType
    {
        RepairKit,
        Gas,
        Bomb,
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
                // Ex: tank.currentGas += 50;
                break;

            case ItemType.RepairKit:
                Debug.Log("Consertando a blindagem do tanque!");
                tank.Heal(100); // Cura o tanque em 100
                break;

            case ItemType.Bomb:
                Debug.Log("Olha a BOMBA!");
                // Lógica de explosão
                break;
        }
    }
}
