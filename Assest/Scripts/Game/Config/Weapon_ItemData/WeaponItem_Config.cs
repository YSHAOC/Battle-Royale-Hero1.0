using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using JKFrame;

[CreateAssetMenu(menuName = "配置/武器物品_配置", fileName = "Weapon_Data")]
public class WeaponItem_Config : ConfigBase
{
    [LabelText("名称")]
    public string Name;


    [LabelText("子弹类型")]
    public string Bullet_Type;

    [LabelText("最大子弹数量")]
    public int MaxBulletNum = 30;

    [LabelText("射击间隔")]
    public float ShootInterval = 0.02f;

    [LabelText("子弹移动力量")]
    public float BulletMovePower = 1000;

    [LabelText("攻击力")]
    public int Attack = 20;
}
