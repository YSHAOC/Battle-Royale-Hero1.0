using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;

//[Pool]
public class ItemTooltip : MonoBehaviour
{
    public Text itemNameText; //物品名字
    public Text itemInfoText; //物品的信息

    RectTransform rectTransform; //物品栏UI位置

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    //物品显示栏目的信息赋值
    public void SetupTooltip(Item_Config item_Config)
    {
        itemNameText.text = item_Config.itemName; //物品的名字
        itemInfoText.text = item_Config.description; //物品的信息
    }

    private void OnEnable() //每次启动的时候（SetActive） 先更新一下鼠标 
    {
        UpdatePosition();
    }


    private void Update()
    {
        UpdatePosition();
    }

    //更新UI位置 为 鼠标实时位置 
    public void UpdatePosition()
    {
        Vector3 mousePos = Input.mousePosition;

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float width = corners[3].x - corners[0].x;
        float height = corners[1].y - corners[0].y;

        if (mousePos.y < height)
        {
            rectTransform.position = mousePos + Vector3.up * height * 0.6f;
        }
        else if (Screen.width - mousePos.x > width)
        {
            rectTransform.position = mousePos + Vector3.right * width * 0.6f;
        }
        else
        {
            rectTransform.position = mousePos + Vector3.left * width * 0.6f;
        }
    }

    /// 放进对象池
    public void Destroy()
    {
        this.JKGameObjectPushPool();
    }
}
