using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CollectionData
{
	public int TemplateID { get; private set; }
	public string Remark { get; private set; }
	public string NameKey { get; private set; }
	public ECollectionType CollectionType { get; private set; }
	public List<int> NeedCollectionList { get; private set; }
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
	public float BossExtraValue { get; private set; }

	public CollectionData(JsonData json)
	{ 
		TemplateID = int.Parse(json["TemplateID"].ToString());
		Remark = json["Remark"].ToString();
		NameKey = json["NameKey"].ToString();
		CollectionType = (ECollectionType)Enum.Parse(typeof(ECollectionType), json["CollectionType"].ToString());

		NeedCollectionList = new List<int>();
		JsonData needCollectionListJson = JsonMapper.ToObject(json["NeedCollectionList"].ToString());
		if (needCollectionListJson.IsArray)
		{
			foreach (JsonData collectionID in needCollectionListJson)
			{
				NeedCollectionList.Add((int)collectionID);
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
		BossExtraValue = float.Parse(json["BossExtraValue"].ToString());
	}
} 
