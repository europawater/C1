using LitJson;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SkillChart : BaseChart<SkillData>
{
	public Dictionary<int, SkillData> HeroSkillDataDict { get; private set; } = new Dictionary<int, SkillData>();
	public Dictionary<int, SkillData> MonsterSkillDataDict { get; private set; } = new Dictionary<int, SkillData>();
	public Dictionary<int, SkillData> BuddySkillDataDict { get; private set; } = new Dictionary<int, SkillData>();

	public override string GetChartFileName()
	{
		return "SkillChart";
	}

	protected override void InitializeData(JsonData jsonData)
	{
		foreach (JsonData data in jsonData)
		{
			SkillData skillData = new SkillData(data);
			switch (skillData.SkillCaster)
			{
				case ESkillCaster.Hero:
					HeroSkillDataDict.Add(skillData.TemplateID, skillData);
					break;
				case ESkillCaster.Monster:
					MonsterSkillDataDict.Add(skillData.TemplateID, skillData);
					break;
				case ESkillCaster.Buddy:
					BuddySkillDataDict.Add(skillData.TemplateID, skillData);
					break;

				default:
					break;
			}
		}
	}
}
