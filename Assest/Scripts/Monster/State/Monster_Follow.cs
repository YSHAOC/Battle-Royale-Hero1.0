using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Monster_Follow : Monster_StateBase
{
    private float Nav_speed;//移动速度
    public override void Enter()
    {
        // 修改移动状态
        SetMoveState(true);
        Nav_speed = navMeshAgent.speed;
        navMeshAgent.speed = Nav_speed * 1.5f;
        // 播放动画
        PlayerAnimation("Run");
    }
    public override void Exit()
    {
        navMeshAgent.speed = Nav_speed; //移动速度还原
    }

    public override void LateUpdate()
    {
        // 如果我距离玩家非常进，应该去攻击玩家
        if (Vector3.Distance(monster.transform.position, player.transform.position) < 1.6f)
        {
            bool isSkill = Random.value < monster.monster_config.Skill_Probability;
            //技能攻击
            if (isSkill)
            {
                stateMachine.ChangeState<Monster_SkillAttack>((int)MonsterStateType.SkillAttack);
                return;
            }

            stateMachine.ChangeState<Monster_Attack>((int)MonsterStateType.Attack);
            return;
        }

        //检测是否发现敌人 或 继续追击
        if(!CheckStopFollowAndChangeState())
        {
            navMeshAgent.SetDestination(player.transform.position);
        }

    }
}