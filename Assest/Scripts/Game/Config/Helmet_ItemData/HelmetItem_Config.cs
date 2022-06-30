using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using JKFrame;

[CreateAssetMenu(menuName = "配置/头盔物品_配置", fileName = "Helmet_Data")]
public class HelmetItem_Config : ConfigBase
{
    [LabelText("名称")]
    public string Name;


    [LabelText("防御力")]
    public float Helmet_DefenceValue;
}

