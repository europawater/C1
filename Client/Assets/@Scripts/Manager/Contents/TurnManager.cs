using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BattleInfo
{
	public BaseAIObject AIObject { get; private set; }
	public List<Skill> CanUseSkillList { get; private set; }

	public BattleInfo(BaseAIObject aiObject, List<Skill> canUseSkillList)
	{
		AIObject = aiObject;
		CanUseSkillList = canUseSkillList;
	}
}

public class TurnManager
{
	private int _currentTurn;
	public int CurrentTurn => _currentTurn;
	private List<BattleInfo> _battleInfoOrderList = new List<BattleInfo>();

	private void SetBattleInfoOrder()
	{
		_battleInfoOrderList.Clear();
		if (Managers.Object.Hero.IsAlive)
		{
			_battleInfoOrderList.Add(new BattleInfo(Managers.Object.Hero, Managers.Object.Hero.GetCanUseSkillList()));
		}

		foreach (BaseMonsterObject monster in Managers.Object.LivingMonsterList)
		{
			_battleInfoOrderList.Add(new BattleInfo(monster, monster.GetCanUseSkillList()));
		}
	}

	public IEnumerator CoTurnBattleHandler(Action onHeroDead, Action onAllMonstersDead)
	{
		_currentTurn = 0;

		if (Managers.Object.Hero.IsAlive)
		{
			Managers.Object.Hero.FullStackAllSkillTurnCount();
		}

		foreach (BaseMonsterObject monster in Managers.Object.LivingMonsterList)
		{
			monster.FullStackAllSkillTurnCount();
		}

		while (true)
		{
			if (!Managers.Object.Hero.IsAlive)
			{
				onHeroDead?.Invoke();
				yield break;
			}

			if (Managers.Object.LivingMonsterList.Count == 0)
			{
				onAllMonstersDead?.Invoke();
				yield break;
			}

			yield return new WaitForSeconds(0.5f);

			_currentTurn++;

			SetBattleInfoOrder();

			foreach (BattleInfo battleInfo in _battleInfoOrderList)
			{
				if (!battleInfo.AIObject.IsAlive)
				{
					continue;
				}

				foreach (Skill skill in battleInfo.CanUseSkillList)
				{
					if (skill.SkillTargetList.Count == 0)
					{
						continue;
					}

					battleInfo.AIObject.UseSkill = skill;
					battleInfo.AIObject.IsAttackComplete = false;
					if(battleInfo.AIObject.UseSkill.SkillData.SkillType == ESkillType.NormalMelee)
					{
						battleInfo.AIObject.SetMeleeAttackMove();
					}
					else
					{
						battleInfo.AIObject.AttackAnimationName = "attack";
						battleInfo.AIObject.AIObjectState = EAIObjectState.Attack;
					}
					
					yield return new WaitUntil(() => battleInfo.AIObject.IsAttackComplete);
					yield return new WaitForSeconds(0.5f);
				}

				battleInfo.AIObject.StackAllSkillTurnCount();
			}
		}
	}
}
