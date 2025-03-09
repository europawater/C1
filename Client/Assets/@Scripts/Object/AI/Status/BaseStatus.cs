using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

public abstract class BaseStatus : MonoBehaviour
{
	public int HP { get; protected set; }
    
    public virtual int Attack { get; protected set; }
	public virtual int Defense { get; protected set; }
	public virtual int MaxHP { get; protected set; }
	public virtual float CriticalValue { get; protected set; }
	public virtual float CriticalRate { get; protected set; }
	public virtual float SkillDamageValue { get; protected set; }
	public virtual float SkillCriticalValue { get; protected set; }
	public virtual float DodgeRate { get; protected set; }
	public virtual float ComboAttackRate { get; protected set; }
	public virtual float CounterAttackRate { get; protected set; }
	public virtual float BossExtraValue { get; protected set; }


	protected BaseAIObject _owner;
    private UI_BattleBarWorldSpace _battleBarUI;

    private void Awake()
    {
		_battleBarUI = GetComponentInChildren<UI_BattleBarWorldSpace>();
	}

	public virtual void SetInfo(BaseAIObject owner, int attack, int defense, int maxHP, float criticalValue, float criticalRate, float skillDamageValue, float skillCriticalValue, float dogeRate, float comboAttackRate, float counterAttackRate, float bossExtraValue = 0.0f)
	{
		_owner = owner;

		Attack = attack;
		Defense = defense;
		MaxHP = maxHP;
		CriticalValue = criticalValue;
		CriticalRate = criticalRate;
		SkillDamageValue = skillDamageValue;
		DodgeRate = skillCriticalValue;
		ComboAttackRate = dogeRate;
		CounterAttackRate = comboAttackRate;
		CounterAttackRate = counterAttackRate;
		BossExtraValue = bossExtraValue;

		HP = MaxHP;

		UpdateHpText();
	}

    public void TakeDamage(int damage)
    {
        _owner.AIObjectState = EAIObjectState.Hit;

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            _owner.OnDead();
        }

        UpdateHpText();
    }

    protected void UpdateHpText()
    {
		_battleBarUI.SetInfo(HP, MaxHP);
    }

    public void ResetHp()
    {
        HP = MaxHP;

		UpdateHpText();
	}
}
