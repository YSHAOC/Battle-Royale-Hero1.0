using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
/// <summary>
/// 怪物管理器
/// </summary>
public class Monster_Manager : LogicManagerBase<Monster_Manager>
{
    [SerializeField] private Transform[] targets;

    private LV_Config lv_config;
    private bool isCreate = true; //怪物是否可以创建
    private Monster_Controller_Base monster_controller_base;
    private Monster_Config monster_Config;

    public Transform create_Fist_Monster_point;
    protected override void Awake()
    {
        base.Awake();
        lv_config = ConfigManager.Instance.GetConfig<LV_Config>("LV");

    }

    // 创建怪物
    public void CreateMonster_Controller<T>(Transform creatMonsterPoint) where T : Monster_Controller_Base
    {
        string Name_Monster_Controller = typeof(T).Name;  //怪物挂载Monster_Controller脚本名（Fist_Monster_Controller）
        string Monster_Controller_LoadPath = "Monster_Contral/" + Name_Monster_Controller; //怪物加载路径 （ Monster_Contral/Fist_Monster_Controller ）


        //monster_config(怪物配置)
        monster_Config = GetMonsterConfig1<T>(Name_Monster_Controller); //怪物挂载Monster_Controller脚本名 获取 怪物配置


        //monster_controller(怪物控制体)
        // monster_controller_base = PoolManager.Instance.GetGameObject<T>(monster_Config.MonsterControllerPrefab, LVManager.Instance.TempObjRoot); //加载怪物
        monster_controller_base = ResManager.Load<T>(Monster_Controller_LoadPath, LVManager.Instance.TempObjRoot); //加载怪物

        if (monster_controller_base != null)
        {
            monster_controller_base.transform.position = creatMonsterPoint.position; //设置怪物生成位置
            monster_controller_base.Init(monster_Config); //怪物初始化
        }
    }

    //获取怪物配置（ LV_Config中获取 ）
    private Monster_Config GetMonsterConfig1<T>(string Name_Monster_Controller) where T : Monster_Controller_Base
    {
        for (int i = 0; i < lv_config.CreateMonsterConfigs.Length; i++) //遍历怪物列表配置
        {
            //（ Fist_Monster        __Controller   ==    Fist_Monster_Controller ）
            //如果怪物配置中的预制体名字__Controller == 怪物挂载Monster_Controller脚本名
            if (lv_config.CreateMonsterConfigs[i].Monster_Config.MonsterViewPrefab.name + "_Controller" == Name_Monster_Controller)
            {
                return lv_config.CreateMonsterConfigs[i].Monster_Config; //返回怪物配置
            }
        }
        return null;
    }

    /// <summary>
    /// 获取巡逻点
    /// </summary>
    public Vector3 GetPatrolTarget()
    {
        return targets[Random.Range(0, targets.Length)].position;
    }

    //注册事件
    protected override void RegisterEventListener()
    {
        EventManager.AddEventListener("MonsterDie", OnMonsterDie);
    }
    //取消事件
    protected override void CancelEventListener()
    {
        EventManager.RemoveEventListener("MonsterDie", OnMonsterDie);
    }

    private void OnMonsterDie()
    {
        //monsterCount -= 1;
        isCreate = true;
    }

