using LitJson;
using System.Collections.Generic;
using UnityEngine;

public class CollectionChart : BaseChart<CollectionData>
{
	public Dictionary<int, CollectionData> CollectionDataDict { get; private set; } = new Dictionary<int, CollectionData>();
	public Dictionary<int, CollectionData> ItemCollectionDataDict { get; private set; } = new Dictionary<int, CollectionData>();
	public Dictionary<int, CollectionData> BuddyCollectionDataDict { get; private set; } = new Dictionary<int, CollectionData>();
	public Dictionary<int, CollectionData> SkillCollectionDataDict { get; private set; } = new Dictionary<int, CollectionData>();

	public override string GetChartFileName()
	{
		return "CollectionChart";
	}

	protected override void InitializeData(JsonData jsonData)
	{
		foreach (JsonData data in jsonData)
		{
			CollectionData collectionData = new CollectionData(data);
			CollectionDataDict.Add(collectionData.TemplateID, collectionData);
			switch (collectionData.CollectionType)
			{
				case Define.ECollectionType.Item:
					ItemCollectionDataDict.Add(collectionData.TemplateID, collectionData);
					break;
				case Define.ECollectionType.Buddy:
					BuddyCollectionDataDict.Add(collectionData.TemplateID, collectionData);
					break;
				case Define.ECollectionType.Skill:
					SkillCollectionDataDict.Add(collectionData.TemplateID, collectionData);
					break;

				default:
					break;
			}
		}
	}
}
