using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using UnityEngine.EventSystems;
using System;

//格子类型
public enum SlotType
{
    //背包  武器   防御  背包装备  可使用 
    BAG, WEAPON, ARMOR, ACTION,
    BAG_EQUIP, HELMET,
}


[Pool]
public class SlotHolder : MonoBehaviour
{
    //格子类型
    public SlotType slotType;

    //物品UI
    public ItemUI itemUI;

    private void Start()
    {
        this.OnMouseEnter(MouseEnter);
        this.OnMouseExit(MouseExit);
    }
    private void Update()
    {
        if (LVManager.Instance.isShow_window == false)
        {
            Inventory_Manager.Instance.tooltip.gameObject.SetActive(false);
        }

    }

    //当鼠标进入
    private void MouseEnter(PointerEventData arg1, object[] arg2)
    {
        if (itemUI.GetItemUI_ItemConfig())
        {
            Inventory_Manager.Instance.tooltip.gameObject.SetActive(true);
            Inventory_Manager.Instance.tooltip.SetupTooltip(itemUI.GetItemUI_ItemConfig());
        }

    }

    //当鼠标离开
    private void MouseExit(PointerEventData arg1, object[] arg2)
    {
        if (itemUI.GetItemUI_ItemConfig())
        {
            Inventory_Manager.Instance.tooltip.gameObject.SetActive(false);
        }
    }


    //更新格子
    public void UpdateItem()
    {
        switch (slotType)//格子类型
        {


            case SlotType.BAG: //背包
                itemUI.Bag = Inventory_Manager.Instance.bag_Config;
                break;


            case SlotType.WEAPON: //武器
                itemUI.Bag = Inventory_Manager.Instance.equipment_Config;
                //装备-武器 切换武器
                if (itemUI.GetItemUI_ItemConfig() != null)
                {
                    Player_Controller.Instance.ChangeWeapon(itemUI.GetItemUI_ItemConfig());
                    Player_Controller.Instance.canShoot = true;
                }
                else
                {
                    Player_Controller.Instance.UnEquipWeapon();
                    Player_Controller.Instance.canShoot = false;
                }
                break;


            case SlotType.HELMET: //头盔
                itemUI.Bag = Inventory_Manager.Instance.equipment_Config;

                //装备-头盔 切换头盔
                if (itemUI.GetItemUI_ItemConfig() != null)
                {
                    Player_Controller.Instance.ChangeHelmet(itemUI.GetItemUI_ItemConfig());
                }
                else
                {
                    Player_Controller.Instance.UnEquipHelmet();
                }
                break;


            case SlotType.ARMOR: //防御甲
                itemUI.Bag = Inventory_Manager.Instance.equipment_Config;

                //装备-防御甲 切换防御甲
                if (itemUI.GetItemUI_ItemConfig() != null)
                {
                    Player_Controller.Instance.ChangeArmor(itemUI.GetItemUI_ItemConfig());
                }
                else
                {
                    Player_Controller.Instance.UnEquipArmor();
                }
                break;


            case SlotType.BAG_EQUIP: //背包装备
                itemUI.Bag = Inventory_Manager.Instance.equipment_Config;
                //装备-背包装备 切换背包
                if (itemUI.GetItemUI_ItemConfig() != null)
                {
                    Player_Controller.Instance.ChangeBagEquip(itemUI.GetItemUI_ItemConfig());
                }
                else
                {
                    Player_Controller.Instance.UnEquipBagEquip();
                }
                break;


            case SlotType.ACTION: //可使用
                itemUI.Bag = Inventory_Manager.Instance.actionc_Config;
                break;
        }

        //SlotHolder中的itemUI更新
        //设置（ 每一个格子） 所对应的 （背包配置的InventoryItem配置） 一 一对应
        var InventoryItem = itemUI.GetItemUI_Inventoryitem();
        itemUI.SetupItemUI(InventoryItem.item_Config, InventoryItem.amount);
    }

}
