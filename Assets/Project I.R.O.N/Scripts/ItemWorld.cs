using UnityEngine;

public class ItemWorld : MonoBehaviour
{

    public static ItemWorld SpawnItemWorld(Vector3 position, Item item)
    {
        // Instanceia o prefab template com a posição indicada
        Transform transform = Instantiate(ItemAssets.instance.pfItemWorld, position, Quaternion.identity);
        // Pega o script do prefab
        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        // Chama SetItem e diz qual será o item
        itemWorld.SetItem(item);

        // Retorna o item pronto
        return itemWorld;
    }


    private Item item;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Diz qual é o item e corrige seu sprite
    public void SetItem(Item item)
    {
        this.item = item;
        spriteRenderer.sprite = item.GetSprite();
    }

    // Retorna o item para coleta
    public Item GetItem()
    {
        return item;
    }

    // Destrói o objeto
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
