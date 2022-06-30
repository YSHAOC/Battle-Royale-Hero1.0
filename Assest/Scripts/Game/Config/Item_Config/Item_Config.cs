using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

public enum ItemType { Useable, Weapon, Weapon_Bullet, Helmet, Armor, Bag_Equip }

[CreateAssetMenu(fileName = "Item_Config", menuName = "配置/物品配置")]
public class Item_Config : ConfigBase
{
    public ItemType itemType; //类型
    public string itemName;   //名字
    public GameObject itemPrefab;//预制体
    public Sprite itemIcon;   //图片
    public int itemAmount;   //数量

    [TextArea]
    public string description = "";  //描述

    public bool stackable;  //是否可以堆叠

#region 方法一 //抽象类继承 （设计模型）
    [Header("Any Item_Config")]
    public ConfigBase item_Data;
#endregion 


    //可使用物品
    [Header("Useable Item")]
    public UseableItem_Config useable_Config; //可使用物品配置

    //武器
    [Header("Weapon")]
    public GameObject weaponPrefab; //武器预制体
    public WeaponItem_Config weapon_Config; //武器属性配置
    public RuntimeAnimatorController weapon_animator; //武器特定动画

    //武器-子弹
    [Header("Weapon_Bullet")]
    public GameObject Weapon_BulletPrefab; //武器预制体

    //头盔
    [Header("Helmet")]
    public GameObject helmetPrefab;
    public DefenceItem_Config helmetItem_Config;


    //防御甲
    [Header("Armor")]
    public GameObject defencePrefab; //防御甲预制体
    public DefenceItem_Config defenceItem_Config; //防御甲属性配置


    //背包装备
    [Header("Bag_equip")]
    public GameObject bagPrefab;//背包装备预制体
    public BagEquipItem_Config bagEquipItem_Config; //背包装备属性配置

}
