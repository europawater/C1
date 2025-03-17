using System.Collections.Generic;
using UnityEngine;
using static Define;

public class HeroStatus : BaseStatus
{
    public override int Attack { get { return Mathf.FloorToInt(_defaultStatDict[EStat.Attack] + _masteryStatDict[EStat.Attack] + _equipmentStatDict[EStat.Attack] + _collectionStatDict[EStat.Attack]); } }
    public override int Defense { get { return Mathf.FloorToInt(_defaultStatDict[EStat.Defense] + _masteryStatDict[EStat.Defense] + _equipmentStatDict[EStat.Defense] + _collectionStatDict[EStat.Defense]); } }
    public override int MaxHP { get { return Mathf.FloorToInt(_defaultStatDict[EStat.MaxHP] + _masteryStatDict[EStat.MaxHP] + _equipmentStatDict[EStat.MaxHP] + _collectionStatDict[EStat.MaxHP]); } }
    public override float CriticalValue { get { return Mathf.FloorToInt(_defaultStatDict[EStat.CriticalValue] + _equipmentStatDict[EStat.CriticalValue] + _collectionStatDict[EStat.CriticalValue]); } }
    public override float CriticalRate { get { return Mathf.FloorToInt(_defaultStatDict[EStat.CriticalRate] + _equipmentStatDict[EStat.CriticalRate] + _collectionStatDict[EStat.CriticalRate]); } }
    public override float SkillDamageValue { get { return Mathf.FloorToInt(_defaultStatDict[EStat.SkillDamageValue] + _equipmentStatDict[EStat.SkillDamageValue] + _collectionStatDict[EStat.SkillDamageValue]); } }
    public override float SkillCriticalValue { get { return Mathf.FloorToInt(_defaultStatDict[EStat.SkillCriticalValue] + _equipmentStatDict[EStat.SkillCriticalValue] + _collectionStatDict[EStat.SkillCriticalValue]); } }
    public override float DodgeRate { get { return Mathf.FloorToInt(_defaultStatDict[EStat.DodgeRate] + _equipmentStatDict[EStat.DodgeRate] + _collectionStatDict[EStat.DodgeRate]); } }
    public override float ComboAttackRate { get { return Mathf.FloorToInt(_defaultStatDict[EStat.ComboAttackRate] + _equipmentStatDict[EStat.ComboAttackRate] + _collectionStatDict[EStat.ComboAttackRate]); } }
    public override float CounterAttackRate { get { return Mathf.FloorToInt(_defaultStatDict[EStat.CounterAttackRate] + _equipmentStatDict[EStat.CounterAttackRate] + _collectionStatDict[EStat.CounterAttackRate]); } }
    public override float BossExtraValue { get { return Mathf.FloorToInt(_defaultStatDict[EStat.BossExtraValue] + _equipmentStatDict[EStat.BossExtraValue] + _collectionStatDict[EStat.BossExtraValue]); } }

    private Dictionary<EStat, float> _defaultStatDict = new Dictionary<EStat, float>();
    private Dictionary<EStat, float> _masteryStatDict = new Dictionary<EStat, float>();
    private Dictionary<EStat, float> _equipmentStatDict = new Dictionary<EStat, float>();
    private Dictionary<EStat, float> _collectionStatDict = new Dictionary<EStat, float>();

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

        _collectionStatDict.Add(EStat.Attack, Managers.Game.CollectionValueDict[EStat.Attack]);
        _collectionStatDict.Add(EStat.Defense, Managers.Game.CollectionValueDict[EStat.Defense]);
        _collectionStatDict.Add(EStat.MaxHP, Managers.Game.CollectionValueDict[EStat.MaxHP]);
        _collectionStatDict.Add(EStat.CriticalValue, Managers.Game.CollectionValueDict[EStat.CriticalValue]);
        _collectionStatDict.Add(EStat.CriticalRate, Managers.Game.CollectionValueDict[EStat.CriticalRate]);
        _collectionStatDict.Add(EStat.SkillDamageValue, Managers.Game.CollectionValueDict[EStat.SkillDamageValue]);
        _collectionStatDict.Add(EStat.SkillCriticalValue, Managers.Game.CollectionValueDict[EStat.SkillCriticalValue]);
        _collectionStatDict.Add(EStat.DodgeRate, Managers.Game.CollectionValueDict[EStat.DodgeRate]);
        _collectionStatDict.Add(EStat.ComboAttackRate, Managers.Game.CollectionValueDict[EStat.ComboAttackRate]);
        _collectionStatDict.Add(EStat.CounterAttackRate, Managers.Game.CollectionValueDict[EStat.CounterAttackRate]);
        _collectionStatDict.Add(EStat.BossExtraValue, Managers.Game.CollectionValueDict[EStat.BossExtraValue]);

        HP = MaxHP;

        UpdateHpText();
    }

    public void UpdateMasteryStat()
    {
        _masteryStatDict[EStat.Attack] = Managers.Backend.GameData.Mastery.MasteryValueDict[EMasteryType.Attack];
        _masteryStatDict[EStat.Defense] = Managers.Backend.GameData.Mastery.MasteryValueDict[EMasteryType.Defense];
        _masteryStatDict[EStat.MaxHP] = Managers.Backend.GameData.Mastery.MasteryValueDict[EMasteryType.MaxHP];
    }

    public void UpdateEquipmentStat()
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

    public void UpdateCollectionStat()
    {
        _collectionStatDict[EStat.Attack] = Managers.Game.CollectionValueDict[EStat.Attack];
        _collectionStatDict[EStat.Defense] = Managers.Game.CollectionValueDict[EStat.Defense];
        _collectionStatDict[EStat.MaxHP] = Managers.Game.CollectionValueDict[EStat.MaxHP];
        _collectionStatDict[EStat.CriticalValue] = Managers.Game.CollectionValueDict[EStat.CriticalValue];
        _collectionStatDict[EStat.CriticalRate] = Managers.Game.CollectionValueDict[EStat.CriticalRate];
        _collectionStatDict[EStat.SkillDamageValue] = Managers.Game.CollectionValueDict[EStat.SkillDamageValue];
        _collectionStatDict[EStat.SkillCriticalValue] = Managers.Game.CollectionValueDict[EStat.SkillCriticalValue];
        _collectionStatDict[EStat.DodgeRate] = Managers.Game.CollectionValueDict[EStat.DodgeRate];
        _collectionStatDict[EStat.ComboAttackRate] = Managers.Game.CollectionValueDict[EStat.ComboAttackRate];
        _collectionStatDict[EStat.CounterAttackRate] = Managers.Game.CollectionValueDict[EStat.CounterAttackRate];
        _collectionStatDict[EStat.BossExtraValue] = Managers.Game.CollectionValueDict[EStat.BossExtraValue];
    }
}
