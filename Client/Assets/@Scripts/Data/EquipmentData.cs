using LitJson;
using System;
using static Define;

public class EquipmentData
{
	public int TemplateID { get; private set; }
	public string Remark { get; private set; }
	public string NameKey { get; private set; }
	public string IconKey { get; private set; }
	public string SlotKey { get; private set; }
	public EEquipmentPart EquipmentPart { get; private set; }
	public EEquipmentGrade EquipmentGrade { get; private set; }
	public int Attack { get; private set; }
	public int Defense { get; private set; }
	public int MaxHP { get; private set; }
	public float CriticalValue { get; private set; }
	public float CriticalRate { get; private set; }
	public float SkillDamageValue { get; private set; }
	public float SkillCriticalValue { get; private set; }
	public float DodgeRate { get; private set; }
	public float ComboAttackRate { get; private set; }
	public float CounterAttackRate { get; private set; }
	public float BossExtraValue { get; private set; }
	public float EnchantRate { get; private set; }

	public EquipmentData(JsonData json)
	{
		TemplateID = int.Parse(json["TemplateID"].ToString());
		Remark = json["Remark"].ToString();
		NameKey = json["NameKey"].ToString();
		IconKey = json["IconKey"].ToString();
		SlotKey = json["SlotKey"].ToString();
		EquipmentPart = (EEquipmentPart)Enum.Parse(typeof(EEquipmentPart), json["EquipmentPart"].ToString());
		EquipmentGrade = (EEquipmentGrade)Enum.Parse(typeof(EEquipmentGrade), json["EquipmentGrade"].ToString());
		Attack = int.Parse(json["Attack"].ToString());
		Defense = int.Parse(json["Defense"].ToString());
		MaxHP = int.Parse(json["MaxHP"].ToString());
		CriticalValue = float.Parse(json["CriticalValue"].ToString());
		CriticalRate = float.Parse(json["CriticalRate"].ToString());
		SkillDamageValue = float.Parse(json["SkillDamageValue"].ToString());
		SkillCriticalValue = float.Parse(json["SkillCriticalValue"].ToString());
		DodgeRate = float.Parse(json["DodgeRate"].ToString());
		ComboAttackRate = float.Parse(json["ComboAttackRate"].ToString());
		CounterAttackRate = float.Parse(json["CounterAttackRate"].ToString());
		BossExtraValue = float.Parse(json["BossExtraValue"].ToString());
		EnchantRate = float.Parse(json["EnchantRate"].ToString());
	}
}
