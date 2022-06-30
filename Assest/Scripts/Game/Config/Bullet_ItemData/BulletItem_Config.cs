using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using JKFrame;

[CreateAssetMenu(menuName = "配置/子弹物品_配置", fileName = "Bullet_Data")]
public class BulletItem_Config : ConfigBase
{
    [LabelText("名称")]
    public string Name;

}
