using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public event EventHandler OnItemListChanged;
    private List<Item> itemList;

    public Inventory()
    {
        itemList = new List<Item>();
    }

    
    public void AddItem(Item item)
    {
        // Checa se o item é "stackable"
        if (item.isStackable())
        {
            bool itemAlreadyInInventory = false;

            // Checa se já há um item igual no inventário e - se sim - soma o amount
            foreach (Item inventoryItem in itemList)
            {
                if (inventoryItem.itemType == item.itemType)
                {
                    inventoryItem.amount += item.amount;
                    itemAlreadyInInventory = true;
                }
            }
            // Caso não haja item igual no inventário, adiciona-o
            if (!itemAlreadyInInventory)
            {
                itemList.Add(item);
            }
        }
        // Se não for stackable, só o adiciona.
        else
        {
            itemList.Add(item);
        }
        // Dispara o evento avisando que a lista mudou
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    // Retorna a lista de itens
    public List<Item> GetItemList()
    {
        return itemList;
    }

    // Remove o item da lista em caso de uso e dispara o evento
    public void UseItem(Item item, MainTank tank)
    {
        item.UseItem(tank);

        item.amount--;

        if (item.amount <= 0)
        {
            itemList.Remove(item);
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }
}
