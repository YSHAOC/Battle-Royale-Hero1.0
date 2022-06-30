using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

[Pool]
public class ItemPickUp : MonoBehaviour
{
    public Item_Config item_Config; //物品配置文件
    public UI_ItemTip uI_ItemTip; //物品显示ui
    public Transform ui_itemTip_transform; //物品提示ui显示位置

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (item_Config != null)
            {
                if (Inventory_Manager.Instance.bag_Container != null)
                {
                    //将物品添加到背包
                    Inventory_Manager.Instance.bag_Config.AddItem(item_Config, item_Config.itemAmount);

                    //当捡起时更新 背包
                    Inventory_Manager.Instance.bag_Container.RefreshUI1();
                }
                //Destroy(gameObject);
                Destroy_Item(); //销毁物品
            }
        }

        //如果捡到的是子弹类物品 就更新 子弹UI显示
        Player_Controller.Instance.UpdateBulletShow();
    }

    private void Update()
    {
        Show_UI_ItemTip(); //时刻检测显示物品提示ui
    }

    private void LateUpdate()
    {
        if (uI_ItemTip != null)    //时刻更新uI_ItemTip显示朝向 与位置信息
        {
            uI_ItemTip.transform.position = ui_itemTip_transform.position; //位置信息
            ui_itemTip_transform.forward = Camera.main.transform.forward; //ui朝向 方向
        }
    }

    //显示--物品提示ui
    private void Show_UI_ItemTip()
    {
        //为空
        if (uI_ItemTip == null)
        {
            //判断物品与人物的位置大小
            if (Vector3.Distance(Player_Controller.Instance.transform.position, this.transform.position) < 8.0f)
            {
                this.gameObject.layer = 14; //设置物品layer URP渲染透明
                uI_ItemTip = ResManager.Load<UI_ItemTip>("ItemTip", LVManager.Instance.World_Canvas.transform); //加载
                if (uI_ItemTip != null)
                {
                    LVManager.Instance.World_Canvas.worldCamera = Camera.main;
                    uI_ItemTip.transform.position = ui_itemTip_transform.position; //设置位置
                }
            }
        }

        //不为空
        if (uI_ItemTip != null)
        {
            //判断物品与人物的位置大小
            if (Vector3.Distance(Player_Controller.Instance.transform.position, this.transform.position) > 8.0f)
            {
                this.gameObject.layer = 0;//设置物品layer URP关闭渲染透明
                uI_ItemTip.Destroy();//ui放进对象池 
                uI_ItemTip = null; //置空垃圾 GC回收
            }
        }

    }

    //物体--销毁
    public void Destroy_Item()
    {
        if (uI_ItemTip != null)
        {
            uI_ItemTip.Destroy();//ui放进对象池 
            uI_ItemTip = null; //置空垃圾 GC回收
            this.JKGameObjectPushPool();
        }
    }
}
