using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using JKFrame;

public enum PlayerState
{
    Normal,
    ReLoad,
    GetHit,
    Dizzy,
    Knockdown,
    Die
}
public class Player_Controller : SingletonMono<Player_Controller>
{
    public bool isCanUpdate = true;

    //CharacterController 相当于 受限的Rigibody
    [SerializeField] private CharacterController characterController;

    //动画
    [SerializeField] private Animator animator;
    private AnimatorStateInfo stateinfo;

    [Header("武器生成位置")]
    [SerializeField] private Transform weponSlot; //武器生成点
    [Header("开火生成位置")]
    [SerializeField] private Transform firePoint_Parent;  //射击点_父节点
    [SerializeField] private Transform firePoint;  //射击点

    private int groundLayerMask;
    //玩家状态
    private PlayerState playerState;

    #region 参数

    //动画
    private RuntimeAnimatorController baseAnimator; //临时存放 Animator动画控制器

    //背包
    private int Bag_Amount;//当前背包格子数

    //攻击
    public WeaponItem_Config weaponItem_Config; //临时存放 weaponItem_Config武器属性配置
    private float moveSpeed; //移动速度
    private int weapon_maxBulletNum;//武器最大子弹数
    private int curr_BulletNum;   //目前子弹数量
    private int curr_allBulletNum;//目前All子弹量
    private float shootInterval; //射击间隔
    private float bulletMovePower;//子弹移动力量
    private int attack;  //攻击力
    public bool canShoot = false; //是否可以射击
    public bool isPauseShoot = true; //是否暂停射击


    //血量
    private int MaxHealth; //最大血量
    private int hp;  //当前血量
    public int HP //血量修改
    {
        get => hp;
        set
        {
            hp = value;
            // 更新血量显示
            EventManager.EventTrigger<int>("UpdateHP", hp);
        }
    }


    //防御
    private DefenceItem_Config Armor_defenceItem_Config; //临时存放 Armor_defenceItem_Config防御属性配置

    //头盔
    private DefenceItem_Config Helmet_defenceItem_Config; //临时存放 Armor_defenceItem_Config防御属性配置



    private float Defence_probability; //防御率（百分比）
                                       // private int MaxDefence; //最大防御值
    private int currDefence;//当前的防御值 （防御百分比 * 最大血量）
    public int CurrDefence //防御值修改
    {
        get => currDefence;
        set
        {
            currDefence = value;
            // 更新防御值显示
            EventManager.EventTrigger<int>("UpdateDF", currDefence);
        }
    }
    #endregion



    #region 初始化
    public void Init(Player_Config config)
    {
        isCanUpdate = true;
        //临时存放的 Animator 初始化
        baseAnimator = animator.runtimeAnimatorController;
        groundLayerMask = LayerMask.GetMask("Ground");

        //血量 防御力赋值
        HP = config.HP; //血量
        MaxHealth = HP; //最大血量
        Defence_probability = config.Defence;//防御率
        CurrDefence = (int)(Defence_probability * MaxHealth);//防御值
        moveSpeed = config.MoveSpeed;//移动速度


        //武器属性赋值
        weapon_maxBulletNum = config.MaxBulletNum;//武器最大子弹数
        curr_BulletNum = config.MaxBulletNum; //目前子弹数量
        curr_allBulletNum = 0; //目前All子弹量
        shootInterval = config.ShootInterval;//射击间隔
        bulletMovePower = config.BulletMovePower; //子弹移动力量
        attack = config.Attack;  //攻击力



        #region 角色装备廊State初始化属性

        ////----------------（方法一）
        //初始化人物 武器
        if (Inventory_Manager.Instance.equipment_Config.GetState_Item_Config(ItemType.Weapon) != null)
        {
            // weaponItem_Config = Inventory_Manager.Instance.equipment_Config.GetState_Item_Config(ItemType.Weapon).weapon_Config;

            ChangeWeapon(Inventory_Manager.Instance.equipment_Config.GetState_Item_Config(ItemType.Weapon));
            canShoot = true;
        }
        //初始化人物 头盔
        if (Inventory_Manager.Instance.equipment_Config.GetState_Item_Config(ItemType.Helmet) != null)
        {
            ChangeHelmet(Inventory_Manager.Instance.equipment_Config.GetState_Item_Config(ItemType.Helmet));
        }
        //初始化人物 防御甲
        if (Inventory_Manager.Instance.equipment_Config.GetState_Item_Config(ItemType.Armor) != null)
        {
            ChangeArmor(Inventory_Manager.Instance.equipment_Config.GetState_Item_Config(ItemType.Armor));
        }
        //初始化人物 背包装备
        if (Inventory_Manager.Instance.equipment_Config.GetState_Item_Config(ItemType.Bag_Equip) != null)
        {
            ChangeBagEquip(Inventory_Manager.Instance.equipment_Config.GetState_Item_Config(ItemType.Bag_Equip));
        }

        #region 方法二
        //////----------------（方法二）
        ////人物属性-武器配置初始化
        //if (weaponItem_Config != null)  //当人物 （有） 武器属性配置
        //{
        //    Refresh_AttackAttributes();  //更新人物攻击属性
        //}
        //else //(没有）
        //{

        //    maxBulletNum = config.MaxBulletNum; //最大子弹数量
        //    currBulletNum = maxBulletNum; //初始化目前子弹量
        //    shootInterval = config.ShootInterval;//射击间隔
        //    bulletMovePower = config.BulletMovePower; //子弹移动力量
        //    attack = config.Attack;  //攻击力
        //}
        #endregion

        EventManager.EventTrigger<int, int>("UpdateBullet", curr_BulletNum, curr_allBulletNum);
        UpdateBulletShow();
        #endregion

    }

