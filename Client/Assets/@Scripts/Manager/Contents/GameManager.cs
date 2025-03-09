using System.Collections.Generic;
using System.Linq;
using static Define;

public class GameManager
{
	#region Equipment

	public Dictionary<EEquipmentPart, Equipment> EquipmentDict { get; private set; } = new Dictionary<EEquipmentPart, Equipment>();
	public Dictionary<EStat, float> EquipmentValueDict { get; private set; } = new Dictionary<EStat, float>();
	public Equipment NewEquipment { get; private set; }

	public void EquipmentInit()
	{
		foreach (var equipmentInfo in Managers.Backend.GameData.Equipment.EquipmentInfoDict)
		{
			EquipmentDict.Add(equipmentInfo.Key, equipmentInfo.Value.TemplateID == 0 ? null : new Equipment(equipmentInfo.Value));
		}

		UpdateEquipmentStat();
	}

	private void UpdateEquipmentStat()
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

	private float CalculateEquipmentValue(EStat stat)
	{
		float equipmentValue = 0.0f;
		foreach (Equipment equipment in EquipmentDict.Values)
		{
			if (equipment == null)
			{
				continue;
			}

			equipmentValue += equipment.EquipmentValueDict[stat];
		}

		return equipmentValue;
	}

	public void CreateEquipmentByGradeAndPart(EEquipmentGrade grade, EEquipmentPart part)
	{
		Equipment newEquipment = new Equipment(grade, part);
		NewEquipment = newEquipment;
		Equipment oldEquipment = EquipmentDict[part] == null ? null : EquipmentDict[part];

		UI_EquipmentPopup popup = Managers.UI.ShowPopupUI<UI_EquipmentPopup>();
		popup.SetInfo(oldEquipment, NewEquipment);
	}

	public void HandleRemoveNewEquipment()
	{
		Managers.Backend.GameData.Equipment.EquipEquipment(null);

		NewEquipment = null;

		UpdateEquipmentStat();

		// TODO : 플레이어 경험치 획득
	}

	public void HandleEquipNewEquipment()
	{
		if (NewEquipment == null)
		{
			return;
		}

		EquipmentDict[NewEquipment.EquipmentData.EquipmentPart] = NewEquipment;
		Managers.Backend.GameData.Equipment.EquipEquipment(NewEquipment);

		NewEquipment = null;

		UpdateEquipmentStat();
	}

	#endregion

	#region Dice

	public Dice Dice { get; private set; }
	public DiceData DiceData
	{
		get
		{
			if (!Managers.Backend.Chart.DiceChart.DataDict.TryGetValue(Managers.Backend.GameData.Dice.DiceLevel, out DiceData diceData))
			{
				return null;
			}

			return diceData;
		}
	}

	public void SetDiceObject(Dice dice)
	{
		Dice = dice;
	}

	public bool CanDrawEquipment()
	{
		if (Dice.DiceState == EDiceState.Draw)
		{
			return false;
		}

		if (Managers.Game.NewEquipment != null)
		{
			return false;
		}

		if (Managers.Backend.GameData.Dice.DiceCount <= 0)
		{
			return false;
		}

		return true;
	}

	public void HandleDrawEquipment()
	{
		Managers.Backend.GameData.Dice.SubDiceCount(1);

		float randomValue = Util.GetRadomfloat(0.0f, 100.0f);
		EEquipmentGrade grade = GetGradeByRate(randomValue);
		EEquipmentPart part = GetRandomPart();

		Managers.Game.CreateEquipmentByGradeAndPart(grade, part);
	}

