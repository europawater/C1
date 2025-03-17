using LitJson;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BuddyData
{
	public int TemplateID { get; private set; }
	public string Remark { get; private set; }
	public string NameKey { get; private set; }
	public string SlotKey { get; private set; }
	public string IconKey { get; private set; }
	public string SkeletondataKey { get; private set; }
	public EBuddyGrade BuddyGrade { get; private set; }
	public int DefaultSkill { get; private set; }
	public Dictionary<EStat, float> OneStarBonusDict { get; private set; } = new Dictionary<EStat, float>();
	public Dictionary<EStat, float> TwoStarBonusDict { get; private set; } = new Dictionary<EStat, float>();
	public Dictionary<EStat, float> ThreeStarBonusDict { get; private set; } = new Dictionary<EStat, float>();
	public Dictionary<EStat, float> FourStarBonusDict { get; private set; } = new Dictionary<EStat, float>();
	public Dictionary<EStat, float> FiveStarBonusDict { get; private set; } = new Dictionary<EStat, float>();
	public int LevelUpPiece { get; private set; }

	public BuddyData(JsonData jsonData)
	{
		TemplateID = int.Parse(jsonData["TemplateID"].ToString());
		Remark = jsonData["Remark"].ToString();
		NameKey = jsonData["NameKey"].ToString();
		SlotKey = jsonData["SlotKey"].ToString();
		IconKey = jsonData["IconKey"].ToString();
		SkeletondataKey = jsonData["SkeletondataKey"].ToString();
		BuddyGrade = (EBuddyGrade)Enum.Parse(typeof(EBuddyGrade), jsonData["BuddyGrade"].ToString());
		DefaultSkill = int.Parse(jsonData["DefaultSkill"].ToString());

		JsonData oneStarBonusListJson = JsonMapper.ToObject(jsonData["OneStarBonusList"].ToString());
		if (oneStarBonusListJson.IsArray)
		{
			foreach (JsonData stat in oneStarBonusListJson)
			{
				OneStarBonusDict.Add((EStat)Enum.Parse(typeof(EStat), stat.ToString()), 0);
			}
		}
		JsonData oneStarBonusValueListJson = JsonMapper.ToObject(jsonData["OneStarBonusValueList"].ToString());
		if (oneStarBonusValueListJson.IsArray)
		{
			int index = 1;
			foreach (JsonData value in oneStarBonusValueListJson)
			{
				OneStarBonusDict[(EStat)index] = float.Parse(value.ToString());
				index++;
			}
		}

		JsonData twoStarBonusListJson = JsonMapper.ToObject(jsonData["TwoStarBonusList"].ToString());
		if (twoStarBonusListJson.IsArray)
		{
			foreach (JsonData stat in twoStarBonusListJson)
			{
				TwoStarBonusDict.Add((EStat)Enum.Parse(typeof(EStat), stat.ToString()), 0);
			}
		}
		JsonData twoStarBonusValueListJson = JsonMapper.ToObject(jsonData["TwoStarBonusValueList"].ToString());
		if (twoStarBonusValueListJson.IsArray)
		{
			int index = 1;
			foreach (JsonData value in twoStarBonusValueListJson)
			{
				TwoStarBonusDict[(EStat)index] = float.Parse(value.ToString());
				index++;
			}
		}

		JsonData threeStarBonusListJson = JsonMapper.ToObject(jsonData["ThreeStarBonusList"].ToString());
		if (threeStarBonusListJson.IsArray)
		{
			foreach (JsonData stat in threeStarBonusListJson)
			{
				ThreeStarBonusDict.Add((EStat)Enum.Parse(typeof(EStat), stat.ToString()), 0);
			}
		}
		JsonData threeStarBonusValueListJson = JsonMapper.ToObject(jsonData["ThreeStarBonusValueList"].ToString());
		if (threeStarBonusValueListJson.IsArray)
		{
			int index = 1;
			foreach (JsonData value in threeStarBonusValueListJson)
			{
				ThreeStarBonusDict[(EStat)index] = float.Parse(value.ToString());
				index++;
			}
		}

		JsonData fourStarBonusListJson = JsonMapper.ToObject(jsonData["FourStarBonusList"].ToString());
		if (fourStarBonusListJson.IsArray)
		{
			foreach (JsonData stat in fourStarBonusListJson)
			{
				FourStarBonusDict.Add((EStat)Enum.Parse(typeof(EStat), stat.ToString()), 0);
			}
		}
		JsonData fourStarBonusValueListJson = JsonMapper.ToObject(jsonData["FourStarBonusValueList"].ToString());
		if (fourStarBonusValueListJson.IsArray)
		{
			int index = 1;
			foreach (JsonData value in fourStarBonusValueListJson)
			{
				FourStarBonusDict[(EStat)index] = float.Parse(value.ToString());
				index++;
			}
		}

		JsonData fiveStarBonusListJson = JsonMapper.ToObject(jsonData["FiveStarBonusList"].ToString());
		if (fiveStarBonusListJson.IsArray)
		{
			foreach (JsonData stat in fiveStarBonusListJson)
			{
				FiveStarBonusDict.Add((EStat)Enum.Parse(typeof(EStat), stat.ToString()), 0);
			}
		}
		JsonData fiveStarBonusValueListJson = JsonMapper.ToObject(jsonData["FiveStarBonusValueList"].ToString());
		if (fiveStarBonusValueListJson.IsArray)
		{
			int index = 1;
			foreach (JsonData value in fiveStarBonusValueListJson)
			{
				FiveStarBonusDict[(EStat)index] = float.Parse(value.ToString());
				index++;
			}
		}

		LevelUpPiece = int.Parse(jsonData["LevelUpPiece"].ToString());
	}
}
