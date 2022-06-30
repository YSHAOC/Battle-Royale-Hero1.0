using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using UnityEngine.UI;

[UIElement(true, "UI/CharacterInventory", 1)]
public class UI_CharacterInventory : UI_WindowBase
{
    [SerializeField] UI_Character_Bag uI_Character_Bag; //背包 面板UI

    [SerializeField] UI_Character_Stats uI_Character_Stats;  //角色信息 面板UI

    [SerializeField] Image image_Throw_Area; //丢弃物品区域

    private WeaponItem_Config weaponItem_Config; //角色武器配置信息

    //初始化
    public override void Init()
    {
        uI_Character_Bag = GetComponentInChildren<UI_Character_Bag>();
        uI_Character_Stats = GetComponentInChildren<UI_Character_Stats>();

        uI_Character_Bag.Init();
        uI_Character_Stats.Init();
        Inventory_Manager.Instance.bag_Container = uI_Character_Bag.bag_Container;
        Inventory_Manager.Instance.equipment_Container = uI_Character_Stats.equipment_Container;

        Inventory_Manager.Instance.Throw_Area = image_Throw_Area; //可丢弃区域赋值
    }

    //显示
    public override void OnShow()
    {
        base.OnShow();
        Inventory_Manager.Instance.bag_Container = uI_Character_Bag.bag_Container;
        Inventory_Manager.Instance.equipment_Container = uI_Character_Stats.equipment_Container;

        uI_Character_Bag.bag_Container.RefreshUI1();//更新背包格子

        Player_Controller.Instance.isCanUpdate = false;//这里是显示而已  不是自己用交换物品个来更新
        uI_Character_Stats.equipment_Container.RefreshUI(); //更新装备格子
        Player_Controller.Instance.isCanUpdate = true;//这里是显示而已  不是自己用交换物品个来更新

        //这里应该放在这里赋值 RefreshUI()后 ，不然weaponItem_Config就为空了
        if (Player_Controller.Instance.weaponItem_Config != null)
        {
            weaponItem_Config = Player_Controller.Instance.weaponItem_Config; //角色武器配置信息 赋值
        }
        uI_Character_Stats.Update_WeaponInfo(weaponItem_Config); //更新角色信息显示

    }

    //注册事件
    protected override void RegisterEventListener()
    {
        base.RegisterEventListener();
        //添加更新角色信息事件
        EventManager.AddEventListener<int>("UpdateHP", uI_Character_Stats.Update_HP); //血量
        EventManager.AddEventListener<int>("UpdateDF", uI_Character_Stats.Update_Defence); //防御值
        EventManager.AddEventListener<WeaponItem_Config>("Resfesh_CharacterInfo", uI_Character_Stats.Update_WeaponInfo); //武器信息


        //更新背包格子
        EventManager.AddEventListener<int>("Resfesh_BagAmount", uI_Character_Bag.bag_Container.UpdateBag_Amount); //背包格子

        //更新背包格子数量显示--Text
        EventManager.AddEventListener<int>("UpdateBagAmount", uI_Character_Bag.UpdateBagAmount); //背包格子数量Text

    }

    //取消事件
    protected override void CancelEventListener()
    {
        base.CancelEventListener();
        //取消更新角色信息事件
        EventManager.RemoveEventListener<int>("UpdateHP", uI_Character_Stats.Update_HP);//血量
        EventManager.RemoveEventListener<int>("UpdateDF", uI_Character_Stats.Update_Defence);//防御值
        EventManager.RemoveEventListener<WeaponItem_Config>("Resfesh_CharacterInfo", uI_Character_Stats.Update_WeaponInfo);//武器信息

        //更新背包格子
        EventManager.RemoveEventListener<int>("Resfesh_BagAmount", uI_Character_Bag.bag_Container.UpdateBag_Amount); //背包格子

        //更新背包格子数量显示--Text
        EventManager.RemoveEventListener<int>("UpdateBagAmount", uI_Character_Bag.UpdateBagAmount); //背包格子数量Text
    }
}
