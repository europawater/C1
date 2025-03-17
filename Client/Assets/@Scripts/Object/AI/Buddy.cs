using Spine;
using Spine.Unity;
using UnityEngine;
using static Define;

public class Buddy : BaseAIObject
{
	protected override void Init()
	{
		GameObjectType = EGameObjectType.Buddy;

		base.Init();
	}

	public override void SetInfo(int templateID)
	{
		if(!Managers.Backend.Chart.BuddyChart.DataDict.TryGetValue(templateID, out BuddyData buddyData))
		{
			return;
		}

		SkeletonAnimation.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(buddyData.SkeletondataKey);
		SkeletonAnimation.Initialize(true);

		SkillInfo defaultSkillInfo = new SkillInfo(buddyData.DefaultSkill, EOwningState.Owned, 1, true, 0);
		DefaultSkill = new Skill(this, defaultSkillInfo);
		SkillList.Clear();
		SkillList.Add(DefaultSkill);


		base.SetInfo(templateID);
	}
}
