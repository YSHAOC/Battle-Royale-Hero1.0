using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using Sirenix.OdinInspector;
using System;

[CreateAssetMenu(fileName = "Inventory_Config", menuName = "配置/Inventory配置")]
public class Inventory_Config : ConfigBase
{
    //容器物品配置列表
    public List<InventoryItem> Inventory_items = new List<InventoryItem>();


    //容器列表添加物品
    public void AddItem(Item_Config newItemConfig, int amount)
    {

        bool found = false;

        //可以堆叠的
        if (newItemConfig.stackable)
        {
            foreach (var item in Inventory_items)
            {
                if (item.item_Config == newItemConfig)
                {
                    item.amount += amount;
                    found = true;
                    break;
                }
            }
        }

        //不可堆叠 或 可堆叠找不到的
        for (int i = 0; i < Inventory_items.Count; i++)
        {
            if (Inventory_items[i].item_Config == null && !found)
            {
                Inventory_items[i].item_Config = newItemConfig;
                Inventory_items[i].amount = amount;
                break;
            }
        }

    }

    //得到容器物品列表想要的Item_Config 配置 (据据类型）
    public Item_Config GetState_Item_Config(ItemType itemType)
    {
        //不可堆叠 或 可堆叠找不到的
        for (int i = 0; i < Inventory_items.Count; i++)
        {

            if (Inventory_items[i].item_Config != null && Inventory_items[i].item_Config.itemType == itemType)
            {
                return Inventory_items[i].item_Config;
            }
        }
        return null;
    }

    //得到容器物品列表想要的InventoryItem 配置 （据据类型 名字）
    public InventoryItem GetState_Item_Config(ItemType itemType, string name)
    {
        //不可堆叠 或 可堆叠找不到的
        for (int i = 0; i < Inventory_items.Count; i++)
        {

            if (Inventory_items[i].item_Config != null && Inventory_items[i].item_Config.itemType == itemType && Inventory_items[i].item_Config.itemName == name)
            {
                return Inventory_items[i];
            }
        }
        return null;
    }


}
//单个各个物品配置
[Serializable]
public class InventoryItem
{
    public Item_Config item_Config; //物品配置

    public int amount; //物品数量
}