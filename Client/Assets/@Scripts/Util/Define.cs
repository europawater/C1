public class Define
{
	#region Core

	public enum ESound
	{
		Bgm,
		Effect,
		MaxCount,
	}

	public enum EScene
	{
		Title,
		Game,
		Dungeon,
	}

	public enum EUIEvent
	{
		Click,
		Enter,
		Exit,
	}

	public enum EEventType
	{
		// Player
		OnPlayerChanged,

		// Currency
		OnCurrencyChanged,

		// Dice
		OnDiceChanged,

		// Skill
		OnSkillChanged,
		OnSelectedSkillSlotIndex,
		OnSkillStackedTurnCountChanged,

		// Stage
		OnStageWaveIndexChanged,

		// Mastery
		OnMasteryChanged,

		// Equipment
		OnEquipmentChanged,
		OnEquipmentEnchantFail,

		// Post
		OnPostListChanged,

		// Buddy
		OnBuddyChanged,
		OnSelectedBuddySlotIndex,

		// Dungeon
		OnSelectDungeonSlotIndex,

		// Collection
		OnCollectionChanged,

		// Mission
		OnMissionChanged,
	}

	#endregion

	#region Object

	public enum EGameObjectType
	{
		None,
		Map,
		Hero,
		Buddy,
		Monster,
		SkillEffect,
		SkillCube,
		Projectile,
	}

	public enum EMapState
	{
		None,
		Stop,
		Move,
	}

	public enum EAIObjectState
	{
		None,
		Idle,
		Move,
		Attack,
		Hit,
		Dead,
	}

	public enum EMonsterType
	{
		None,
		NormalMonster,
		BossMonster,
	}

	public enum EStat
	{
		None,
		Attack,
		Defense,
		MaxHP,
		CriticalValue,
		CriticalRate,
		SkillDamageValue,
		SkillCriticalValue,
		DodgeRate,
		ComboAttackRate,
		CounterAttackRate,
		BossExtraValue,
	}

	#endregion

	#region Stage

	public enum EStageState
	{
		None,
		Start,
		Battle,
		Move,
		Over,
		Clear,
	}

	#endregion

	#region Skill

	public enum ESkillCaster
	{
		None,
		Hero,
		Monster,
		Buddy,
	}

	public enum ESkillGrade
	{
		None,
		Common,
		Advanced,
		Rare,
		Epic,
		Legend,
	}

	public enum ESkillType
	{
		None,
		NormalMelee,
		NormalRange,
		SkillSingle,
		SkillMulti,
	}

	public enum ESkillEffectType
	{
		Buff,
		DeBuff,

	}

	public enum ESkillSlot
	{
		SkillSlot01,
		SkillSlot02,
		SkillSlot03,
		SkillSlot04,
		SkillSlot05,
	}

	#endregion

	#region Mastery

	public enum EMasteryType
	{
		None,
		Attack,
		Defense,
		MaxHP,
	}

	#endregion

	#region Equipment

	public enum EEquipmentPart
	{
		None,
		Weapon,
		Helmet,
		Armor,
		Pants,
		Pauldrons,
		Gloves,
		Necklace,
		Ring,
	}

	public enum EEquipmentGrade
	{
		None,
		Common,
		Rare,
		Unique,
		Epic,
		Legend,
		Mythic,
		Beyond,
		Twilight,
	}

	#endregion

	#region Dice

	public enum EDiceState
	{
		None,
		Idle,
		Draw,
	}

	#endregion

	#region Common

	public enum EOwningState
	{
		None,
		Unowned,
		Owned,
	}

	public enum ECurrency
	{
		None,
		Gold,
		Diamond,
		DungeonTicket,
	}

	public enum ERewardType
	{
		None,
		Gold,
		Diamond,
		DiceCount,
	}

	#endregion

	#region Buddy

	public enum EBuddyGrade
	{
		None,
		Common,
		Rare,
		Unique,
		Epic,
		Legend,
	}

	public enum EBuddySlot
	{
		BuddySlot01,
		BuddySlot02,
	}

	#endregion

	#region Dungeon

	public enum EDungeonType
	{
		None,
		Cube,
		Gold,
		Diamond,
	}

	#endregion

	#region Collection

	public enum ECollectionType
	{
		None,
		Item,
		Buddy,
		Skill,
	}

	#endregion

	#region Mission

	public enum EMissionType
	{
		None,
		Normal,
		Day,
		Week,
	}

	public enum EMissionGoal
	{
		None,
		MonsterKill,
		ConsumGold,
	}

	#endregion

	#region Shop

	public enum EShopType
	{ 
		None,
		InApp,
		AD,
		Buddy,
		Skill,
	}

	#endregion
}