    #region 方法二
    /* 
    #region 怪物生成位置

    [SerializeField] private Transform create_Fist_Monster_point;
    [SerializeField] private Transform create_Iron_Monster_point;
    [SerializeField] private Transform create_Skeleton_Monster_point;
    [SerializeField] private Transform create_Orc_Monster_point;
    [SerializeField] private Transform create_Dragon_Monster_point;
    [SerializeField] private Transform create_Plant_Monster_point;
    #endregion


    [SerializeField] private Transform[] targets;

    private LV_Config lv_config;
    private bool isCreate = true; //怪物是否可以创建

    private void Start()
    {
        lv_config = ConfigManager.Instance.GetConfig<LV_Config>("LV");
        // InvokeRepeating("CreateMonster", config.CreatMonsterInterval, 1);
        // InvokeRepeating("CreateFist_Monster", lv_config.CreatMonsterInterval, 1);

        CreateFist_Monster(); //创建Fist_Monster怪物
        CreateIron_Monster(); //创建Iron_Monster怪物
        CreateSkeleton_Monster(); //创建Skeleton_Monster怪物
        CreateOrc_Monster(); //创建Orc_Monster怪物
        CreateDragon_Monster();//创建Dragon_Monster怪物
        CreatePlant_Monster();//创建Plant_Monster怪物
    }

    #region 创建怪物
    //创建Fist_Monster怪物
    private void CreateFist_Monster()
    {
        CreateMonster<Fist_Monster_Controller>(create_Fist_Monster_point);
    }
    //创建Iron_Monster怪物
    private void CreateIron_Monster()
    {
        CreateMonster<Iron_Monster_Controller>(create_Iron_Monster_point);
    }
    //创建Iron_Monster怪物
    private void CreateSkeleton_Monster()
    {
        CreateMonster<Skeleton_Monster_Controller>(create_Skeleton_Monster_point);
    }
    //创建Orc_Monster怪物
    private void CreateOrc_Monster()
    {
        CreateMonster<Orc_Monster_Controller>(create_Orc_Monster_point);
    }
    //创建Dragon_Monster怪物
    private void CreateDragon_Monster()
    {
        CreateMonster<Dragon_Monster_Controller>(create_Dragon_Monster_point);
    }
    //创建Plant_Monster怪物
    private void CreatePlant_Monster()
    {
        CreateMonster<Plant_Monster_Controller>(create_Plant_Monster_point);
    }
    #endregion


    //注册事件
    protected override void RegisterEventListener()
    {
        EventManager.AddEventListener("MonsterDie", OnMonsterDie);
    }
    //取消事件
    protected override void CancelEventListener()
    {
        EventManager.RemoveEventListener("MonsterDie", OnMonsterDie);
    }

    private void OnMonsterDie()
    {
        //monsterCount -= 1;
        isCreate = true;
    }

    // 每X秒生成一只怪物
    private void CreateMonster<T>(Transform creatMonsterPoint) where T : Monster_Controller
    {
        string Name_Monster_Controller = typeof(T).Name;  //怪物挂载Monster_Controller脚本名（Fist_Monster_Controller）
        string Monster_Controller_LoadPath = "Monster_Contral/" + Name_Monster_Controller; //怪物加载路径 （ Monster_Contral/Fist_Monster_Controller ）

        //monster_config(怪物配置)
        Monster_Config monster_Config = GetMonsterConfig<T>(Name_Monster_Controller); //怪物挂载Monster_Controller脚本名 获取 怪物配置



        //monster_controller(怪物控制器)

        //方法一:基于（关卡配置文件中monster_config的 预制体MonsterControllerPrefab）---- PoolManager.Instance.GetGameObject<T>(GameObject prefab, Transform parent = null);    
        // --重要-- 这里可以进行一个优化：monster_config的加载可以通过monster_config的MonsterControllerPrefab
        //Monster_Controller monste = PoolManager.Instance.GetGameObject<T>(monster_Config.MonsterControllerPrefab, LVManager.Instance.TempObjRoot); //加载怪物

        //方法二：基于（脚本名组成的 路径名LoadPath）------ ResManager.Load<T>(string path, Transform parent = null);
        Monster_Controller monste = ResManager.Load<T>(Monster_Controller_LoadPath, LVManager.Instance.TempObjRoot); //加载怪物


        monste.transform.position = creatMonsterPoint.position; //设置怪物生成位置
        monste.Init(monster_Config); //怪物初始化

    }

    //获取怪物配置（ LV_Config中获取 ）
    public Monster_Config GetMonsterConfig<T>(string Name_Monster_Controller) where T : Monster_Controller
    {
        for (int i = 0; i < lv_config.CreateMonsterConfigs.Length; i++) //遍历怪物列表配置
        {
            //（ Fist_Monster        __Controller   ==    Fist_Monster_Controller ）
            //如果怪物配置中的预制体名字__Controller == 怪物挂载Monster_Controller脚本名
            if (lv_config.CreateMonsterConfigs[i].Monster_Config.MonsterPrefab.name + "_Controller" == Name_Monster_Controller)
            {
                return lv_config.CreateMonsterConfigs[i].Monster_Config; //返回怪物配置
            }
        }
        return null;
    }


    //// 每X秒生成一只怪物
    //private void CreateMonster()
    //{
    //    // 当前未达到怪物上限
    //    if (monsterCount<config.MaxMonsterCountOnScene)
    //    {
    //        float randomNum = Random.value;
    //        monsterCount += 1;
    //        for (int i = 0; i < config.CreateMonsterConfigs.Length; i++)
    //        {
    //            // 当前随机数大于配置中的概率
    //            if (randomNum>config.CreateMonsterConfigs[i].Probability)
    //            {
    //                Monster_Controller monste = ResManager.Load<Monster_Controller>("Monster", LVManager.Instance.TempObjRoot);
    //                monste.transform.position = creatMonsterPoint.position;
    //                monste.Init(config.CreateMonsterConfigs[i].Monster_Config);
    //                return;
    //            }
    //        }
    //    }
    //}


    /// <summary>
    /// 获取巡逻点
    /// </summary>
    public Vector3 GetPatrolTarget()
    {
        return targets[Random.Range(0, targets.Length)].position;
    }

 */

    #endregion

}
