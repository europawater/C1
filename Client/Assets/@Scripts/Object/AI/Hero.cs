using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class Hero : BaseAIObject
{
	public override Skill DefaultSkill => Managers.Game.DefaultSkill;
	public override List<Skill> SkillList => Managers.Game.EquippedSkillSlotDict.Values.ToList();

	protected override void Init()
    {
        GameObjectType = EGameObjectType.Hero;

        base.Init();
    }

    public override void SetInfo(int templateID)
    {
        if (!Managers.Backend.Chart.HeroChart.DataDict.TryGetValue(templateID, out HeroData heroData))
        {
            return;
        }

        SkeletonAnimation.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(Util.GetHeroSkeletonDataKey());
		SkeletonAnimation.Initialize(true);

		Status.SetInfo(this, heroData.Attack, heroData.Defense, heroData.MaxHP, heroData.CriticalValue, heroData.CriticalRate, heroData.SkillDamageValue, heroData.SkillCriticalValue, heroData.DodgeRate, heroData.ComboAttackRate, heroData.CounterAttackRate);

		Managers.Game.SetSkill();

        base.SetInfo(templateID);
    }

	#region Animation Event

	public override void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e)
    {
        base.OnAnimEventHandler(trackEntry, e);
    }

    #endregion
}
