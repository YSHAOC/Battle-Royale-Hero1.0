using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using JKFrame;

[CreateAssetMenu(menuName = "配置/背包装备物品_配置", fileName = "BagEquip_Data")]
public class BagEquipItem_Config : ConfigBase
{
    [LabelText("名称")]
    public string Name;

    [LabelText("背包格子数")]
    public int Bag_Amount;
}