    public void UpdateBulletShow() //更新背包与界面 对应子弹的显示
    {
        if (weaponItem_Config != null && Inventory_Manager.Instance.bag_Config.GetState_Item_Config(ItemType.Weapon_Bullet, weaponItem_Config.Bullet_Type) != null)
        {
            curr_allBulletNum = Inventory_Manager.Instance.bag_Config.GetState_Item_Config(ItemType.Weapon_Bullet, weaponItem_Config.Bullet_Type).amount;
            EventManager.EventTrigger<int, int>("UpdateBullet", curr_BulletNum, curr_allBulletNum);
        }
        else // 这里主要是Inventory_Manager.Instance.bag_Config.GetState_Item_Config(ItemType.Weapon_Bullet, weaponItem_Config.Bullet_Type)为null
        {
            curr_allBulletNum = 0;
            EventManager.EventTrigger<int, int>("UpdateBullet", curr_BulletNum, curr_allBulletNum);
        }
    }
    #endregion



    #region 状态帧更新

    public PlayerState PlayerState
    {
        get => playerState;
        set
        {
            playerState = value;
            switch (playerState)
            {
                case PlayerState.ReLoad:
                    StartCoroutine(ReLoad());
                    break;

                case PlayerState.GetHit:
                    // 重置上一次受伤带来的效果
                    StopCoroutine(DoGetHit());
                    animator.SetBool("GetHit", false);

                    // 开始这一次受伤带来的效果
                    animator.SetBool("GetHit", true);
                    StartCoroutine(DoGetHit());
                    break;

                case PlayerState.Dizzy:
                    StartCoroutine(DoGetDizzy());
                    break;

                case PlayerState.Knockdown:
                    StartCoroutine(DoGetKnockdown());
                    break;

                case PlayerState.Die:
                    EventManager.EventTrigger("GameOver");
                    animator.SetTrigger("Die");
                    break;
            }
        }
    }

