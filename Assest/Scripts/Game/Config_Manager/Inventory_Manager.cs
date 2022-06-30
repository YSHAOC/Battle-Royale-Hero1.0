using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using UnityEngine.UI;

public class Inventory_Manager : LogicManagerBase<Inventory_Manager>
{

    #region 注册事件 / 取消事件
    protected override void RegisterEventListener()
    {

    }
    protected override void CancelEventListener()
    {

    }
    #endregion

    //Inventory_Manger临时存放当前拖拽的物品信息
    public class DragData
    {
        public SlotHolder originaHolder;
        public RectTransform originalParent;
    }
    private LV_Config lv_config;


    [Header("各类型容器_配置")]
    public Inventory_Config bag_Config; //背包容器_配置
    public Inventory_Config actionc_Config; //可使用物品容器_配置
    public Inventory_Config equipment_Config; //装备容器_配置


    [Header("各类型容器_脚本")]
    public Bag_ContainerUI bag_Container; //背包容器
    public ContainerUI action_Container; //可使用物品容器
    public ContainerUI equipment_Container; //装备容器

    [Header("Drag Canvas")] //物品UI显示拖拽的Canvas
    public Canvas dragCanvas;

    [Header("Item Tooltip")] //物品UI显示信息栏
    public ItemTooltip tooltip;

    public DragData currentDrag; //在Inventory_Manger临时存放当前拖拽的物品信息

    //--------------重要---------------------------这里要用Awake（）不能用Start（）----------这里是游戏最开始要赋值的------
    //----不然在其他逻辑Start中 拿不到这里
    protected override void Awake()
    {
        base.Awake();
        lv_config = ConfigManager.Instance.GetConfig<LV_Config>("LV");

        bag_Config = lv_config.GetConfig<Inventory_Config>("Inventory", 0);
        actionc_Config = lv_config.GetConfig<Inventory_Config>("Inventory", 1);
        equipment_Config = lv_config.GetConfig<Inventory_Config>("Inventory", 2);


    }

    private void Start()
    {

        //bag_Container.RefreshUI();
        //action_Container.RefreshUI();
        //equipment_Container.RefreshUI();
    }

    #region 检查拖拽物品是否每一个 Slot 范围内

    //检查是否在 背包容器 格子上
    public bool CheckInInventoryUI(Vector3 position)
    {
        for (int i = 0; i < bag_Container.bag_amount; i++)
        {
            if (bag_Container.slotHolders1[i] != null)
            {
                RectTransform t = bag_Container.slotHolders1[i].transform as RectTransform;

                if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
                {
                    return true;
                }
            }
        }
        return false;

    }

    //检查是否在 可使用物品容器 格子上
    public bool CheckInActionUI(Vector3 position)
    {
        for (int i = 0; i < action_Container.slotHolders.Length; i++)
        {
            RectTransform t = action_Container.slotHolders[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }

    //检查是否在 装备容器 格子上
    public bool CheckInEquipmentUI(Vector3 position)
    {
        for (int i = 0; i < equipment_Container.slotHolders.Length; i++)
        {
            RectTransform t = equipment_Container.slotHolders[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;

    }

    #endregion


    //丢弃区域
    public Image Throw_Area;

    //检查是否在 丢弃区域 上
    public bool CheckInThrow_Area(Vector3 position)
    {

        RectTransform t = Throw_Area.transform as RectTransform;
        if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
        {
            return true;
        }

        return false;

    }
}
