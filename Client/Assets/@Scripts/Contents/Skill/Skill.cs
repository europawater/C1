using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class Skill
{
	public SkillData SkillData { get; private set; }
	public SkillInfo SkillInfo { get; private set; }

	private BaseAIObject _owner;

	public List<BaseAIObject> SkillTargetList
	{
		get
		{
			List<BaseAIObject> targetList = new List<BaseAIObject>();

			if (_owner.GameObjectType == EGameObjectType.Hero)
			{
				switch (SkillData.SkillType)
				{
					case ESkillType.NormalMelee:
					case ESkillType.NormalRange:
					case ESkillType.SkillSingle:
						BaseAIObject target = Managers.Object.LivingMonsterList.FirstOrDefault();
						if (target != null)
						{
							targetList.Add(target);
						}
						break;
					case ESkillType.SkillMulti:
						targetList = Managers.Object.LivingMonsterList.ToList<BaseAIObject>();
						break;
				}
			}
			else if (_owner.GameObjectType == EGameObjectType.Monster)
			{
				targetList.Add(Managers.Object.Hero);
			}

			return targetList;
		}
	}

	public Skill(BaseAIObject owner, SkillInfo skillInfo)
	{
		SetInfo(owner, skillInfo);
	}

	public void SetInfo(BaseAIObject owner, SkillInfo skillInfo)
	{
		_owner = owner;

		if (_owner.GameObjectType == EGameObjectType.Hero)
		{
			SkillData = Managers.Backend.Chart.SkillChart.HeroSkillDataDict[skillInfo.TemplateID];
		}
		else if (_owner.GameObjectType == EGameObjectType.Monster)
		{
			SkillData = Managers.Backend.Chart.SkillChart.MonsterSkillDataDict[skillInfo.TemplateID];
		}

		SkillInfo = skillInfo;
	}

	public void Reset()
	{
		_owner = null;
		SkillData = null;
		SkillInfo = null;
	}

	#region Battle

	private int _stackedTurnCount = 0;
	public int StackedTurnCount
	{
		get { return _stackedTurnCount; }
		set
		{
			if (_stackedTurnCount == value || NeedTurnCount < value)
			{
				return;
			}

			_stackedTurnCount = value;

			Managers.Event.TriggerEvent(EEventType.OnSkillStackedTurnCountChanged);
		}
	}

	public int NeedTurnCount
	{
		get { return SkillData.NeedTurnCount; }
	}

	public bool IsReadyToUse
	{
		get { return SkillInfo.IsEquipped && StackedTurnCount >= NeedTurnCount; }
	}

	private Coroutine _coSkillHitHandler = null;

	public void UseSkill()
	{
		if (IsReadyToUse == false)
		{
			return;
		}

		StackedTurnCount = 0;

		if (SkillData.SkillType == ESkillType.NormalMelee)
		{
			Managers.Object.SpawnSkillEffect(_owner.transform.position, SkillData.SkillEffectPrefabKey, SkillData.LifeTime);
		}
		else if (SkillData.SkillType == ESkillType.NormalRange)
		{ 
			Vector2 startPosition = _owner.transform.position + Vector3.up;
			Vector2 endPosition = SkillTargetList.FirstOrDefault().transform.position + Vector3.up;
			float lifeTime = SkillData.HitDelay == 0 ? 0.3f : SkillData.HitDelay;

			Managers.Object.SpawnProjectile(SkillData.SkillEffectPrefabKey, startPosition, endPosition, lifeTime);
		}
		else
		{
			Managers.Object.SpawnSkillCube(SkillTargetList.FirstOrDefault().transform.position + new Vector3(0.0f, 6.0f, 0.0f));
			Managers.Object.SpawnSkillEffect(SkillTargetList.FirstOrDefault().transform.position, SkillData.SkillEffectPrefabKey, SkillData.LifeTime);
		}

		if (_coSkillHitHandler != null)
		{
			Managers.Instance.StopCoroutine(_coSkillHitHandler);
			_coSkillHitHandler = null;
		}

		_coSkillHitHandler = Managers.Instance.StartCoroutine(CoSkillHitHandler());
	}

	private IEnumerator CoSkillHitHandler()
	{
		// 피격 타이밍 까지 대기합니다.
		yield return new WaitForSeconds(SkillData.HitStartTime);

		for (int hitCount = 0; hitCount < SkillData.HitCount; hitCount++)
		{
			foreach (BaseAIObject target in SkillTargetList)
			{
				if (!target.IsAlive)
				{
					continue;
				}

				int damage = 0;
				float criticalRate = _owner.Status.CriticalRate;
				float randomValue = Util.GetRadomfloat(0.0f, 100.0f);
				bool isCritical = randomValue <= criticalRate;

				if (SkillData.SkillType == ESkillType.NormalMelee || SkillData.SkillType == ESkillType.NormalRange)
				{
					damage = Mathf.FloorToInt(CalculateNormalDamage(target, isCritical));
				}
				else
				{
					damage = Mathf.FloorToInt(CalculateSkillDamage(target, isCritical));
				}

				target.OnDamage(_owner, damage, isCritical, SkillData.HitEffectPrefabKey);
			}

			yield return new WaitForSeconds(SkillData.HitDelay);
		}

		yield return null;
	}

	private float CalculateNormalDamage(BaseAIObject target, bool isCritical)
	{
		float bossExtraValue = 0.0f;
		if (target.GameObjectType == EGameObjectType.Monster)
		{
			BaseMonsterObject monster = (BaseMonsterObject)target;
			if(monster.MonsterType == EMonsterType.BossMonster)
			{
				bossExtraValue = _owner.Status.BossExtraValue;
			}
		}

		float damage = _owner.Status.Attack + (_owner.Status.Attack * bossExtraValue);
		if(isCritical)
		{
			float criticalvalue = _owner.Status.CriticalValue;
			damage = damage + (damage * criticalvalue);
		}

		return damage;
	}

	private float CalculateSkillDamage(BaseAIObject target, bool isCritical)
	{
		float damage = _owner.Status.Attack * SkillData.AttackValue;
		if (isCritical)
		{
			float criticalvalue = _owner.Status.SkillCriticalValue;
			damage = damage + (damage * criticalvalue);
		}

		return damage;
	}

	#endregion
}
