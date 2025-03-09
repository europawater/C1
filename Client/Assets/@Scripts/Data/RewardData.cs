using LitJson;
using UnityEngine;

public class RewardData
{
	public int TemplateID { get; private set; }
	public string Remark { get; private set; }
	public string NameKey { get; private set; }

	public RewardData(JsonData json)
	{
		TemplateID = int.Parse(json["TemplateID"].ToString());
		Remark = json["Remark"].ToString();
		NameKey = json["NameKey"].ToString();
	}
}
