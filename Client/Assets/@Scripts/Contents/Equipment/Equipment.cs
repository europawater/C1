using System.Collections.Generic;
using System.Linq;
using static Define;

/// <summary>
/// 게임서 사용되는 장비 아이템을 정의한 클래스입니다.
/// <see cref="EquipmentData"/>와 <see cref="EquipmentLevelScalingData"/>를 이용하여 장비 아이템의 정보를 정의합니다."/>
/// <see cref="EquipmentInfo"/>를 이용하여 장비 아이템의 정보를 저장합니다."/>
/// </summary>
public class Equipment
{
	public EquipmentData EquipmentData { get; private set; }
	public EquipmentLevelScalingData EquipmentLevelScalingData { get; private set; }
	public EquipmentInfo EquipmentInfo { get; private set; }

	public int EnchantLevel => EquipmentInfo.EnchantLevel;
	public int EnchantSafe => EquipmentInfo.EnchantSafe;

	/// <summary>장비 아이템 능력치 정보입니다.</summary>
	public Dictionary<EStat, float> EquipmentValueDict { get; private set; } = new Dictionary<EStat, float>();
	public Dictionary<EStat, float> ValidEquipmentValueDict
	{
		get { return EquipmentValueDict.Where(kv => kv.Value != 0).ToDictionary(kv => kv.Key, kv => kv.Value); }
	}

	public Equipment(EquipmentInfo equipmentInfo)
	{
		EquipmentData = Managers.Backend.Chart.EquipmentChart.DataDict[equipmentInfo.TemplateID];
		EquipmentLevelScalingData = Managers.Backend.Chart.EquipmentLevelScalingChart.DataDict[equipmentInfo.TemplateID];
		EquipmentInfo = equipmentInfo;

		Initialize();
	}

	public Equipment(EEquipmentGrade grade, EEquipmentPart part)
	{
		EquipmentData = Managers.Backend.Chart.EquipmentChart.DataDict.Values.FirstOrDefault(data => data.EquipmentGrade == grade && data.EquipmentPart == part);
		EquipmentLevelScalingData = Managers.Backend.Chart.EquipmentLevelScalingChart.DataDict[EquipmentData.TemplateID];
		EquipmentInfo = new EquipmentInfo(EquipmentData.TemplateID, 0, 3);

		Initialize();
	}

	private void Initialize()
	{
		EquipmentValueDict.Clear();
		EquipmentValueDict.Add(EStat.Attack, CalculateEquipmentValue(EStat.Attack));
		EquipmentValueDict.Add(EStat.Defense, CalculateEquipmentValue(EStat.Defense));
		EquipmentValueDict.Add(EStat.MaxHP, CalculateEquipmentValue(EStat.MaxHP));
		EquipmentValueDict.Add(EStat.CriticalValue, CalculateEquipmentValue(EStat.CriticalValue));
		EquipmentValueDict.Add(EStat.CriticalRate, CalculateEquipmentValue(EStat.CriticalRate));
		EquipmentValueDict.Add(EStat.SkillDamageValue, CalculateEquipmentValue(EStat.SkillDamageValue));
		EquipmentValueDict.Add(EStat.SkillCriticalValue, CalculateEquipmentValue(EStat.SkillCriticalValue));
		EquipmentValueDict.Add(EStat.DodgeRate, CalculateEquipmentValue(EStat.DodgeRate));
		EquipmentValueDict.Add(EStat.ComboAttackRate, CalculateEquipmentValue(EStat.ComboAttackRate));
		EquipmentValueDict.Add(EStat.CounterAttackRate, CalculateEquipmentValue(EStat.CounterAttackRate));
		EquipmentValueDict.Add(EStat.BossExtraValue, CalculateEquipmentValue(EStat.BossExtraValue));
	}

	public float CalculateEquipmentValue(EStat stat)
	{
		float defaultStat = 0.0f;
		float increaseStat = 0.0f;
		switch (stat)
		{
			case EStat.Attack:
				defaultStat = EquipmentData.Attack;
				increaseStat = EquipmentLevelScalingData.IncreaseAttack;
				return defaultStat + (increaseStat * EnchantLevel);
			case EStat.Defense:
				defaultStat = EquipmentData.Defense;
				increaseStat = EquipmentLevelScalingData.IncreaseDefense;
				return defaultStat + (increaseStat * EnchantLevel);
			case EStat.MaxHP:
				defaultStat = EquipmentData.MaxHP;
				increaseStat = EquipmentLevelScalingData.IncreaseMaxHP;
				return defaultStat + (increaseStat * EnchantLevel);
			case EStat.CriticalValue:
				defaultStat = EquipmentData.CriticalValue;
				increaseStat = EquipmentLevelScalingData.IncreaseCriticalValue;
				return defaultStat + (increaseStat * EnchantLevel);
			case EStat.CriticalRate:
				defaultStat = EquipmentData.CriticalRate;
				increaseStat = EquipmentLevelScalingData.IncreaseCriticalRate;
				return defaultStat + (increaseStat * EnchantLevel);
			case EStat.SkillDamageValue:
				defaultStat = EquipmentData.SkillDamageValue;
				increaseStat = EquipmentLevelScalingData.IncreaseSkillDamageValue;
				return defaultStat + (increaseStat * EnchantLevel);
			case EStat.SkillCriticalValue:
				defaultStat = EquipmentData.SkillCriticalValue;
				increaseStat = EquipmentLevelScalingData.IncreaseSkillCriticalValue;
				return defaultStat + (increaseStat * EnchantLevel);
			case EStat.DodgeRate:
				defaultStat = EquipmentData.DodgeRate;
				increaseStat = EquipmentLevelScalingData.IncreaseDodgeRate;
				return defaultStat + (increaseStat * EnchantLevel);
			case EStat.ComboAttackRate:
				defaultStat = EquipmentData.ComboAttackRate;
				increaseStat = EquipmentLevelScalingData.IncreaseComboAttackRate;
				return defaultStat + (increaseStat * EnchantLevel);
			case EStat.CounterAttackRate:
				defaultStat = EquipmentData.CounterAttackRate;
				increaseStat = EquipmentLevelScalingData.IncreaseCounterAttackRate;
				return defaultStat + (increaseStat * EnchantLevel);
			case EStat.BossExtraValue:
				defaultStat = EquipmentData.BossExtraValue;
				increaseStat = EquipmentLevelScalingData.IncreaseBossExtraValue;
				return defaultStat + (increaseStat * EnchantLevel);

			default:
				return 0.0f;
		}
	}
}
