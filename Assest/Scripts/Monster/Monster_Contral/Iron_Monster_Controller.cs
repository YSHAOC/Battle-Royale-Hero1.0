using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using JKFrame;

public class Iron_Monster_Controller : Monster_Controller
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
       // Player_Controller.Instance.PlayerState = PlayerState.Knockdown;
        Vector3 direction = (Player_Controller.Instance.transform.position - this.transform.position).normalized;
        Player_Controller.Instance.GetComponent<NavMeshAgent>().velocity = direction * 7.5f;
    }
}