	// FIXME: 뒤끝 랜덤함수에서 처리할수 있도록..
	private EEquipmentGrade GetGradeByRate(float randomValue)
	{
		float cumulativeRate = 0.0f;

		if (DiceData.CommonRate > 0)
		{
			cumulativeRate += DiceData.CommonRate;
			if (randomValue <= cumulativeRate)
			{
				return EEquipmentGrade.Common;
			}
		}

		if (DiceData.RareRate > 0)
		{
			cumulativeRate += DiceData.RareRate;
			if (randomValue <= cumulativeRate)
			{
				return EEquipmentGrade.Rare;
			}
		}

		if (DiceData.UniqueRate > 0)
		{
			cumulativeRate += DiceData.UniqueRate;
			if (randomValue <= cumulativeRate)
			{
				return EEquipmentGrade.Unique;
			}
		}

		if (DiceData.EpicRate > 0)
		{
			cumulativeRate += DiceData.EpicRate;
			if (randomValue <= cumulativeRate)
			{
				return EEquipmentGrade.Epic;
			}
		}

		if (DiceData.LegendRate > 0)
		{
			cumulativeRate += DiceData.LegendRate;
			if (randomValue <= cumulativeRate)
			{
				return EEquipmentGrade.Legend;
			}
		}

		if (DiceData.MythicRate > 0)
		{
			cumulativeRate += DiceData.MythicRate;
			if (randomValue <= cumulativeRate)
			{
				return EEquipmentGrade.Mythic;
			}
		}

		if (DiceData.BeyondRate > 0)
		{
			cumulativeRate += DiceData.BeyondRate;
			if (randomValue <= cumulativeRate)
			{
				return EEquipmentGrade.Beyond;
			}
		}

		if (DiceData.TwilightRate > 0)
		{
			cumulativeRate += DiceData.TwilightRate;
			if (randomValue <= cumulativeRate)
			{
				return EEquipmentGrade.Twilight;
			}
		}

		return EEquipmentGrade.None;
	}

	private const int MAX_PART_COUNT = 8;
	private EEquipmentPart GetRandomPart()
	{
		int randomValue = Util.GetRandomInt(1, MAX_PART_COUNT);
		return (EEquipmentPart)randomValue;
	}

	#endregion

	#region Skill

	private int? _selectedSkillSlotIndex = null;
	public int? SelectedSkillSlotIndex
	{
		get { return _selectedSkillSlotIndex; }
		set
		{
			_selectedSkillSlotIndex = _selectedSkillSlotIndex == value ? null : value;

			Managers.Event.TriggerEvent(EEventType.OnSelectedSkillSlotIndex);
		}
	}

	public class SkillPool
	{
		private readonly Stack<Skill> _pool = new Stack<Skill>();

		public Skill GetSkill(BaseAIObject owner, SkillInfo skillInfo)
		{
			if (_pool.Count > 0)
			{
				Skill skill = _pool.Pop();
				skill.SetInfo(owner, skillInfo);
				return skill;
			}

			return new Skill(owner, skillInfo);
		}

		public void ReturnSkill(Skill skill)
		{
			skill.Reset();
			_pool.Push(skill);
		}
	}

	public Skill DefaultSkill { get; private set; }
	public Dictionary<ESkillSlot, Skill> EquippedSkillSlotDict { get; private set; } = new Dictionary<ESkillSlot, Skill>();

	private SkillPool _skillPool = new SkillPool();

	public void SetSkill()
	{
		DefaultSkill = _skillPool.GetSkill(Managers.Object.Hero, Managers.Backend.GameData.Skill.DefaultSkillInfo);
		UpdateSkillSlot();
	}

	public void UpdateSkillSlot()
	{
		foreach (Skill skill in EquippedSkillSlotDict.Values)
		{
			_skillPool.ReturnSkill(skill);
		}

		EquippedSkillSlotDict.Clear();
		foreach (var skillSlot in Managers.Backend.GameData.Skill.SkillSlotDict)
		{
			SkillInfo skillInfo = null;
			if (!Managers.Backend.GameData.Skill.SkillInfoDict.TryGetValue(skillSlot.Value, out skillInfo))
			{
				continue;
			}

			EquippedSkillSlotDict.Add(skillSlot.Key, _skillPool.GetSkill(Managers.Object.Hero, skillInfo));
		}
	}

	#endregion
}
