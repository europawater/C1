using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

/// <summary>
/// 동료 정보를 담는 클래스입니다.
/// 뒤끝에 저장되는 데이터입니다.
/// </summary>
[Serializable]
public class BuddyInfo
{
	public int TemplateID { get; private set; }
	public EOwningState OwningState { get; private set; }
	public int Level { get; private set; }
	public bool IsEquipped { get; private set; }
	public int PieceCount { get; private set; }

	public bool CanLevelUp
	{
		get
		{
			return Level <= 5 && PieceCount >= Managers.Backend.Chart.BuddyChart.DataDict[TemplateID].LevelUpPiece;
		}
	}

	public Dictionary<EStat, float> Stats
	{
		get
		{
			BuddyData buddyData = Managers.Backend.Chart.BuddyChart.DataDict[TemplateID];
			Dictionary<EStat, float> stats = new Dictionary<EStat, float>();
			stats.Add(EStat.Attack, 0);
			stats.Add(EStat.Defense, 0);
			stats.Add(EStat.MaxHP, 0);

			if (Level >= 1)
			{
				foreach (KeyValuePair<EStat, float> pair in buddyData.OneStarBonusDict)
				{
					stats[pair.Key] += pair.Value;
				}
			}

			if (Level >= 2)
			{
				foreach (KeyValuePair<EStat, float> pair in buddyData.TwoStarBonusDict)
				{
					stats[pair.Key] += pair.Value;
				}
			}

			if (Level >= 3)
			{
				foreach (KeyValuePair<EStat, float> pair in buddyData.ThreeStarBonusDict)
				{
					stats[pair.Key] += pair.Value;
				}
			}

			if (Level >= 4)
			{
				foreach (KeyValuePair<EStat, float> pair in buddyData.FourStarBonusDict)
				{
					stats[pair.Key] += pair.Value;
				}
			}

			if (Level >= 5)
			{
				foreach (KeyValuePair<EStat, float> pair in buddyData.FiveStarBonusDict)
				{
					stats[pair.Key] += pair.Value;
				}
			}

			return stats;
		}
	}

	public BuddyInfo(int templatedID, EOwningState owningState, int level, bool isEquipped, int pieceCount)
	{
		TemplateID = templatedID;
		OwningState = owningState;
		Level = level;
		IsEquipped = isEquipped;
		PieceCount = pieceCount;
	}

	public BuddyInfo(JsonData jsonData)
	{
		TemplateID = int.Parse(jsonData["TemplateID"].ToString());
		OwningState = (EOwningState)Enum.Parse(typeof(EOwningState), jsonData["OwningState"].ToString());
		Level = int.Parse(jsonData["Level"].ToString());
		IsEquipped = bool.Parse(jsonData["IsEquipped"].ToString());
		PieceCount = int.Parse(jsonData["PieceCount"].ToString());
	}

	public void SetOwningState(EOwningState owningState)
	{
		OwningState = owningState;
	}

	public void LevelUp()
	{
		if (!CanLevelUp)
		{
			return;
		}

		Level++;
		SubtractPieceCount(Managers.Backend.Chart.BuddyChart.DataDict[TemplateID].LevelUpPiece);
	}

	public void SetIsEquipped(bool isEquipped)
	{
		IsEquipped = isEquipped;
	}

	public void AddPieceCount(int pieceCount)
	{
		PieceCount += pieceCount;
	}

	public void SubtractPieceCount(int pieceCount)
	{
		PieceCount -= pieceCount;
	}
}
