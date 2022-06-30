using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

public class MouseManager : ManagerBase<MouseManager>
{
    public Texture2D point, doorway, attack, target, arrow;

    RaycastHit hitInfo;

    private bool isDown = false;

    private bool canMouseControl;
    private void Update()
    {
        if (!isDown)
        {
            if (!LVManager.Instance.isShow_window) SetCursorTexture();
        }

        //是否可以MouseControl
        /*         if (!LVManager.Instance.isShow_window)
                {
                    if (LVManager.Instance.isShow_window == true && LVManager.Instance.isShow_SettingWindow == false) //这里避免 打开背包 再打开设置窗口 还可以进行判断
                    {
                        MouseControl();
                    }
                } */
        MouseControl();


        //如果与显示窗口 就设置鼠标指针为 arrow类型
        if (LVManager.Instance.isShow_window == true)
        {
            Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
        }


    }

    //鼠标实时检测到的Tag类型 CursorTexture的变化
    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo))
        {
            //切换鼠标贴图
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;

                case "Monster":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;

            }
        }
    }

    //鼠标点击时 CursorTexture的变化
    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            isDown = true;

            if (hitInfo.collider.gameObject.CompareTag("Ground"))
                Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
            if (hitInfo.collider.gameObject.CompareTag("Monster"))
                Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);

        }
        if (Input.GetMouseButtonUp(0) && hitInfo.collider != null)
        {
            isDown = false;
            if (hitInfo.collider.gameObject.CompareTag("Monster"))
                Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);

        }
    }
}
