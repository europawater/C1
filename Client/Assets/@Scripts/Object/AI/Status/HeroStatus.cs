using System.Collections.Generic;
using UnityEngine;
using static Define;

public class HeroStatus : BaseStatus
{
	public override int Attack { get { return Mathf.FloorToInt(_defaultStatDict[EStat.Attack] + _masteryStatDict[EStat.Attack] + _equipmentStatDict[EStat.Attack]); } }
	public override int Defense { get { return Mathf.FloorToInt(_defaultStatDict[EStat.Defense] + _masteryStatDict[EStat.Defense] + _equipmentStatDict[EStat.Defense]); } }
	public override int MaxHP { get { return Mathf.FloorToInt(_defaultStatDict[EStat.MaxHP] + _masteryStatDict[EStat.MaxHP] + _equipmentStatDict[EStat.MaxHP]); } }
	public override float CriticalValue { get { return Mathf.FloorToInt(_defaultStatDict[EStat.CriticalValue] + _equipmentStatDict[EStat.CriticalValue]); } }
	public override float CriticalRate { get { return Mathf.FloorToInt(_defaultStatDict[EStat.CriticalRate] + _equipmentStatDict[EStat.CriticalRate]); } }
	public override float SkillDamageValue { get { return Mathf.FloorToInt(_defaultStatDict[EStat.SkillDamageValue] + _equipmentStatDict[EStat.SkillDamageValue]); } }
	public override float SkillCriticalValue { get { return Mathf.FloorToInt(_defaultStatDict[EStat.SkillCriticalValue] + _equipmentStatDict[EStat.SkillCriticalValue]); } }
	public override float DodgeRate { get { return Mathf.FloorToInt(_defaultStatDict[EStat.DodgeRate] + _equipmentStatDict[EStat.DodgeRate]); } }
	public override float ComboAttackRate { get { return Mathf.FloorToInt(_defaultStatDict[EStat.ComboAttackRate] + _equipmentStatDict[EStat.ComboAttackRate]); } }
	public override float CounterAttackRate { get { return Mathf.FloorToInt(_defaultStatDict[EStat.CounterAttackRate] + _equipmentStatDict[EStat.CounterAttackRate]); } }
	public override float BossExtraValue { get { return Mathf.FloorToInt(_defaultStatDict[EStat.BossExtraValue] + _equipmentStatDict[EStat.BossExtraValue]); } }

	private Dictionary<EStat, float> _defaultStatDict = new Dictionary<EStat, float>();
	private Dictionary<EStat, float> _masteryStatDict = new Dictionary<EStat, float>();
	private Dictionary<EStat, float> _equipmentStatDict = new Dictionary<EStat, float>();

	private void OnEnable()
	{
		Managers.Event.AddEvent(EEventType.OnMasteryChanged, UpdateMasteryStat);
		Managers.Event.AddEvent(EEventType.OnEquipmentChanged, UpdateEquipmentStat);
	}

	private void OnDisable()
	{
		Managers.Event.RemoveEvent(EEventType.OnMasteryChanged, UpdateMasteryStat);
		Managers.Event.RemoveEvent(EEventType.OnEquipmentChanged, UpdateEquipmentStat);
	}

