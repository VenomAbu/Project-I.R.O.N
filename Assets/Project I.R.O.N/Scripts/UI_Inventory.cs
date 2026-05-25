using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Inventory : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;
    private MainTank mainTank;

    private void Awake()
    {
        // Guarda os objetos na memória
        itemSlotContainer = transform.Find("ItemSlotContainer");
        itemSlotTemplate = transform.Find("ItemSlotTemplate");
    }

    public void SetInventory(Inventory inventory, MainTank tank)
    {
        // Configura o inventário e se inscreve no event
        this.inventory = inventory;
        this.mainTank = tank;
        inventory.OnItemListChanged += Inventory_OnItemListChanged;
        RefreshInventoryItems();
    }

    // Executa a função assim que o event disparar
    private void Inventory_OnItemListChanged(object sender, System.EventArgs e)
    {
        RefreshInventoryItems();
    }

    // Redesenha a tela de itens
    private void RefreshInventoryItems()
    {
        // Destrói todos os itens (menos o template) para redesenhar tudo
        foreach(Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        // Criando as variáveis de posição e tamanho
        int x = 0;
        int y = 0;
        float itemSlotCellSize = 110f;

        foreach (Item item in inventory.GetItemList())
        {
            // Clona o template
            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            // Ativa o objeto na tela
            itemSlotRectTransform.gameObject.SetActive(true);
            // Calcula a posição do objeto multiplicando o valor de X e Y pelo CellSize
            itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);

            // Acerta o sprite do item
            Image image = itemSlotRectTransform.Find("Image").GetComponent<Image>();
            image.sprite = item.GetSprite();

            // Cria os botões que permitem que o item seja usado em caso de clique
            Button button = itemSlotRectTransform.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                inventory.UseItem(item, mainTank);
            });

            // Salva o valor do texto de quantidade em uma variável
            TextMeshProUGUI uiText = itemSlotRectTransform.Find("amountText").GetComponent<TextMeshProUGUI>();

            // Checa se o texto é maior que um, se sim - o exibe.
            if (item.amount > 1)
            {
                uiText.SetText(item.amount.ToString());
            }
            else
            {
                uiText.SetText(" ");
            }

            // Adiciona +1 a posição para que o próximo item seja gerado ao lado, quando sua posição for multiplicada pelo CellSize
                x++;

            // Checa se há 4 itens na linha, se sim - cria uma nova abaixo.
            if (x > 4)
            {
                x = 0;
                y++;
            }
        }
    }
}
