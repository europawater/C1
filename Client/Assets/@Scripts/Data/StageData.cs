using LitJson;
using System.Collections.Generic;

public class StageData
{
	public int TemplateID { get; private set; }
	public string Remark { get; private set; }
	public string NameKey { get; private set; }
	public string PrefabKey { get; private set; }
	public string BGMKey { get; private set; }
	public List<int> FirstWaveMonsterList { get; private set; }
	public List<int> SecondWaveMonsterList { get; private set; }
	public List<int> ThirdWaveMonsterList { get; private set; }
	public List<int> FourthWaveMonsterList { get; private set; }
	public List<int> BossWaveMonsterList { get; private set; }
	public int DifficultyLevel { get; private set; }
	public int MinGoldReward { get; private set; }
	public int MaxGoldReward { get; private set; }

	public StageData(JsonData json)
	{ 
		TemplateID = int.Parse(json["TemplateID"].ToString());
		Remark = json["Remark"].ToString();
		NameKey = json["NameKey"].ToString();
		PrefabKey = json["PrefabKey"].ToString();
		BGMKey = json["BGMKey"].ToString();

		FirstWaveMonsterList = new List<int>();
		JsonData firstWaveMonsterListJson = JsonMapper.ToObject(json["FirstWaveMonsterList"].ToString());
		if (firstWaveMonsterListJson.IsArray)
		{
			foreach (JsonData monsterID in firstWaveMonsterListJson)
			{
				FirstWaveMonsterList.Add((int)monsterID);
			}
		}

		SecondWaveMonsterList = new List<int>();
		JsonData secondWaveMonsterListJson = JsonMapper.ToObject(json["SecondWaveMonsterList"].ToString());
		if (secondWaveMonsterListJson.IsArray)
		{
			foreach (JsonData monsterID in secondWaveMonsterListJson)
			{
				SecondWaveMonsterList.Add((int)monsterID);
			}
		}

		ThirdWaveMonsterList = new List<int>();
		JsonData thirdWaveMonsterListJson = JsonMapper.ToObject(json["ThirdWaveMonsterList"].ToString());
		if (thirdWaveMonsterListJson.IsArray)
		{
			foreach (JsonData monsterID in thirdWaveMonsterListJson)
			{
				ThirdWaveMonsterList.Add((int)monsterID);
			}
		}

		FourthWaveMonsterList = new List<int>();
		JsonData fourthWaveMonsterListJson = JsonMapper.ToObject(json["FourthWaveMonsterList"].ToString());
		if (fourthWaveMonsterListJson.IsArray)
		{
			foreach (JsonData monsterID in fourthWaveMonsterListJson)
			{
				FourthWaveMonsterList.Add((int)monsterID);
			}
		}

		BossWaveMonsterList = new List<int>();
		JsonData bossWaveMonsterListJson = JsonMapper.ToObject(json["BossWaveMonsterList"].ToString());
		if (bossWaveMonsterListJson.IsArray)
		{
			foreach (JsonData monsterID in bossWaveMonsterListJson)
			{
				BossWaveMonsterList.Add((int)monsterID);
			}
		}

		DifficultyLevel = int.Parse(json["DifficultyLevel"].ToString());
		MinGoldReward = int.Parse(json["MinGoldReward"].ToString());
		MaxGoldReward = int.Parse(json["MaxGoldReward"].ToString());
	}
}
