using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    public KeyCode actionKey; //按键

    public ItemUI currentItemUI; //ItemUI物品

    private void Start() //GetComponentInChildren 的话就用 Start 不用 Awake不然会报错
    {
        //currentItemUI = GetComponentInChildren<ItemUI>(); //初始化组件 ------因为子物体itemUI一开始是失活的 所以没法动态赋值
    }

    private void Update()
    {
        //if(currentItemUI==null)
        //{
        //    currentItemUI = GetComponentInChildren<ItemUI>(); //初始化组件 ------因为子物体itemUI一开始是失活的 所以没法动态赋值
        //}

        if (Input.GetKeyDown(actionKey)&& currentItemUI.GetItemUI_ItemConfig()) //如果按下该按键就
        {
            currentItemUI.UseItem(); //使用物品

        }
    }
}