    private void Update()
    {
        if (Time.timeScale != 0)
        {
            StateOnUpdate();
        }


    }
    private void StateOnUpdate()
    {
        switch (PlayerState)
        {
            case PlayerState.Normal:
                Move();
                if (isPauseShoot == true)
                {
                    Shoot();
                    if (curr_BulletNum < weapon_maxBulletNum && Input.GetKeyDown(KeyCode.R) && curr_allBulletNum > 0)
                    {
                        PlayerState = PlayerState.ReLoad;
                    }
                }

                break;
            case PlayerState.ReLoad:
                Move();
                break;
            case PlayerState.Knockdown:
                stateinfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateinfo.IsName("GetUp") && stateinfo.normalizedTime >= 1.0f)
                {
                    animator.SetBool("IsKnockdowny", false);
                    this.GetComponent<NavMeshAgent>().isStopped = false;
                    PlayerState = PlayerState.Normal;
                }
                break;
        }
    }

    #endregion

    #region 移动

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(h, -5, v);
        characterController.Move(moveDir * moveSpeed * Time.deltaTime);

        Ray ray = Camera_Controller.Instance.camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000, groundLayerMask))
        {
            if (hitInfo.point.z < transform.position.z)
            {
                v *= -1;
                h *= -1;
            }
            Vector3 dir = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z) - transform.position;
            Quaternion targetQuaternion = Quaternion.FromToRotation(Vector3.forward, dir);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetQuaternion, Time.deltaTime * 20f);
        }

        animator.SetFloat("MoveX", h);
        animator.SetFloat("MoveY", v);
    }

    #endregion

    #region 射击 / 更换子弹
    private void Shoot()
    {

        if (canShoot && curr_BulletNum > 0 && Input.GetMouseButton(0))
        {
            StartCoroutine(DoShoot());
        }
        else
        {
            animator.SetBool("Shoot", false);
        }

    }

    private IEnumerator DoShoot()
    {
        curr_BulletNum -= 1;
        // 修改UI
        EventManager.EventTrigger<int, int>("UpdateBullet", curr_BulletNum, curr_allBulletNum);

        animator.SetBool("Shoot", true);
        AudioManager.Instance.PlayOnShot("Audio/Shoot/laser_01", transform.position);
        // 生成子弹
        Bullet bullet = ResManager.Load<Bullet>("Bullet", LVManager.Instance.TempObjRoot);
        bullet.transform.position = firePoint.position;
        bullet.Init(firePoint.forward, bulletMovePower, attack);

        canShoot = false;

        yield return new WaitForSeconds(shootInterval);
        canShoot = true;
        // 子弹打完，需要换弹
        if (curr_BulletNum == 0 && curr_allBulletNum > 0)
        {
            PlayerState = PlayerState.ReLoad;
        }
    }

    private IEnumerator ReLoad()
    {
        animator.SetBool("ReLoad", true);
        AudioManager.Instance.PlayOnShot("Audio/Shoot/ReLoad", this);
        yield return new WaitForSeconds(1.9f);
        animator.SetBool("ReLoad", false);
        PlayerState = PlayerState.Normal;


        int allBulletNum;
        if ((curr_allBulletNum - (weapon_maxBulletNum - curr_BulletNum) < 0))
        {
            allBulletNum = 0;

        }
        else
        {
            allBulletNum = curr_allBulletNum - (weapon_maxBulletNum - curr_BulletNum);
        }

        curr_allBulletNum = allBulletNum;
        curr_BulletNum = weapon_maxBulletNum;

        switch (weaponItem_Config.Bullet_Type)
        {
            case "5.56毫米":
                if (Inventory_Manager.Instance.bag_Config.GetState_Item_Config(ItemType.Weapon_Bullet, "5.56毫米") != null)
                {
                    Inventory_Manager.Instance.bag_Config.GetState_Item_Config(ItemType.Weapon_Bullet, "5.56毫米").amount = allBulletNum;
                    EventManager.EventTrigger<int, int>("UpdateBullet", curr_BulletNum, allBulletNum);
                    if (Inventory_Manager.Instance.bag_Container != null) Inventory_Manager.Instance.bag_Container.RefreshUI1(); //这里顺便更新一下背包UI
                }
                break;
            case "7.62毫米":
                if (Inventory_Manager.Instance.bag_Config.GetState_Item_Config(ItemType.Weapon_Bullet, "7.62毫米") != null)
                {
                    Inventory_Manager.Instance.bag_Config.GetState_Item_Config(ItemType.Weapon_Bullet, "7.62毫米").amount = allBulletNum;
                    EventManager.EventTrigger<int, int>("UpdateBullet", curr_BulletNum, allBulletNum);
                    if (Inventory_Manager.Instance.bag_Container != null) Inventory_Manager.Instance.bag_Container.RefreshUI1(); //这里顺便更新一下背包UI
                }
                break;

        }

    }

    #endregion

    #region 受到伤害GetHit() / 受伤害效果  || 加血ApplyHealth()
    //受伤害
    public void GetHit(int damage, Monster_View monster_View)
    {
        //防御值 抵 伤害
        GetHit_CurrDefence(damage, monster_View);

        //血量 抵 伤害
        if (hp == 0 || CurrDefence > 0) return; //如果血量为0 ，防御值>0 ，就不进行减血
        hp -= damage;
        if (hp <= 0)
        {
            HP = 0;
            PlayerState = PlayerState.Die;
        }
        else
        {
            HP = hp;
            PlayerState = PlayerState.GetHit;
            monster_View.canHit = false;
            monster_View.canSkillHit = false;
        }
    }
    //目前防御值受伤害
    public void GetHit_CurrDefence(int damage, Monster_View monster_View)
    {
        //防御值 抵 伤害
        if (CurrDefence > 0 && CurrDefence - damage > 0)
        {
            CurrDefence -= damage;

            PlayerState = PlayerState.GetHit;
            monster_View.canHit = false;
            monster_View.canSkillHit = false;
            return;
        }
        else
        {
            CurrDefence = 0;
            return;
        }
    }


    //加血
    public void ApplyHealth(int amount)
    {
        if (hp == 0) return;
        if (hp + amount <= MaxHealth)
            HP += amount;
        else
            HP = MaxHealth;

    }

    private IEnumerator DoGetHit() //受伤害
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("GetHit", false);
        if (PlayerState == PlayerState.GetHit)
        {
            PlayerState = PlayerState.Normal;
        }
    }

    private IEnumerator DoGetDizzy() //眩晕
    {
        animator.SetBool("IsDizzy", true);
        yield return new WaitForSeconds(2.0f);
        animator.SetBool("IsDizzy", false);
        PlayerState = PlayerState.Normal;
    }

    private IEnumerator DoGetKnockdown() //击倒
    {
        animator.SetBool("IsKnockdown", true);
        this.GetComponent<NavMeshAgent>().isStopped = false;
        yield return new WaitForSeconds(3.0f);
    }

    #endregion

    #region 更换武器/修改人物属性

    //切换武器
    public void ChangeWeapon(Item_Config item_Config)
    {
        UnEquipWeapon();
        EquipWeapon(item_Config);

    }


    //卸下武器
    public void UnEquipWeapon()
    {

        if (weponSlot.transform.childCount != 0)
        {
            for (int i = 0; i < weponSlot.transform.childCount; i++)
            {
                Destroy(weponSlot.transform.GetChild(i).gameObject);

            }
        }
        weaponItem_Config = null;

        //切换动画 （更改为最基础 原来的动画）
        animator.runtimeAnimatorController = baseAnimator;

    }


    //装备武器
    public void EquipWeapon(Item_Config Weapon_item_Config)
    {

        if (Weapon_item_Config.weaponPrefab == null) return;

        //实例化武器
        Instantiate(Weapon_item_Config.weaponPrefab, weponSlot);
        //武器开火点
        firePoint = firePoint_Parent.transform.Find(Weapon_item_Config.weaponPrefab.name + "_FirePoint").transform;
        //更新属性
        weaponItem_Config = Weapon_item_Config.weapon_Config;
        Refresh_AttackAttributes(weaponItem_Config);
        //切换画animator
        animator.runtimeAnimatorController = Weapon_item_Config.weapon_animator;


        //更新角色信息 显示
        EventManager.EventTrigger<WeaponItem_Config>("Resfesh_CharacterInfo", Weapon_item_Config.weapon_Config);

    }



    //装备武器 更新 攻击属性
    public void Refresh_AttackAttributes(WeaponItem_Config weapon_item_Config)
    {
        if (weaponItem_Config == null) return;

        if (isCanUpdate)
        {
            weapon_maxBulletNum = weaponItem_Config.MaxBulletNum; //武器最大子弹量
            curr_BulletNum = weapon_maxBulletNum; //当前子弹量
            EventManager.EventTrigger<int, int>("UpdateBullet", curr_BulletNum, curr_allBulletNum);
        }
        UpdateBulletShow();

        shootInterval = weaponItem_Config.ShootInterval; //射击频率
        bulletMovePower = weaponItem_Config.BulletMovePower; //子弹移动速度
        attack = weaponItem_Config.Attack; //攻击力

    }


    #endregion

    #region 更换--头盔装备--/修改人物防御属性

    //切换头盔
    public void ChangeHelmet(Item_Config item_Config)
    {
        if (isCanUpdate)
        {
            UnEquipHelmet();
            EquipHelmet(item_Config.helmetItem_Config);
        }
        else
        {
            CurrDefence = currDefence;
        }

    }

    //卸下头盔
    public void UnEquipHelmet()
    {
        //当前临时配置不能为 null
        if (Helmet_defenceItem_Config != null)
        {
            float HelmetDefence = Helmet_defenceItem_Config.Defence_Value;
            //减去后防御值 必须>= 0
            if (CurrDefence - HelmetDefence * MaxHealth >= 0)
            {
                CurrDefence = currDefence - (int)(HelmetDefence * MaxHealth);  //当前防御值 - 新增防御值  ------防御值 =（防御百分比 * 最大血量）
            }
            else // 小于0 值设置目前的防御值为 0
            {
                CurrDefence = 0;
            }
            Helmet_defenceItem_Config = null; //将临时存放 Armor_defenceItem_Config防御属性配置 置null
        }


    }

    //装备头盔
    public void EquipHelmet(DefenceItem_Config defenceItem_Config)
    {
        Helmet_defenceItem_Config = defenceItem_Config; ///将临时存放 Armor_defenceItem_Config防御属性配置 赋值
        //更新属性
        Refresh_HelmetAttributes(defenceItem_Config);

    }


    //装备头盔 更新 防御属性
    public void Refresh_HelmetAttributes(DefenceItem_Config defenceItem_Config)
    {

        float defence_probability = defenceItem_Config.Defence_Value;//防御率
        if ((currDefence + (int)(defence_probability * MaxHealth)) > 100)
        {
            CurrDefence = 100;
        }
        else
        {
            CurrDefence = currDefence + (int)(defence_probability * MaxHealth);  //当前防御值 + 新增防御值  ------防御值 =（防御百分比 * 最大血量）
        }

    }

    #endregion

    #region 更换防甲装备/修改人物防御属性

    //切换防御甲
    public void ChangeArmor(Item_Config item_Config)
    {
        if (isCanUpdate)
        {
            UnEquipArmor();
            EquipArmor(item_Config.defenceItem_Config);
        }
        else
        {
            CurrDefence = currDefence;
        }

    }

    //卸下防御甲
    public void UnEquipArmor()
    {
        //当前临时配置不能为 null
        if (Armor_defenceItem_Config != null)
        {
            float ArmorDefence = Armor_defenceItem_Config.Defence_Value;
            //减去后防御值 必须>= 0
            if (CurrDefence - ArmorDefence * MaxHealth >= 0)
            {
                CurrDefence = currDefence - (int)(ArmorDefence * MaxHealth);  //当前防御值 - 新增防御值  ------防御值 =（防御百分比 * 最大血量）
            }
            else // 小于0 值设置目前的防御值为 0
            {
                CurrDefence = 0;
            }

            Armor_defenceItem_Config = null; //将临时存放 Armor_defenceItem_Config防御属性配置 置null
        }


    }


    //装备防御甲
    public void EquipArmor(DefenceItem_Config defenceItem_Config)
    {
        Armor_defenceItem_Config = defenceItem_Config; ///将临时存放 Armor_defenceItem_Config防御属性配置 赋值
        //更新属性
        Refresh_ArmorAttributes(defenceItem_Config);

        //更新角色信息 显示
        //EventManager.EventTrigger<WeaponItem_Config>("Resfesh_CharacterInfo", Weapon_item_Config.weapon_Config);
    }


    //装备防御甲 更新 防御属性
    public void Refresh_ArmorAttributes(DefenceItem_Config defenceItem_Config)
    {

        float defence_probability = defenceItem_Config.Defence_Value;//防御率
        if ((currDefence + (int)(defence_probability * MaxHealth)) > 100)
        {
            CurrDefence = 100;
        }
        else
        {
            CurrDefence = currDefence + (int)(defence_probability * MaxHealth);  //当前防御值 + 新增防御值  ------防御值 =（防御百分比 * 最大血量）
        }

    }

    #endregion


    #region 更换背包装备/修改人物背包属性

    //切换背包装备
    public void ChangeBagEquip(Item_Config item_Config)
    {
        UnEquipBagEquip();
        EquipBagEquip(item_Config.bagEquipItem_Config);

    }

    //卸下背包装备
    public void UnEquipBagEquip()
    {
        //更新角色信息 显示
        EventManager.EventTrigger<int>("Resfesh_BagAmount", 20);
        EventManager.EventTrigger<int>("UpdateBagAmount", 20);

    }


    //装备背包装备
    public void EquipBagEquip(BagEquipItem_Config bagEquipItem_Config)
    {
        EventManager.EventTrigger<int>("Resfesh_BagAmount", bagEquipItem_Config.Bag_Amount);
        EventManager.EventTrigger<int>("UpdateBagAmount", bagEquipItem_Config.Bag_Amount);
    }


    #endregion
}
