using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Character_Stats : MonoBehaviour
{
    public ContainerUI equipment_Container; //武器容器

    [SerializeField] public Text hp_info;  //血量
    [SerializeField] public Text defence_info; //防御值
    [SerializeField] public Text weapon_name; //武器名称
    [SerializeField] public Text weapon_info; //武器信息

    //初始化
    public void Init()
    {
        equipment_Container = GetComponentInChildren<ContainerUI>();
    }

    //更新血量信息
    public void Update_HP(int hp)
    {
        hp_info.text = hp.ToString();
    }

    //更新防御信息
    public void Update_Defence(int df)
    {
        defence_info.text = df.ToString();
    }

    //更新武器信息
    public void Update_WeaponInfo(WeaponItem_Config weaponItem_Config)
    {
        if (weaponItem_Config != null)   //武器信息显示更新
        {
            weapon_name.text = weaponItem_Config.Name;
            weapon_info.text = string.Format("Attack：{0}   MaxBulletNum：{1}  ShootInterval：{2}  BulletMovePower：{3}",
                weaponItem_Config.Attack, weaponItem_Config.MaxBulletNum, weaponItem_Config.ShootInterval, weaponItem_Config.BulletMovePower);
        }

    }


}