	public override void SetInfo(BaseAIObject owner, int attack, int defense, int maxHP, float criticalValue, float criticalRate, float skillDamageValue, float skillCriticalValue, float dogeRate, float comboAttackRate, float counterAttackRate, float bossExtraValue)
	{
		_owner = owner;

		_defaultStatDict.Add(EStat.Attack, attack);
		_defaultStatDict.Add(EStat.Defense, defense);
		_defaultStatDict.Add(EStat.MaxHP, maxHP);
		_defaultStatDict.Add(EStat.CriticalValue, criticalValue);
		_defaultStatDict.Add(EStat.CriticalRate, criticalRate);
		_defaultStatDict.Add(EStat.SkillDamageValue, skillDamageValue);
		_defaultStatDict.Add(EStat.SkillCriticalValue, skillCriticalValue);
		_defaultStatDict.Add(EStat.DodgeRate, dogeRate);
		_defaultStatDict.Add(EStat.ComboAttackRate, comboAttackRate);
		_defaultStatDict.Add(EStat.CounterAttackRate, counterAttackRate);
		_defaultStatDict.Add(EStat.BossExtraValue, bossExtraValue);

		_masteryStatDict.Add(EStat.Attack, Managers.Backend.GameData.Mastery.MasteryValueDict[EMasteryType.Attack]);
		_masteryStatDict.Add(EStat.Defense, Managers.Backend.GameData.Mastery.MasteryValueDict[EMasteryType.Defense]);
		_masteryStatDict.Add(EStat.MaxHP, Managers.Backend.GameData.Mastery.MasteryValueDict[EMasteryType.MaxHP]);

		_equipmentStatDict.Add(EStat.Attack, Managers.Game.EquipmentValueDict[EStat.Attack]);
		_equipmentStatDict.Add(EStat.Defense, Managers.Game.EquipmentValueDict[EStat.Defense]);
		_equipmentStatDict.Add(EStat.MaxHP, Managers.Game.EquipmentValueDict[EStat.MaxHP]);
		_equipmentStatDict.Add(EStat.CriticalValue, Managers.Game.EquipmentValueDict[EStat.CriticalValue]);
		_equipmentStatDict.Add(EStat.CriticalRate, Managers.Game.EquipmentValueDict[EStat.CriticalRate]);
		_equipmentStatDict.Add(EStat.SkillDamageValue, Managers.Game.EquipmentValueDict[EStat.SkillDamageValue]);
		_equipmentStatDict.Add(EStat.SkillCriticalValue, Managers.Game.EquipmentValueDict[EStat.SkillCriticalValue]);
		_equipmentStatDict.Add(EStat.DodgeRate, Managers.Game.EquipmentValueDict[EStat.DodgeRate]);
		_equipmentStatDict.Add(EStat.ComboAttackRate, Managers.Game.EquipmentValueDict[EStat.ComboAttackRate]);
		_equipmentStatDict.Add(EStat.CounterAttackRate, Managers.Game.EquipmentValueDict[EStat.CounterAttackRate]);
		_equipmentStatDict.Add(EStat.BossExtraValue, Managers.Game.EquipmentValueDict[EStat.BossExtraValue]);

		HP = MaxHP;

		UpdateHpText();
	}

	private void UpdateMasteryStat()
	{
		_masteryStatDict[EStat.Attack] = Managers.Backend.GameData.Mastery.MasteryValueDict[EMasteryType.Attack];
		_masteryStatDict[EStat.Defense] = Managers.Backend.GameData.Mastery.MasteryValueDict[EMasteryType.Defense];
		_masteryStatDict[EStat.MaxHP] = Managers.Backend.GameData.Mastery.MasteryValueDict[EMasteryType.MaxHP];
	}

	private void UpdateEquipmentStat()
	{
		_equipmentStatDict[EStat.Attack] = Managers.Game.EquipmentValueDict[EStat.Attack];
		_equipmentStatDict[EStat.Defense] = Managers.Game.EquipmentValueDict[EStat.Defense];
		_equipmentStatDict[EStat.MaxHP] = Managers.Game.EquipmentValueDict[EStat.MaxHP];
		_equipmentStatDict[EStat.CriticalValue] = Managers.Game.EquipmentValueDict[EStat.CriticalValue];
		_equipmentStatDict[EStat.CriticalRate] = Managers.Game.EquipmentValueDict[EStat.CriticalRate];
		_equipmentStatDict[EStat.SkillDamageValue] = Managers.Game.EquipmentValueDict[EStat.SkillDamageValue];
		_equipmentStatDict[EStat.SkillCriticalValue] = Managers.Game.EquipmentValueDict[EStat.SkillCriticalValue];
		_equipmentStatDict[EStat.DodgeRate] = Managers.Game.EquipmentValueDict[EStat.DodgeRate];
		_equipmentStatDict[EStat.ComboAttackRate] = Managers.Game.EquipmentValueDict[EStat.ComboAttackRate];
		_equipmentStatDict[EStat.CounterAttackRate] = Managers.Game.EquipmentValueDict[EStat.CounterAttackRate];
		_equipmentStatDict[EStat.BossExtraValue] = Managers.Game.EquipmentValueDict[EStat.BossExtraValue];
	}
}
