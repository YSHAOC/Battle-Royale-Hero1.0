using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

public class Monster_SkillAttack : Monster_StateBase
{
    // 用于接收怪物View层的动画事件
    private int monsterEventID;
    public override void Init(IStateMachineOwner owner, int stateType, StateMachine stateMachine)
    {
        base.Init(owner, stateType, stateMachine);
        monsterEventID = monster.transform.GetInstanceID();
    }
    public override void Enter()
    {
        //monster.isSkill = false;

        // 修改移动状态
        SetMoveState(false);
        // 播放动画
        PlayerAnimation("SkillAttack");
        // 监听动画的攻击结束
        EventManager.AddEventListener("EndSkillAttack_" + monsterEventID, OnSkillAttackOver);

    }
    public override void Exit()
    {
        EventManager.RemoveEventListener("EndSkillAttack_" + monsterEventID, OnSkillAttackOver);
    }

    /// <summary>
    /// 当技能攻击结束时候执行的逻辑
    /// </summary>
    private void OnSkillAttackOver()
    {
        stateMachine.ChangeState<Monster_Follow>((int)MonsterStateType.Follow);
    }
}
