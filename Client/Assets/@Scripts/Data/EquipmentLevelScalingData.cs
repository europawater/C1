using LitJson;

public class EquipmentLevelScalingData
{
	public int TemplateID { get; private set; }
	public string Remark { get; private set; }
	public int IncreaseAttack { get; private set; }
	public int IncreaseDefense { get; private set; }
	public int IncreaseMaxHP { get; private set; }
	public float IncreaseCriticalValue { get; private set; }
	public float IncreaseCriticalRate { get; private set; }
	public float IncreaseSkillDamageValue { get; private set; }
	public float IncreaseSkillCriticalValue { get; private set; }
	public float IncreaseDodgeRate { get; private set; }
	public float IncreaseComboAttackRate { get; private set; }
	public float IncreaseCounterAttackRate { get; private set; }
	public float IncreaseBossExtraValue { get; private set; }

	public EquipmentLevelScalingData(JsonData json)
	{
		TemplateID = int.Parse(json["TemplateID"].ToString());
		Remark = json["Remark"].ToString();
		IncreaseAttack = int.Parse(json["IncreaseAttack"].ToString());
		IncreaseDefense = int.Parse(json["IncreaseDefense"].ToString());
		IncreaseMaxHP = int.Parse(json["IncreaseMaxHP"].ToString());
		IncreaseCriticalValue = float.Parse(json["IncreaseCriticalValue"].ToString());
		IncreaseCriticalRate = float.Parse(json["IncreaseCriticalRate"].ToString());
		IncreaseSkillDamageValue = float.Parse(json["IncreaseSkillDamageValue"].ToString());
		IncreaseSkillCriticalValue = float.Parse(json["IncreaseSkillCriticalValue"].ToString());
		IncreaseDodgeRate = float.Parse(json["IncreaseDodgeRate"].ToString());
		IncreaseComboAttackRate = float.Parse(json["IncreaseComboAttackRate"].ToString());
		IncreaseCounterAttackRate = float.Parse(json["IncreaseCounterAttackRate"].ToString());
		IncreaseBossExtraValue = float.Parse(json["IncreaseBossExtraValue"].ToString());
	}
}
