using LitJson;
using System;
using UnityEngine;
using static Define;

public class SkillData
{
	public int TemplateID { get; private set; }
	public string Remark { get; private set; }
	public string Namekey { get; private set; }
	public string IconKey { get; private set; }
	public ESkillCaster SkillCaster { get; private set; }
	public ESkillGrade SkillGrade { get; private set; }
	public ESkillType SkillType { get; private set; }
	public string SkillEffectPrefabKey { get; private set; }
	public string HitEffectPrefabKey { get; private set; }
	public int NeedTurnCount { get; private set; }
	public float AttackValue { get; private set; }
	public float HitStartTime { get; private set; }
	public int HitCount { get; private set; }
	public float HitDelay { get; private set; }
	public float LifeTime { get; private set; }

	public SkillData(JsonData json)
	{
		TemplateID = int.Parse(json["TemplateID"].ToString());
		Remark = json["Remark"].ToString();
		Namekey = json["Namekey"].ToString();
		IconKey = json["IconKey"].ToString();
		SkillCaster = (ESkillCaster)Enum.Parse(typeof(ESkillCaster), json["SkillCaster"].ToString());
		SkillGrade = (ESkillGrade)Enum.Parse(typeof(ESkillGrade), json["SkillGrade"].ToString());
		SkillType = (ESkillType)Enum.Parse(typeof(ESkillType), json["SkillType"].ToString());
		SkillEffectPrefabKey = json["SkillEffectPrefabKey"].ToString();
		HitEffectPrefabKey = json["HitEffectPrefabKey"].ToString();
		NeedTurnCount = int.Parse(json["NeedTurnCount"].ToString());
		AttackValue = float.Parse(json["AttackValue"].ToString());
		HitStartTime = float.Parse(json["HitStartTime"].ToString());
		HitCount = int.Parse(json["HitCount"].ToString());
		HitDelay = float.Parse(json["HitDelay"].ToString());
		LifeTime = float.Parse(json["LifeTime"].ToString());
	}
}
