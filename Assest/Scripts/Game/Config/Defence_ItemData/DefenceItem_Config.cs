using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using JKFrame;

[CreateAssetMenu(menuName = "配置/防御物品_配置", fileName = "Defence_Data")]
public class DefenceItem_Config : ConfigBase
{
    [LabelText("名称")]
    public string Name;
    

    [LabelText("防御力")]
    public float Defence_Value;
}
