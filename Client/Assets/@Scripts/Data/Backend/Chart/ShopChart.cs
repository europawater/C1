using LitJson;
using System.Collections.Generic;
using UnityEngine;

public class ShopChart : BaseChart<ShopData>
{
	public Dictionary<int, ShopData> ShopDataDict = new Dictionary<int, ShopData>();
	public Dictionary<int, ShopData> InAppAndADShopDataDict = new Dictionary<int, ShopData>();
	public Dictionary<int, ShopData> SkillShopDataDict = new Dictionary<int, ShopData>();
	public Dictionary<int, ShopData> BuddyShopDataDict = new Dictionary<int, ShopData>();

	public override string GetChartFileName()
	{
		return "ShopChart";
	}

	protected override void InitializeData(JsonData jsonData)
	{
		foreach(JsonData data in jsonData)
		{
			ShopData shopData = new ShopData(data);
			ShopDataDict.Add(shopData.TemplateID, shopData);
			switch (shopData.ShopType)
			{
				case Define.EShopType.InApp:
				case Define.EShopType.AD:
					InAppAndADShopDataDict.Add(shopData.TemplateID, shopData);
					break;
				case Define.EShopType.Skill:
					SkillShopDataDict.Add(shopData.TemplateID, shopData);
					break;
				case Define.EShopType.Buddy:
					BuddyShopDataDict.Add(shopData.TemplateID, shopData);
					break;

				default:
					break;
			}
		}
	}
}
