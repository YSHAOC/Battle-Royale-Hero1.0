using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using Sirenix.OdinInspector;

//可使用类型
public enum UseItem_Type
{
    ApplyHP //加血
}
[CreateAssetMenu(menuName = "配置/可使用物品_配置", fileName = "Usable_Data")]
public class UseableItem_Config : ConfigBase
{
    [LabelText("名称")]
    public string Name;
    

    public UseItem_Type useItem_Type; //使用类型
    public int healthAmount; //回血量
}
