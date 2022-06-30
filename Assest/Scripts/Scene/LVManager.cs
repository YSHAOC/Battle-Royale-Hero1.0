using JKFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVManager : LogicManagerBase<LVManager>
{

    #region 怪物生成位置
    [Header("怪物生成点")]
    [SerializeField] private Transform create_Fist_Monster_point;
    [SerializeField] private Transform create_Iron_Monster_point;
    [SerializeField] private Transform create_Skeleton_Monster_point;
    [SerializeField] private Transform create_Orc_Monster_point;
    [SerializeField] private Transform create_Dragon_Monster_point;
    [SerializeField] private Transform create_Plant_Monster_point;
    #endregion


    [Header("窗口")]
    public UI_GameMainWindow ui_GameMainWindow;  //主窗口
    public UI_CharacterInventory uI_CharacterInventory; //角色容器窗口


    [Header("可使用物品容器")]
    public ContainerUI temp_action_Container; //可使用物品容器


    [Header("窗口Bool值")]
    //是否显示CharacterInventory
    public bool isShow_CharacterInventory = false;
    public bool isShow_SettingWindow = false;
    //是否显示有窗口
    public bool isShow_window = false;


    [Header("对象池世界中的父物体")]
    //对象池世界中的父物体
    public Transform TempObjRoot;

    [Header("Canvas--世界空间")]
    //怪物血条Canvas
    public Canvas World_Canvas;


    //得分
    private int score = 0;
    public int Score
    {
        get => score;
        set
        {
            score = value;
            EventManager.EventTrigger<int>("UpdateScore", score);
        }
    }


    private void Start()
    {


        GameManager.Instance.ContiuneGame(); //游戏暂停
        // 关闭全部UI
        UIManager.Instance.CloseAll();
        // 打开游戏主窗口
        ui_GameMainWindow = UIManager.Instance.Show<UI_GameMainWindow>();
        temp_action_Container = ui_GameMainWindow.action_Container;
        Inventory_Manager.Instance.action_Container = temp_action_Container;



        if (GameManager.Instance.UserData != null)
        {
            //更新分数
            LVManager.Instance.Score = GameManager.Instance.UserData.Score;
            EventManager.EventTrigger("UpdateName", GameManager.Instance.UserData.UserName);//更新名字
        }



        // 初始化玩家
        Player_Controller.Instance.Init(ConfigManager.Instance.GetConfig<Player_Config>("Player"));



        //怪物的生成
        Monster_Manager.Instance.CreateMonster_Controller<Fist_Monster_Controller>(create_Fist_Monster_point);
        Monster_Manager.Instance.CreateMonster_Controller<Iron_Monster_Controller>(create_Iron_Monster_point);
        Monster_Manager.Instance.CreateMonster_Controller<Skeleton_Monster_Controller>(create_Skeleton_Monster_point);
        Monster_Manager.Instance.CreateMonster_Controller<Orc_Monster_Controller>(create_Orc_Monster_point);
        Monster_Manager.Instance.CreateMonster_Controller<Dragon_Monster_Controller>(create_Dragon_Monster_point);
        Monster_Manager.Instance.CreateMonster_Controller<Plant_Monster_Controller>(create_Plant_Monster_point);
    }


    private void Update()
    {
        //按Esc建（设置窗口）
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isShow_window = !isShow_window;
            isShow_SettingWindow = !isShow_SettingWindow;
            // 暂停游戏，打开设置窗口
            if (isShow_SettingWindow)
            {
                GameManager.Instance.PauseGame(); //游戏暂停
                UIManager.Instance.Show<UI_SettingWindow>().InitOnGame(); //打开设置窗口 并 InitOnGame()
            }
            // 继续游戏，关闭设置窗口
            else
            {
                UIManager.Instance.Close<UI_SettingWindow>(); //关闭设置窗口
                GameManager.Instance.ContiuneGame();  //游戏启动
            }
        }

        //按B建（角色容器窗口）
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (isShow_SettingWindow == true) return; //这里避免 打开设置窗口 还 可以打开角色容器窗口

            isShow_window = !isShow_window;
            isShow_CharacterInventory = !isShow_CharacterInventory;
            // 打开Bag、CharacterStats窗口
            if (isShow_CharacterInventory)
            {

                Player_Controller.Instance.isPauseShoot = false; //玩家不可射击

                uI_CharacterInventory = UIManager.Instance.Show<UI_CharacterInventory>(); //打开角色容器窗口
                temp_action_Container.transform.SetParent(uI_CharacterInventory.transform); //设置可使用物品容器 为 角色容器窗口 的子级
            }
            //关闭Bag、CharacterStats窗口
            else
            {

                Player_Controller.Instance.isPauseShoot = true; //玩家可射击

                UIManager.Instance.Close<UI_CharacterInventory>(); //打开角色容器窗口

                temp_action_Container.transform.SetParent(ui_GameMainWindow.transform); //设置可使用物品容器 为 主窗口 的子级

            }
        }

    }



    protected override void RegisterEventListener()
    {
        EventManager.AddEventListener("MonsterDie", OnMonsterDie);
        EventManager.AddEventListener("GameOver", OnGameOver);
    }
    protected override void CancelEventListener()
    {
        EventManager.RemoveEventListener("MonsterDie", OnMonsterDie);
        EventManager.RemoveEventListener("GameOver", OnGameOver);
    }



    private void OnMonsterDie()
    {
        Score += 15;
    }



    private void OnGameOver()
    {
        // 更新存档
        GameManager.Instance.UpdateScore(score);
        // 打开结果页面
        UIManager.Instance.Show<UI_ResultWindow>().Init(score);
    }


}
