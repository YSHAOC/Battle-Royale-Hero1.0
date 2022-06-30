using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using JKFrame;

public class Orc_Monster_Controller : Monster_Controller
{
    protected override void RegisterEventListener()
    {
        base.RegisterEventListener();
        EventManager.AddEventListener("SkillAttackEffect" + monsterEventID, SkillAttackEffect);
    }

    protected override void CancelEventListener()
    {
        base.CancelEventListener();
        EventManager.RemoveEventListener("SkillAttackEffect" + monsterEventID, SkillAttackEffect);

    }

    private void SkillAttackEffect()
    {
        Vector3 direction =( Player_Controller.Instance.transform.position-this.transform.position).normalized;
        Player_Controller.Instance.GetComponent<NavMeshAgent>().velocity = direction * 7.5f;
        Player_Controller.Instance.PlayerState = PlayerState.Dizzy;
    }
    ////Animation Event
    //public void KickOff() //将攻击目标击飞
    //{
    //    //攻击目标存在 且 在可视范围
    //    if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
    //    {
    //        var targetStats = attackTarget.GetComponent<CharacterStats>();//找到攻击目标数据

    //        Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
    //        //****************direction.Normalize(); 方法一、方法二在上面

    //        targetStats.GetComponent<NavMeshAgent>().isStopped = true; //但被击飞时，停止它继续前进的agent
    //        targetStats.GetComponent<NavMeshAgent>().velocity = direction * kickForce; //反向的速度
    //        targetStats.GetComponent<Animator>().SetTrigger("Dizzy"); //被击飞时眩晕
    //        targetStats.TakeDamage(characterStats, targetStats);//攻击目标遭受伤害
    //    }
    //}


}
