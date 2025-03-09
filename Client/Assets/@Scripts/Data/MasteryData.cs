using LitJson;

public class MasteryData
{
	public int TemplateID { get; private set; }
	public string Remark { get; private set; }
	public string NameKey { get; private set; }
	public int StartLevel { get; private set; }
	public int MaxLevel { get; private set; }
	public int IncreaseAttackValue { get; private set; }
	public int IncreaseDefenseValue { get; private set; }
	public int IncreaseMaxHPValue { get; private set; }
	public int StartGoldAmount { get; private set; }
	public int IncreaseGoldAmount { get; private set; }

	public MasteryData(JsonData json)
	{
		TemplateID = int.Parse(json["TemplateID"].ToString());
		Remark = json["Remark"].ToString();
		NameKey = json["NameKey"].ToString();
		StartLevel = int.Parse(json["StartLevel"].ToString());
		MaxLevel = int.Parse(json["MaxLevel"].ToString());
		IncreaseAttackValue = int.Parse(json["IncreaseAttackValue"].ToString());
		IncreaseDefenseValue = int.Parse(json["IncreaseDefenseValue"].ToString());
		IncreaseMaxHPValue = int.Parse(json["IncreaseMaxHPValue"].ToString());
		StartGoldAmount = int.Parse(json["StartGoldAmount"].ToString());
		IncreaseGoldAmount = int.Parse(json["IncreaseGoldAmount"].ToString());
	}
}
