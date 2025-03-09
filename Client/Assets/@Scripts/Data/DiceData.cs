using LitJson;

public class DiceData
{
	public int TemplateID { get; private set; }
	public string Remark { get; private set; }
	public string NameKey { get; private set; }
	public string SkeletondataKey { get; private set; }
	public float CommonRate { get; private set; }
	public float RareRate { get; private set; }
	public float UniqueRate { get; private set; }
	public float EpicRate { get; private set; }
	public float LegendRate { get; private set; }
	public float MythicRate { get; private set; }
	public float BeyondRate { get; private set; }
	public float TwilightRate { get; private set; }

	public DiceData(JsonData json)
	{
		TemplateID = int.Parse(json["TemplateID"].ToString());
		Remark = json["Remark"].ToString();
		NameKey = json["NameKey"].ToString();
		SkeletondataKey = json["SkeletondataKey"].ToString();
		CommonRate = float.Parse(json["CommonRate"].ToString());
		RareRate = float.Parse(json["RareRate"].ToString());
		UniqueRate = float.Parse(json["UniqueRate"].ToString());
		EpicRate = float.Parse(json["EpicRate"].ToString());
		LegendRate = float.Parse(json["LegendRate"].ToString());
		MythicRate = float.Parse(json["MythicRate"].ToString());
		BeyondRate = float.Parse(json["BeyondRate"].ToString());
		TwilightRate = float.Parse(json["TwilightRate"].ToString());
	}
}
