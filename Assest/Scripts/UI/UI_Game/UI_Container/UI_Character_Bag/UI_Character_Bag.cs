using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Character_Bag : MonoBehaviour
{
    [SerializeField] public Text bagAmount_text; //武器名称

    public Bag_ContainerUI bag_Container;

    public void Init()
    {
        bag_Container = GetComponentInChildren<Bag_ContainerUI>();
        bag_Container.Init();
    }

    //更新背包格子数量显示
    public void UpdateBagAmount(int bag_amount)
    {
        bagAmount_text.text = String.Format("x{0}", bag_amount.ToString());
    }

}
