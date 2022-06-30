using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using JKFrame;

/// <summary>
/// 怪物状态类型
/// </summary>
public enum MonsterStateType
{
    Idle,
    Patrol,
    Follow,
    Attack,
    SkillAttack,
    GetHit,
    Die
}


[Pool]
public class Monster_Controller : Monster_Controller_Base, IStateMachineOwner
{
    public int monsterEventID;//当前怪物实例ID
    public bool isSkill = false;

    #region 自身组件

    [SerializeField] private NavMeshAgent navMeshAgent;
    private StateMachine stateMachine;
    public NavMeshAgent NavMeshAgent { get => navMeshAgent; private set => navMeshAgent = value; }

    #endregion

    #region View组件

    private Monster_View view;
    public Animator animator { get; private set; }

    #endregion

    #region 数值

    private int maxHp;//最大血量
    private int currentHp; //当前血量
    public int CurrentHP
    {
        get => currentHp;
        set
        {
            currentHp = value;
            // 更新血条
            EventManager.EventTrigger<int, int>("Update_MonsterHP" + monsterEventID, currentHp, maxHp);
        }
    }
    private UI_MonsterHealthHolder uI_MonsterHealthHolder; //血条

    #endregion

    #region 怪物配置

    public Monster_Config monster_config;

    #endregion

    //初始化
    public override void Init(Monster_Config config)
    {
        //当前怪物实例ID
        monsterEventID = this.transform.GetInstanceID();
        //当前怪物配置
        monster_config = config;

        //怪物viw初始
        view = PoolManager.Instance.GetGameObject<Monster_View>(config.MonsterViewPrefab, transform);
        view.transform.localPosition = Vector3.zero;
        view.Init(this, config.Attack, config.Skill_Attack);
        animator = view.Animator;

        //血条血量 血条
        currentHp = config.HP;
        maxHp = config.HP;
        uI_MonsterHealthHolder = ResManager.Load<UI_MonsterHealthHolder>("Monster_HPBar", LVManager.Instance.World_Canvas.transform);
        if (uI_MonsterHealthHolder != null)
        {
            LVManager.Instance.World_Canvas.worldCamera = Camera.main;
            // uI_MonsterHealthHolder.transform.parent = LVManager.Instance.Monster_HPCanvas.transform;
            uI_MonsterHealthHolder.transform.position = view.hp_transform.position;
        }


        // 初始化状态机
        stateMachine = ResManager.Load<StateMachine>();
        stateMachine.Init(this);
        stateMachine.ChangeState<Monster_Idle>((int)MonsterStateType.Idle);
        //注册事件
        RegisterEventListener();

    }

    //注册事件
    protected override void RegisterEventListener()
    {
        EventManager.AddEventListener<int, int>("Update_MonsterHP" + monsterEventID, uI_MonsterHealthHolder.UpdateMonsterHP);
    }

    //取消事件
    protected override void CancelEventListener()
    {
        EventManager.RemoveEventListener<int, int>("Update_MonsterHP" + monsterEventID, uI_MonsterHealthHolder.UpdateMonsterHP);
    }

    //更新
    private void LateUpdate()
    {
        if (uI_MonsterHealthHolder != null)
        {
            uI_MonsterHealthHolder.transform.position = view.hp_transform.position;
            uI_MonsterHealthHolder.transform.forward = Camera.main.transform.forward;
        }
    }

    /// <summary>
    /// 获取巡逻点
    /// </summary>
    public  Vector3 GetPatrolTarget()
    {
        return Monster_Manager.Instance.GetPatrolTarget();
    }


    /// <summary>
    /// 被攻击
    /// </summary>
    public override void GetHit(int damage)
    {
        if (currentHp == 0) return;
        currentHp -= damage;
        if (currentHp <= 0)
        {
            CurrentHP = 0;
            stateMachine.ChangeState<Monster_Die>((int)MonsterStateType.Die);
        }
        else
        {
            CurrentHP = currentHp;

            if (Random.value < 0.2f)
            {
                stateMachine.ChangeState<Monster_GetHit>((int)MonsterStateType.GetHit);
            }
        }
    }

    /// <summary>
    /// 逻辑调用的死亡销毁
    /// </summary>
    public override void Die()
    {
        stateMachine.Destory();
        stateMachine = null;
        view.Destroy();
        view = null;

        uI_MonsterHealthHolder.Destroy();//血条放进对象池

        CancelEventListener();//取消事件 
        this.JKGameObjectPushPool(); //当前怪物放进对象池
    }

    /// <summary>
    /// 因为场景切换导致的销毁，只能把非GameObject上的脚本放进对象池
    /// </summary>
    public override void OnDestroy()
    {
        if (stateMachine != null)
        {
            stateMachine.Destory();
        }
    }
}
