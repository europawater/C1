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
        CreateSkill,
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

		// Post
		OnPostListChanged,
	}

	#endregion

	#region Object

	public enum EGameObjectType
    { 
        None,
        Map,
        Hero,
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
    }

    #endregion
}
