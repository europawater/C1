using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public abstract class BaseMonsterObject : BaseAIObject
{
    public EMonsterType MonsterType { get; protected set; }

    protected override void Init()
    {
        GameObjectType = EGameObjectType.Monster;

        base.Init();
    }

    public void SetInfo(int templateID, int difficultyLevel)
    {
		if (!Managers.Backend.Chart.MonsterChart.DataDict.TryGetValue(templateID, out MonsterData monsterData))
		{
			return;
		}

		if(!Managers.Backend.Chart.MonsterLevelScalingChart.DataDict.TryGetValue(templateID, out MonsterLevelScalingData monsterLevelScalingData))
		{
			return;
		}

		SkeletonAnimation.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(monsterData.SkeletondataKey);
		SkeletonAnimation.Initialize(true);

		int MaxHP = monsterData.MaxHP + (monsterLevelScalingData.IncreaseMaxHP * difficultyLevel);
		int Attack = monsterData.Attack + (monsterLevelScalingData.IncreaseAttack * difficultyLevel);
		int Defense = monsterData.Defense + (monsterLevelScalingData.IncreaseDefense * difficultyLevel);
		float CriticalValue = monsterData.CriticalValue + (monsterLevelScalingData.IncreaseCriticalValue * difficultyLevel);
		float CriticalRate = monsterData.CriticalRate + (monsterLevelScalingData.IncreaseCriticalRate * difficultyLevel);
		float SkillDamageValue = monsterData.SkillDamageValue + (monsterLevelScalingData.IncreaseSkillDamageValue * difficultyLevel);
		float SkillCriticalValue = monsterData.CriticalValue + (monsterLevelScalingData.IncreaseCriticalValue * difficultyLevel);
		float DogeRate = monsterData.DodgeRate + (monsterLevelScalingData.IncreaseDodgeRate * difficultyLevel);
		float ComboAttackRate = monsterData.ComboAttackRate + (monsterLevelScalingData.IncreaseComboAttackRate * difficultyLevel);
		float CounterAttackRate = monsterData.CounterAttackRate + (monsterLevelScalingData.IncreaseCounterAttackRate * difficultyLevel);

		Status.SetInfo(this, Attack, Defense, MaxHP, CriticalValue, CriticalRate, SkillDamageValue, SkillCriticalValue, DogeRate, ComboAttackRate, CounterAttackRate);

		SkillInfo defaultSkillInfo = new SkillInfo(monsterData.DefaultSkill, EOwningState.Owned, 1, true);
		DefaultSkill = new Skill(this, defaultSkillInfo);
		List<SkillInfo> skillInfoList = new List<SkillInfo>();
		foreach (int skillID in monsterData.SkillList)
		{
			SkillInfo skillInfo = new SkillInfo(skillID, EOwningState.Owned, 1, true);
			skillInfoList.Add(skillInfo);
		}
		foreach(SkillInfo skillInfo in skillInfoList)
		{
			Skill skill = new Skill(this, skillInfo);
			SkillList.Add(skill);
		}

		base.SetInfo(templateID);
	}

	#region Battle

	public override void OnDead()
	{
		GameScene gameScene = Managers.Scene.CurrentScene as GameScene;
		int goldReward = Util.GetRandomInt(gameScene.StageData.MinGoldReward, gameScene.StageData.MaxGoldReward);
		Managers.Backend.GameData.Currency.AddAmount(ECurrency.Gold, goldReward);

		UI_GoldRewardText goldRewardText = Managers.UI.MakeSubItem<UI_GoldRewardText>(transform, "UI_GoldRewardText");
		goldRewardText.SetInfo(goldReward);

		// Bakcend Kill Count ¡ı∞°
		Managers.Backend.GameData.Player.AddMonsterKillCount(1);

		base.OnDead();
	}

	#endregion

	#region Animation Event

	public override void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e)
    {
        base.OnAnimEventHandler(trackEntry, e);
    }

	public override void OnAnimCompleteHandler(TrackEntry trackEntry)
	{
		base.OnAnimCompleteHandler(trackEntry);

		if (AIObjectState == EAIObjectState.Dead)
		{
			Managers.Object.RemoveMonster(this);
		}
	}

	#endregion
}
