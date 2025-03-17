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
		popup.SetInfo(oldEquipment);
	}

	public void HandleRemoveNewEquipment()
	{
		Managers.Backend.GameData.Equipment.EquipEquipment(null);

		NewEquipment = null;

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

		if (Managers.Object.Hero != null)
		{
			Managers.Object.Hero.GetComponent<HeroStatus>().UpdateEquipmentStat();
		}
	}

	public void HandleEnchantNewEquipment()
	{
		if (NewEquipment == null)
		{
			return;
		}

		float randomValue = Util.GetRadomfloat(0.0f, 100.0f);
		bool isSuccess = randomValue <= NewEquipment.EquipmentData.EnchantRate;

		if (isSuccess)
		{
			Managers.Backend.GameData.Equipment.EnchantEquipment(NewEquipment);
		}
		else
		{
			if (NewEquipment.EnchantSafe > 0)
			{
				Managers.Backend.GameData.Equipment.FailEnchant(NewEquipment);
			}
			else
			{
				NewEquipment = null;

				Managers.Event.TriggerEvent(EEventType.OnEquipmentEnchantFail);
			}
		}
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

	public void HandleDrawSkill()
	{
		List<int> drawSkillIdList = new List<int>();
		drawSkillIdList = Managers.Backend.GameData.Skill.DrawSkill();

		UI_DrawPopup popup = Managers.UI.ShowPopupUI<UI_DrawPopup>();
		popup.SetInfo(UI_DrawPopup.DrawPopupState.Skill, drawSkillIdList);
	}

	#endregion

	#region Buddy

	private int? _selectedBuddySlotIndex = null;
	public int? SelectedBuddySlotIndex
	{
		get { return _selectedBuddySlotIndex; }
		set
		{
			_selectedBuddySlotIndex = _selectedBuddySlotIndex == value ? null : value;

			Managers.Event.TriggerEvent(EEventType.OnSelectedBuddySlotIndex);
		}
	}

	public Dictionary<EBuddySlot, BuddyInfo> EquippedBuddyInfoSlotDict { get; private set; } = new Dictionary<EBuddySlot, BuddyInfo>();

	public void SetBuddy()
	{
		UpdateBuddySlot();
	}

	public void UpdateBuddySlot()
	{
		EquippedBuddyInfoSlotDict.Clear();
		foreach (var buddySlot in Managers.Backend.GameData.Buddy.BuddySlotDict)
		{
			BuddyInfo buddyInfo = null;
			if (!Managers.Backend.GameData.Buddy.BuddyInfoDict.TryGetValue(buddySlot.Value, out buddyInfo))
			{
				continue;
			}

			EquippedBuddyInfoSlotDict.Add(buddySlot.Key, buddyInfo);
		}
	}

	public void HandleDrawBuddy()
	{
		List<int> drawBuddyIdList = new List<int>();
		drawBuddyIdList = Managers.Backend.GameData.Buddy.DrawBuddy();

		UI_DrawPopup popup = Managers.UI.ShowPopupUI<UI_DrawPopup>();
		popup.SetInfo(UI_DrawPopup.DrawPopupState.Buddy, drawBuddyIdList);
	}

	#endregion

	#region Dungeon

	private EDungeonType _selectDungeonType = EDungeonType.None;
	public EDungeonType SelectDungeonType
	{
		get { return _selectDungeonType; }
		set
		{
			_selectDungeonType = value;
		}
	}

	#endregion

	#region Reward

	public void HandleReward(ERewardType rewardType, int rewardAmount)
	{
		switch (rewardType)
		{
			case ERewardType.Gold:
				Managers.Backend.GameData.Currency.AddAmount(ECurrency.Gold, rewardAmount);
				break;
			case ERewardType.Diamond:
				Managers.Backend.GameData.Currency.AddAmount(ECurrency.Diamond, rewardAmount);
				break;
			case ERewardType.DiceCount:
				Managers.Backend.GameData.Dice.AddDiceCount(rewardAmount);
				break;

			default:
				break;
		}

		List<Reward> rewardList = new List<Reward>();
		rewardList.Add(new Reward(rewardType, rewardAmount));
		UI_RewardPopup popup = Managers.UI.ShowPopupUI<UI_RewardPopup>();
		popup.SetInfo(rewardList);
	}

	#endregion

	#region Collection

	public List<Collection> CollectionList { get; private set; } = new List<Collection>();
	public List<Collection> ItemCollectionList => CollectionList.Where(collection => collection.CollectionData.CollectionType == ECollectionType.Item).ToList();
	public List<Collection> BuddyCollectionList => CollectionList.Where(collection => collection.CollectionData.CollectionType == ECollectionType.Buddy).ToList();
	public List<Collection> SkillCollectionList => CollectionList.Where(collection => collection.CollectionData.CollectionType == ECollectionType.Skill).ToList();
	public Dictionary<EStat, float> CollectionValueDict { get; private set; } = new Dictionary<EStat, float>();

	public void CollectionInit()
	{
		foreach (var collectionInfo in Managers.Backend.GameData.Collection.ItemCollectionInfoDict)
		{
			CollectionList.Add(collectionInfo.Value.TemplateID == 0 ? null : new Collection(collectionInfo.Value));
		}

		foreach (var collectionInfo in Managers.Backend.GameData.Collection.BuddyCollectionInfoDict)
		{
			CollectionList.Add(collectionInfo.Value.TemplateID == 0 ? null : new Collection(collectionInfo.Value));
		}

		foreach (var collectionInfo in Managers.Backend.GameData.Collection.SkillCollectionInfoDict)
		{
			CollectionList.Add(collectionInfo.Value.TemplateID == 0 ? null : new Collection(collectionInfo.Value));
		}

		UpdateCollectionStat();
	}

	private void UpdateCollectionStat()
	{
		CollectionValueDict.Clear();
		CollectionValueDict.Add(EStat.Attack, CalculateCollectionValue(EStat.Attack));
		CollectionValueDict.Add(EStat.Defense, CalculateCollectionValue(EStat.Defense));
		CollectionValueDict.Add(EStat.MaxHP, CalculateCollectionValue(EStat.MaxHP));
		CollectionValueDict.Add(EStat.CriticalValue, CalculateCollectionValue(EStat.CriticalValue));
		CollectionValueDict.Add(EStat.CriticalRate, CalculateCollectionValue(EStat.CriticalRate));
		CollectionValueDict.Add(EStat.SkillDamageValue, CalculateCollectionValue(EStat.SkillDamageValue));
		CollectionValueDict.Add(EStat.SkillCriticalValue, CalculateCollectionValue(EStat.SkillCriticalValue));
		CollectionValueDict.Add(EStat.DodgeRate, CalculateCollectionValue(EStat.DodgeRate));
		CollectionValueDict.Add(EStat.ComboAttackRate, CalculateCollectionValue(EStat.ComboAttackRate));
		CollectionValueDict.Add(EStat.CounterAttackRate, CalculateCollectionValue(EStat.CounterAttackRate));
		CollectionValueDict.Add(EStat.BossExtraValue, CalculateCollectionValue(EStat.BossExtraValue));
	}

	private float CalculateCollectionValue(EStat stat)
	{
		float collectionValue = 0.0f;
		foreach (Collection collection in CollectionList)
		{
			if (collection == null || collection.CollectionInfo.OwningState != EOwningState.Owned)
			{
				continue;
			}

			collectionValue += collection.CollectionValueDict[stat];
		}

		return collectionValue;
	}

	public bool CanRegistCollection(ECollectionType collectionType, int templateID)
	{
		switch (collectionType)
		{
			case ECollectionType.Item:
				foreach (Collection collection in ItemCollectionList)
				{
					if (collection.CollectionInfo.OwningState == EOwningState.Owned)
					{
						continue;
					}

					if (!collection.CollectionInfo.NeedCollectionDict.TryGetValue(templateID, out EOwningState owningState))
					{
						continue;
					}

					if (owningState == EOwningState.Unowned)
					{
						return true;
					}
				}
				break;
			case ECollectionType.Buddy:
				foreach (Collection collection in BuddyCollectionList)
				{
					if (collection.CollectionInfo.OwningState == EOwningState.Owned)
					{
						continue;
					}

					if (!collection.CollectionInfo.NeedCollectionDict.TryGetValue(templateID, out EOwningState owningState))
					{
						continue;
					}

					if (owningState == EOwningState.Unowned)
					{
						return true;
					}
				}
				break;
			case ECollectionType.Skill:
				foreach (Collection collection in SkillCollectionList)
				{
					if (collection.CollectionInfo.OwningState == EOwningState.Owned)
					{
						continue;
					}

					if (!collection.CollectionInfo.NeedCollectionDict.TryGetValue(templateID, out EOwningState owningState))
					{
						continue;
					}

					if (owningState == EOwningState.Unowned)
					{
						return true;
					}
				}
				break;

			default:
				break;
		}

		return false;
	}

	public void RegistCollection(ECollectionType collectionType, int templateID)
	{
		switch (collectionType)
		{
			case ECollectionType.Item:
				Managers.Backend.GameData.Collection.AddNeedCollectionDict(ECollectionType.Item, templateID);
				break;
			case ECollectionType.Buddy:
				Managers.Backend.GameData.Collection.AddNeedCollectionDict(ECollectionType.Buddy, templateID);
				break;
			case ECollectionType.Skill:
				Managers.Backend.GameData.Collection.AddNeedCollectionDict(ECollectionType.Skill, templateID);
				break;

			default:
				break;
		}

		NewEquipment = null;

		UpdateCollectionStat();

		if (Managers.Object.Hero != null)
		{
			Managers.Object.Hero.GetComponent<HeroStatus>().UpdateCollectionStat();
		}
	}

	#endregion

	#region Mission

	public List<Mission> MissionList { get; private set; } = new List<Mission>();
	public List<Mission> NormalMissionList => MissionList.Where(mission => mission.MissionData.MissionType == EMissionType.Normal).ToList();
	public List<Mission> DayMissionList => MissionList.Where(mission => mission.MissionData.MissionType == EMissionType.Day).ToList();
	public List<Mission> WeekMissionList => MissionList.Where(mission => mission.MissionData.MissionType == EMissionType.Week).ToList();

	public void MissionInit()
	{
		foreach (var missionInfo in Managers.Backend.GameData.Mission.NormalMissionInfoDict)
		{
			MissionList.Add(missionInfo.Value.TemplateID == 0 ? null : new Mission(missionInfo.Value));
		}

		foreach (var missionInfo in Managers.Backend.GameData.Mission.DayMissionInfoDict)
		{
			MissionList.Add(missionInfo.Value.TemplateID == 0 ? null : new Mission(missionInfo.Value));
		}

		foreach (var missionInfo in Managers.Backend.GameData.Mission.WeekMissionInfoDict)
		{
			MissionList.Add(missionInfo.Value.TemplateID == 0 ? null : new Mission(missionInfo.Value));
		}
	}

	#endregion

	#region Shop



	#endregion
}
