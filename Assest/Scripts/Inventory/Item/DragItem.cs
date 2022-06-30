using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using JKFrame;


[RequireComponent(typeof(ItemUI))] //确保当前GameObject有ItemUI组件
public class DragItem : MonoBehaviour
{
    ItemUI currentItemUI; //当前ItemUI
    SlotHolder currentHolder; //当前SlotHolder
    SlotHolder targetHolder; //目标SlotHolder

    void Start()
    {
        currentItemUI = GetComponent<ItemUI>();
        currentHolder = GetComponentInParent<SlotHolder>();


        this.OnBeginDrag(BeginDrag); //开始拖拽
        this.OnDrag(Drag);     //拖拽中
        this.OnEndDrag(EndDrag);  //结束拖拽
    }

    public void BeginDrag(PointerEventData eventData, params object[] args)
    {

        //当拖拽时保存当前拖拽的的 SlotHolder 信息
        Inventory_Manager.Instance.currentDrag = new Inventory_Manager.DragData();
        Inventory_Manager.Instance.currentDrag.originaHolder = GetComponentInParent<SlotHolder>();
        Inventory_Manager.Instance.currentDrag.originalParent = (RectTransform)transform.parent;

        //记录原始数据
        transform.SetParent(Inventory_Manager.Instance.dragCanvas.transform, true);

    }
    public void Drag(PointerEventData eventData, params object[] args)
    {

        //跟随鼠标位置移动
        transform.position = eventData.position;

    }
    public void EndDrag(PointerEventData eventData, params object[] args)
    {

        //放下物品 交换数据
        //是否指向UI物品
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (Inventory_Manager.Instance.CheckInActionUI(eventData.position) ||
                Inventory_Manager.Instance.CheckInEquipmentUI(eventData.position) ||
                Inventory_Manager.Instance.CheckInInventoryUI(eventData.position))
            {
                if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())
                    targetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>();
                else
                    targetHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();
                Debug.Log(eventData.pointerEnter.gameObject);

                targetHolder.UpdateItem();//目标格子更新

                switch (targetHolder.slotType) //--------如果这里出错 看看Drag Canvas是否删除两个组件 或 看以下另一个错误
                {
                    case SlotType.BAG: //背包
                        SwapItem();
                        break;

                    case SlotType.WEAPON: //武器
                        if (currentItemUI.Bag.Inventory_items[currentItemUI.Index].item_Config.itemType == ItemType.Weapon)
                        {
                            //Player_Controller.Instance.isCanUpdate = true;
                            SwapItem();
                        }
                        break;

                    case SlotType.BAG_EQUIP: //背包装备
                        if (currentItemUI.Bag.Inventory_items[currentItemUI.Index].item_Config.itemType == ItemType.Bag_Equip)
                        {
                            SwapItem();
                        }
                        break;

                    case SlotType.HELMET: //头盔
                        if (currentItemUI.Bag.Inventory_items[currentItemUI.Index].item_Config.itemType == ItemType.Helmet)
                        {
                            SwapItem();
                        }
                        break;

                    case SlotType.ARMOR: //防御甲
                        if (currentItemUI.Bag.Inventory_items[currentItemUI.Index].item_Config.itemType == ItemType.Armor)
                        {
                            //Player_Controller.Instance.isCanUpdate = true;
                            SwapItem();
                        }
                        break;


                    case SlotType.ACTION: //可使用
                        if (currentItemUI.Bag.Inventory_items[currentItemUI.Index].item_Config.itemType == ItemType.Useable)
                            SwapItem();
                        break;
                }

                //交换
                currentHolder.UpdateItem();//当前格子更新
                targetHolder.UpdateItem();//目标格子更新

            }

            //检测是否在丢弃的区域内  -- 实现物品的丢弃
            if (Inventory_Manager.Instance.CheckInThrow_Area(eventData.position))
            {
                InventoryItem currentHolder_inventoryItem = currentHolder.itemUI.Bag.Inventory_items[currentHolder.itemUI.Index]; //容器物品item配置
                ItemPickUp itemPickUp = ResManager.Load<ItemPickUp>("Prefabs/Items/OnWorld/" + currentHolder_inventoryItem.item_Config.itemPrefab.name);//物品item加载预制体
                itemPickUp.gameObject.transform.position = Player_Controller.Instance.transform.position + Vector3.forward; //预制体生成位置
                itemPickUp.item_Config.itemAmount = currentHolder_inventoryItem.amount; //物品item预制体数量
                currentHolder_inventoryItem.amount = 0; //容器item数量修改为0
                currentHolder.UpdateItem();//当前格子更新
            }
            currentHolder.UpdateItem();//当前格子更新

        }

        //如果不是在可交换 物品的UI 上 就格子回初始位置
        transform.SetParent(Inventory_Manager.Instance.currentDrag.originalParent);
        RectTransform t = transform as RectTransform;
        t.offsetMax = -Vector2.one * 5; //主要这里是Vector2类型
        t.offsetMin = Vector2.one * 5;

    }

    //交换物品 （其实是交换 格子SlotHolder ->物品itemUI中 -> InventoryItems ->的InventoryItem）
    public void SwapItem()
    {
        //--------如果这里出错---------
        //格子显示出来后要更新 Inventory_Config equipment_Config(action_Container).RefreshUI()--------（重要）：Inventory_Manager初始化慢了要用Awake而不是Start
        //试着看一下 UGUI（itemUI) 的Rectranform 是否是全覆盖父级
        var targetItem = targetHolder.itemUI.Bag.Inventory_items[targetHolder.itemUI.Index]; //临时存放 目标格子的 InventoryItem
        var tempItem = currentHolder.itemUI.Bag.Inventory_items[currentHolder.itemUI.Index]; //临时存放 当前格子的 InventoryItem

        bool isSameItem = tempItem.item_Config == targetItem.item_Config; //是否相同

        //相同 且 可堆叠 且不是自己targetHolder！=currentHolder（不然拖回重新放回回消失物品）
        if (isSameItem && targetItem.item_Config.stackable && targetHolder != currentHolder)
        {
            targetItem.amount += tempItem.amount;
            tempItem.item_Config = null;
            tempItem.amount = 0;
        }
        else  //不相同 且 不可堆叠  （交换）
        {
            currentHolder.itemUI.Bag.Inventory_items[currentHolder.itemUI.Index] = targetItem;
            targetHolder.itemUI.Bag.Inventory_items[targetHolder.itemUI.Index] = tempItem;
        }
    }

}
