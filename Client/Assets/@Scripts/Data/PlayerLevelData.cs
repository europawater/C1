using LitJson;
using UnityEngine;

public class PlayerLevelData
{
	public int TemplateID { get; private set; }
	public string Remark { get; private set; }
	public int NeedToLevelUpEXP { get; private set; }

	public PlayerLevelData(JsonData json)
	{
		TemplateID = int.Parse(json["TemplateID"].ToString());
		Remark = json["Remark"].ToString();
		NeedToLevelUpEXP = int.Parse(json["NeedToLevelUpEXP"].ToString());
	}
}
