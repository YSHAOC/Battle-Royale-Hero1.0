using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物基类
/// </summary>
public abstract class Monster_Controller_Base : MonoBehaviour
{
    public virtual void Init(Monster_Config config) { }
    public virtual void GetHit(int damage) { }
    public virtual void Die() { }

    public virtual void OnDestroy() { }
    protected virtual void RegisterEventListener() { }
    protected virtual void CancelEventListener() { }
}
