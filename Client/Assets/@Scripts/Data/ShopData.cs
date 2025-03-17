using LitJson;
using System;
using UnityEngine;
using static Define;

public class ShopData
{
	public int TemplateID { get; private set; }
	public string Remark { get; private set; }
	public string NameKey { get; private set; }
	public string IconKey { get; private set; }
	public EShopType ShopType { get; private set; }
	public int PurchaseValue { get; private set; }
	public int RewardValue { get; private set; }

	public ShopData(JsonData json)
	{
		TemplateID = int.Parse(json["TemplateID"].ToString());
		Remark = json["Remark"].ToString();
		NameKey = json["NameKey"].ToString();
		IconKey = json["IconKey"].ToString();
		ShopType = (EShopType)Enum.Parse(typeof(EShopType), json["ShopType"].ToString());
		PurchaseValue = int.Parse(json["PurchaseValue"].ToString());
		RewardValue = int.Parse(json["RewardValue"].ToString());
	}
}
