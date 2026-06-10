using UnityEngine;

// Script com os sprites e o prefab para a criação dos itens.
public class ItemAssets : MonoBehaviour
{
    public static ItemAssets instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public Transform pfItemWorld;
    
    public Sprite gas;
    public Sprite repairKit;
    public Sprite ammo;
    public Sprite bomb;
}
