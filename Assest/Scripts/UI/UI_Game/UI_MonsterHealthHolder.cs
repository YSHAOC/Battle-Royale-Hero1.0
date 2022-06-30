using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;

[Pool]
public class UI_MonsterHealthHolder : MonoBehaviour
{
    [SerializeField] private Image healthUIPrefab;


    //更新血条
    public void UpdateMonsterHP(int currentHealth, int maxHealth = 100)
    {
        float CurrentHealth = (float)currentHealth / maxHealth;
        healthUIPrefab.fillAmount = CurrentHealth;
    }

    /// 放进对象池
    public void Destroy()
    {
        healthUIPrefab.fillAmount = 1;
        this.JKGameObjectPushPool();
    }
}
