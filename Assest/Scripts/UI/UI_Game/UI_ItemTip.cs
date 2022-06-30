using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using UnityEngine.UI;

[Pool]
public class UI_ItemTip : MonoBehaviour
{
    //销毁
    public void Destroy()
    {
        //移除事件
        //放进对象池
        this.JKGameObjectPushPool();
    }
}
