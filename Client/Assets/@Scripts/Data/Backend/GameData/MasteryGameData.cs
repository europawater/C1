using BackEnd;
using LitJson;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MasteryGameData : BaseGameData
{
	public int MasteryGrade { get; private set; }
	public int CurrentMasteryCountByGrade { get; private set; }
	public Dictionary<EMasteryType, int> MasteryLevelDict { get; private set; } = new Dictionary<EMasteryType, int>();
	public Dictionary<EMasteryType, int> MasteryValueDict { get; private set; } = new Dictionary<EMasteryType, int>();

	/// <summary>마스터리 종류 갯수</summary>
	private const int MASTERY_TYPE_COUNT = 3;
	/// <summary>다음 마스터리 등급을 올리기 위해 필요한 최대 Count 입니다.</summary>
	public int MaxMasteryCountByGrade
	{
		get { return (Managers.Backend.Chart.MasteryChart.DataDict[MasteryGrade].MaxLevel - Managers.Backend.Chart.MasteryChart.DataDict[MasteryGrade].StartLevel) * MASTERY_TYPE_COUNT; }
	}

	public override string GetTableName()
	{
		return "Mastery";
	}

	public override Param GetParam()
	{
		Param param = new Param();

		param.Add("MasteryGrade", MasteryGrade);
		param.Add("CurrentMasteryCountByGrade", CurrentMasteryCountByGrade);
		param.Add("MasteryLevelDict", MasteryLevelDict);

		return param;
	}

	protected override void InitializeData()
	{
		MasteryGrade = 1;
		CurrentMasteryCountByGrade = 0;

		MasteryLevelDict.Clear();
		MasteryLevelDict.Add(EMasteryType.Attack, 0);
		MasteryLevelDict.Add(EMasteryType.Defense, 0);
		MasteryLevelDict.Add(EMasteryType.MaxHP, 0);

		MasteryValueDict.Clear();
		MasteryValueDict.Add(EMasteryType.Attack, CalculateCumulativeMasteryValue(EMasteryType.Attack));
		MasteryValueDict.Add(EMasteryType.Defense, CalculateCumulativeMasteryValue(EMasteryType.Defense));
		MasteryValueDict.Add(EMasteryType.MaxHP, CalculateCumulativeMasteryValue(EMasteryType.MaxHP));
	}

	protected override void SetServerDataToLocal(JsonData gameDataJson)
	{
		MasteryGrade = int.Parse(gameDataJson["MasteryGrade"].ToString());
		CurrentMasteryCountByGrade = int.Parse(gameDataJson["CurrentMasteryCountByGrade"].ToString());

		MasteryLevelDict.Clear();
		MasteryLevelDict.Add(EMasteryType.Attack, int.Parse(gameDataJson["MasteryLevelDict"]["Attack"].ToString()));
		MasteryLevelDict.Add(EMasteryType.Defense, int.Parse(gameDataJson["MasteryLevelDict"]["Defense"].ToString()));
		MasteryLevelDict.Add(EMasteryType.MaxHP, int.Parse(gameDataJson["MasteryLevelDict"]["MaxHP"].ToString()));

		MasteryValueDict.Clear();
		MasteryValueDict.Add(EMasteryType.Attack, CalculateCumulativeMasteryValue(EMasteryType.Attack));
		MasteryValueDict.Add(EMasteryType.Defense, CalculateCumulativeMasteryValue(EMasteryType.Defense));
		MasteryValueDict.Add(EMasteryType.MaxHP, CalculateCumulativeMasteryValue(EMasteryType.MaxHP));
	}

	protected override void UpdateData()
	{
		base.UpdateData();

		Managers.Event.TriggerEvent(EEventType.OnMasteryChanged);
	}

	#region Contents

	private int CalculateCumulativeMasteryValue(EMasteryType masteryType)
	{
		int cumulativeValue = 0;
		int masteryLevel = MasteryLevelDict[masteryType];

		for (int grade = 1; grade <= MasteryGrade; grade++)
		{
			MasteryData masteryData = Managers.Backend.Chart.MasteryChart.DataDict[grade];
			int levelToUse = (grade == MasteryGrade) ? (masteryLevel - masteryData.StartLevel) : masteryData.MaxLevel;

			switch (masteryType)
			{
				case EMasteryType.Attack:
					cumulativeValue += masteryData.IncreaseAttackValue * levelToUse;
					break;
				case EMasteryType.Defense:
					cumulativeValue += masteryData.IncreaseDefenseValue * levelToUse;
					break;
				case EMasteryType.MaxHP:
					cumulativeValue += masteryData.IncreaseMaxHPValue * levelToUse;
					break;
			}
		}

		return cumulativeValue;
	}

	public int GetNeedToGold(EMasteryType masteryType)
	{
		int startGoldAmount = Managers.Backend.Chart.MasteryChart.DataDict[MasteryGrade].StartGoldAmount;
		int levelDifference = MasteryLevelDict[masteryType] - Managers.Backend.Chart.MasteryChart.DataDict[MasteryGrade].StartLevel;
		int increaseGoldAmount = Managers.Backend.Chart.MasteryChart.DataDict[MasteryGrade].IncreaseGoldAmount * levelDifference;
		return startGoldAmount + increaseGoldAmount;
	}

	public bool GetCanMasterLevelUp(EMasteryType masteryType)
	{
		if (Managers.Backend.Chart.MasteryChart.DataDict[MasteryGrade].MaxLevel <= MasteryLevelDict[masteryType])
		{
			return false;
		}

		if (Managers.Backend.GameData.Currency.CurrencyDict[ECurrency.Gold] < GetNeedToGold(masteryType))
		{
			return false;
		}

		return true;
	}

	public void LevelUpMastery(EMasteryType masteryType)
	{
		if (!GetCanMasterLevelUp(masteryType))
		{
			return;
		}

		Managers.Backend.GameData.Currency.RemoveAmount(ECurrency.Gold, GetNeedToGold(masteryType));

        MasteryLevelDict[masteryType]++;
		CurrentMasteryCountByGrade++;

		int increaseValue = 0;
		switch (masteryType)
		{
			case EMasteryType.Attack:
				increaseValue = Managers.Backend.Chart.MasteryChart.DataDict[MasteryGrade].IncreaseAttackValue;
				break;
			case EMasteryType.Defense:
				increaseValue = Managers.Backend.Chart.MasteryChart.DataDict[MasteryGrade].IncreaseDefenseValue;
				break;
			case EMasteryType.MaxHP:
				increaseValue = Managers.Backend.Chart.MasteryChart.DataDict[MasteryGrade].IncreaseMaxHPValue;
				break;
		}

		MasteryValueDict[masteryType] += increaseValue;

		UpdateData();

        if (Managers.Object.Hero != null)
        {
            Managers.Object.Hero.GetComponent<HeroStatus>().UpdateMasteryStat();
        }
    }

	public bool GetCanMasterytUpgrade()
	{
		if (MaxMasteryCountByGrade > CurrentMasteryCountByGrade)
		{
			return false;
		}

		return true;
	}

	public void UpgradeMastery()
	{
		if (!GetCanMasterytUpgrade())
		{
			return;
		}

		MasteryGrade++;
		CurrentMasteryCountByGrade = 0;

		UpdateData();
	}

	public float GetMasteryProgress()
	{
		return (float)CurrentMasteryCountByGrade / MaxMasteryCountByGrade;
	}

	#endregion
}
