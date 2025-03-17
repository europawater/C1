using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MissionData
{
	public int TemplateID { get; private set; }
	public string Remark { get; private set; }
	public string NameKey { get; private set; }
	public EMissionType MissionType { get; private set; }
	public EMissionGoal MissionGoal { get; private set; }
	public int MaxPoint { get; private set; }
	public int Point { get; private set; }
	public List<int> PointStepList { get; private set; }
	public List<ERewardType> RewardTypeList { get; private set; }
	public List<int> RewardValueList { get; private set; }

	public MissionData(JsonData jsonData)
	{
		TemplateID = int.Parse(jsonData["TemplateID"].ToString());
		Remark = jsonData["Remark"].ToString();
		NameKey = jsonData["NameKey"].ToString();
		MissionType = (EMissionType)Enum.Parse(typeof(EMissionType), jsonData["MissionType"].ToString());
		MissionGoal = (EMissionGoal)Enum.Parse(typeof(EMissionGoal), jsonData["MissionGoal"].ToString());
		MaxPoint = int.Parse(jsonData["MaxPoint"].ToString());
		Point = int.Parse(jsonData["Point"].ToString());
		
		PointStepList = new List<int>();
		JsonData pointStepListJson = JsonMapper.ToObject(jsonData["PointStepList"].ToString());
		if (pointStepListJson.IsArray)
		{ 
			foreach(JsonData pointStep in pointStepListJson)
			{
				PointStepList.Add((int)pointStep);
			}
		}

		RewardTypeList = new List<ERewardType>();
		JsonData rewardTypeListJson = JsonMapper.ToObject(jsonData["RewardTypeList"].ToString());
		if (rewardTypeListJson.IsArray)
		{
			foreach(JsonData rewardType in rewardTypeListJson)
			{
				RewardTypeList.Add((ERewardType)Enum.Parse(typeof(ERewardType), rewardType.ToString()));
			}
		}

		RewardValueList = new List<int>();
		JsonData rewardValueListJson = JsonMapper.ToObject(jsonData["RewardValueList"].ToString());
		if (rewardValueListJson.IsArray)
		{
			foreach (JsonData rewardValue in rewardValueListJson)
			{
				RewardValueList.Add((int)rewardValue);
			}
		}
	}
}
