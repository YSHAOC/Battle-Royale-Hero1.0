using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

public class Monster_View : MonoBehaviour
{
    public Transform hp_transform; //血条位置

    private Monster_Controller Monster_Controller; //本身Monster_Controller

    #region 动画
    [SerializeField] private Animator animator;
    public Animator Animator { get => animator; private set => animator = value; }
    #endregion

    #region 普通攻击 技能攻击
    [SerializeField] private Transform weaponCollider; //普通攻击Collider
    [SerializeField] private Transform SkillweaponCollider;//技能攻击Collider

    public bool canHit; //是否可普通攻击
    public bool canSkillHit; //是否可技能攻击
    private int attackValue; //普通攻击值
    private int skillAttackValue;//技能攻击值
    #endregion

    private void Start()
    {
        weaponCollider.OnTriggerEnter(OnWeaponColliderTriggerEnter);//普通攻击TriggerCollider
        SkillweaponCollider.OnTriggerEnter(OnSkillWeaponColliderTriggerEnter);//技能攻击TriggerCollider
    }


    public void Init(Monster_Controller monster_Controller, int attackValue, int skillAttackValue)
    {
        Monster_Controller = monster_Controller;//本身Monster_Controller赋值
        this.attackValue = attackValue; //普通攻击
        this.skillAttackValue = skillAttackValue; //技能攻击
    }

    private void OnWeaponColliderTriggerEnter(Collider other, params object[] args)
    {

        if (other.gameObject.tag == "Player")
        {
            if (!canHit) return;
            Player_Controller.Instance.GetHit(attackValue, this);
            Debug.Log("普通攻击：" + attackValue);
            AudioManager.Instance.PlayOnShot("Audio/Monster/拳头击中", transform);
        }
    }
    private void OnSkillWeaponColliderTriggerEnter(Collider other, params object[] args)
    {

        if (other.gameObject.tag == "Player")
        {
            if (!canSkillHit) return;
            Player_Controller.Instance.GetHit(skillAttackValue, this);
            EventManager.EventTrigger("SkillAttackEffect" + Monster_Controller.monsterEventID);
            Debug.Log("技能攻击：" + skillAttackValue);
            AudioManager.Instance.PlayOnShot("Audio/Monster/拳头击中", transform);
        }
    }

    /// <summary>
    /// 放进对象池
    /// </summary>
    public void Destroy()
    {
        this.JKGameObjectPushPool();
    }

    #region 动画事件


    #region 脚步声
    private void Footstep()
    {
        AudioManager.Instance.PlayOnShot("Audio/Monster/走路", transform, 0.1f);
    }
    #endregion


    #region 普通攻击
    private void StartHit()
    {
        canHit = true;
        weaponCollider.gameObject.SetActive(true);

    }

    private void StopHit()
    {
        canHit = false;
        weaponCollider.gameObject.SetActive(false);
    }

    private void EndAttack()
    {
        weaponCollider.gameObject.SetActive(false);
        EventManager.EventTrigger("EndAttack_" + transform.parent.GetInstanceID());
    }
    #endregion

    #region 技能攻击
    private void StartSkillHit()
    {
        canSkillHit = true;
        SkillweaponCollider.gameObject.SetActive(true);

    }

    private void StopSkillHit()
    {
        canSkillHit = false;
        SkillweaponCollider.gameObject.SetActive(false);
    }

    private void EndSkillAttack()
    {
        SkillweaponCollider.gameObject.SetActive(false);
        EventManager.EventTrigger("EndSkillAttack_" + transform.parent.GetInstanceID());
    }
    #endregion


    #region 怪物自己得到攻击
    private void EndGetHit()
    {
        EventManager.EventTrigger("EndGetHit_" + transform.parent.GetInstanceID());
    }
    #endregion

    #endregion
}
