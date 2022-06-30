using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

public class Bag_ContainerUI : ContainerUI
{
    public SlotHolder[] slotHolders1; //背包格子数组
    public int bag_amount; //背包格子数

    //履带对象  需要通过他得到可视范围的位置  还要把动态创建的格子设置为他的子对象
    public RectTransform content;

    //可视范围高
    public int viewPortH;

    //当前显示着的格子对象
    private Dictionary<int, GameObject> nowShowItems = new Dictionary<int, GameObject>();

    //记录当前显示的索引范围
    private int minIndex;
    private int maxIndex;
    //记录上一次显示的索引范围
    private int oldMinIndex = -1;
    private int oldMaxIndex = -1;

    //初始化
    public void Init()
    {
        bag_amount = 20; //50个格子
        slotHolders1 = new SlotHolder[bag_amount];
        //应该要初始化履带的长度content的高
        content.sizeDelta = new Vector2(0, Mathf.CeilToInt(bag_amount / 5) * 105); //Mathf.CeilToInt()向上取整 就是0.5行其实也得算+1
        CheckShowOrHide();// 更新格子显示的方法

    }
    //重新更新背包--格子数量
    public void UpdateBag_Amount(int BagAmount)
    {
        minIndex = -1;
        maxIndex = -1;
        oldMinIndex = -1;
        oldMaxIndex = -1;
        bag_amount = 20; //20个格子
        DeleteUI1();

        this.bag_amount = BagAmount;
        slotHolders1 = new SlotHolder[bag_amount];//重新分配数组内存空间
        //应该要初始化履带的长度content的高
        content.sizeDelta = new Vector2(0, Mathf.CeilToInt(bag_amount / 5) * 105); //Mathf.CeilToInt()向上取整 就是0.5行其实也得算+1
        CheckShowOrHide();// 更新格子显示的方法
    }
    public void DeleteUI1() //清除背包容器所有格子（重新初始）
    {
        nowShowItems.Clear();
        //销毁gameObject &&  数组的slotHolders1引用置null,方便GC垃圾回收  
        for (int i = 0; i <= slotHolders1.Length - 1; i++)
        {
            if (slotHolders1[i] != null)
            {
                Destroy(slotHolders1[i].gameObject);
                //slotHolders1[i].JKGameObjectPushPool();
                //PoolManager.Instance.PushGameObject(slotHolders1[i].gameObject);
                slotHolders1[i] = null;
            }
        }
        Array.Clear(slotHolders1, 0, slotHolders1.Length); //数组清空
        //slotHolders1 = new SlotHolder[bag_amount];  //重新分配数组内存空间
    }

    public void RefreshUI1() //更新--格子信息
    {
        for (int i = minIndex; i <= maxIndex; i++)
        {
            slotHolders1[i].UpdateItem();
        }
    }




    //Update函数
    private void Update()
    {
        CheckShowOrHide();// 更新--格子显示的方法
    }

    /// <summary>
    /// 更新格子显示的方法
    /// </summary>
    public void CheckShowOrHide()
    {
        // 如果小于0了 就不处理更新相关
        if (content.anchoredPosition.y < 0)
            return;

        //检测哪些格子应该显示出来
        minIndex = (int)(content.anchoredPosition.y / 105) * 5;
        maxIndex = (int)((content.anchoredPosition.y + viewPortH) / 105) * 5 + 5 - 1;


        //最小值判断
        if (minIndex < 0)
            minIndex = 0;

        //超出道具最大数量
        if (maxIndex >= bag_amount)
        {
            maxIndex = bag_amount - 1;

        }

        //删除格子-----------------------------------------------------------
        if (minIndex != oldMinIndex ||
            maxIndex != oldMaxIndex)
        {
            //删除上一节溢出 （向上滑 删 上）
            for (int i = oldMinIndex; i < minIndex; ++i)
            {
                if (nowShowItems.ContainsKey(i))
                {
                    if (nowShowItems[i] != null)
                        PoolManager.Instance.PushGameObject(nowShowItems[i]);
                    nowShowItems.Remove(i);
                }
            }
            //删除下一节溢出  （向下滑 删 下）
            for (int i = maxIndex + 1; i <= oldMaxIndex; ++i)
            {
                if (nowShowItems.ContainsKey(i))
                {
                    if (nowShowItems[i] != null)
                        PoolManager.Instance.PushGameObject(nowShowItems[i]);
                    nowShowItems.Remove(i);
                }
            }
        }

        //在记录当前索引之前 要做一些事儿
        //根据上一次索引和这一次新算出来的索引 用来判断 哪些该移除
        oldMinIndex = minIndex;
        oldMaxIndex = maxIndex;

        //创建格子----------------------------------------------------------
        //创建指定索引范围内的格子
        for (int i = minIndex; i <= maxIndex; ++i)
        {
            if (nowShowItems.ContainsKey(i))
                continue;
            else
            {
                int index = i;
                nowShowItems.Add(index, null);

                SlotHolder slotHolder = ResManager.Load<SlotHolder>("Slot Holder 1", content);
                slotHolder.gameObject.transform.localScale = Vector3.one;
                slotHolder.gameObject.transform.localPosition = new Vector3((index % 5) * 105, -index / 5 * 105, 0);


                slotHolders1[index] = slotHolder;
                slotHolders1[index].itemUI.Index = index;
                slotHolders1[index].UpdateItem();


                //判断有没有这个坑
                if (nowShowItems.ContainsKey(index))
                    nowShowItems[index] = slotHolder.gameObject;
                else
                    PoolManager.Instance.PushGameObject(nowShowItems[i]);
            }
        }

    }
}
