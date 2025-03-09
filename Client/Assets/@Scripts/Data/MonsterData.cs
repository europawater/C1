using LitJson;
using System;
using System.Collections.Generic;
using static Define;

public class MonsterData
{
	public int TemplateID { get; private set; }
	public string Remark { get; private set; }
	public string NameKey { get; private set; }
	public string SkeletondataKey { get; private set; }
	public EMonsterType MonsterType { get; private set; }
	public int DefaultSkill { get; private set; }
	public List<int> SkillList { get; private set; }
	public int Attack { get; private set; }
	public int Defense { get; private set; }
	public int MaxHP { get; private set; }
	public float CriticalValue { get; private set; }
	public float CriticalRate { get; private set; }
	public float SkillDamageValue { get; private set; }
	public float SkillCriticalValue { get; private set; }
	public float DodgeRate { get; private set; }
	public float ComboAttackRate { get; private set; }
	public float CounterAttackRate { get; private set; }

	public MonsterData(JsonData json)
	{ 
		TemplateID = int.Parse(json["TemplateID"].ToString());
		Remark = json["Remark"].ToString();
		NameKey = json["NameKey"].ToString();
		SkeletondataKey = json["SkeletondataKey"].ToString();
		MonsterType = (EMonsterType)Enum.Parse(typeof(EMonsterType), json["MonsterType"].ToString());
		DefaultSkill = int.Parse(json["DefaultSkill"].ToString());

		SkillList = new List<int>();
		JsonData skillListJson = JsonMapper.ToObject(json["SkillList"].ToString());
		if (skillListJson.IsArray)
		{
			foreach (JsonData skill in skillListJson)
			{
				SkillList.Add((int)skill);
			}
		}

		Attack = int.Parse(json["Attack"].ToString());
		Defense = int.Parse(json["Defense"].ToString());
		MaxHP = int.Parse(json["MaxHP"].ToString());
		CriticalValue = float.Parse(json["CriticalValue"].ToString());
		CriticalRate = float.Parse(json["CriticalRate"].ToString());
		SkillDamageValue = float.Parse(json["SkillDamageValue"].ToString());
		SkillCriticalValue = float.Parse(json["SkillCriticalValue"].ToString());
		DodgeRate = float.Parse(json["DodgeRate"].ToString());
		ComboAttackRate = float.Parse(json["ComboAttackRate"].ToString());
		CounterAttackRate = float.Parse(json["CounterAttackRate"].ToString());
	}
}
