using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JKFrame;


public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image icon = null; //物品图片

    [SerializeField] private Text amount = null; //物品数量

    public Inventory_Config Bag { get; set; } //背包配置文件
    public int Index { get; set; } = -1; //每个物品的在背包中索引ID



    #region 鼠标双击使用物品

    private void Start()
    {
        this.OnClick(OnPointerClick); //添加双击事件
    }
    private void OnPointerClick(PointerEventData eventData, object[] arg2) //鼠标双击事件
    {
        if (eventData.clickCount % 2 == 0) //取余 是否双击
        {
            Debug.Log("双击-使用物品");
            UseItem();//使用物品
        }
    }
    //鼠标双击-使用物品
    public void UseItem()
    {
        //物品类型是可使用的 且 数量要大于0
        if (this.GetItemUI_ItemConfig().itemType == ItemType.Useable && this.GetItemUI_Inventoryitem().amount > 0)
        {
            UseItem_Type useItem_Type = this.GetItemUI_ItemConfig().useable_Config.useItem_Type; //可使用的类型
            switch (useItem_Type) //判断为什么类型
            {
                case UseItem_Type.ApplyHP:  //可加血类型
                    if (Player_Controller.Instance.HP != 100)
                    {
                        Player_Controller.Instance.ApplyHealth(this.GetItemUI_ItemConfig().useable_Config.healthAmount); //玩家加血
                        this.GetItemUI_Inventoryitem().amount -= 1; //物品减一
                        this.GetComponentInParent<SlotHolder>().UpdateItem(); //重新更新格子
                    }
                    break;
            }
        }
    }
    #endregion

    #region 更新格子物品显示

    public void SetupItemUI(Item_Config item_Config, int itemAmount)
    {
        //如果item数量为0 就消失
        if (itemAmount == 0)
        {
            Bag.Inventory_items[Index].item_Config = null; //配置为空
            icon.gameObject.SetActive(false); //照片失效
            return;
        }

        //如果物品对应 背包配置 不为 null
        if (item_Config != null)
        {
            icon.sprite = item_Config.itemIcon; //图片数量
            amount.text = itemAmount.ToString(); //数量
            icon.gameObject.SetActive(true);  //物品显示（图片，数量）
        }
        else //如果物品对应 背包配置 为 null
        {
            icon.gameObject.SetActive(false); //物品不显示（图片，数量）
        }
    }

    #endregion

    #region 得到Inventory_items[i]=Inventory_item（.item_Config）

    //得到当前ItemUI对应容器Inventory_items中对应的Inventory_item （一一对应 ）
    public InventoryItem GetItemUI_Inventoryitem()
    {
        return Bag.Inventory_items[Index];
    }

    //得到当前ItemUI对应容器Inventory_items中对应的Item_Config （一一对应 ）
    public Item_Config GetItemUI_ItemConfig()
    {
        return Bag.Inventory_items[Index].item_Config;
    }

    #endregion
}
