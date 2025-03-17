using LitJson;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static Define;

/// <summary>
/// 컬렉션 정보를 담는 클래스입니다.
/// 뒤끝에 저장되는 데이터입니다.
/// </summary>
[SerializeField]
public class CollectionInfo
{
	public int TemplateID { get; private set; }
	public Dictionary<int, EOwningState> NeedCollectionDict { get; private set; } = new Dictionary<int, EOwningState>();
	public EOwningState OwningState { get; private set; }

	public CollectionInfo(int templateID, Dictionary<int, EOwningState> needCollectionDict, EOwningState owningState)
	{
		TemplateID = templateID;
		NeedCollectionDict = needCollectionDict;
		OwningState = owningState;
	}
	
	public CollectionInfo(JsonData jsonData)
	{
		TemplateID = int.Parse(jsonData["TemplateID"].ToString());
		NeedCollectionDict = new Dictionary<int, EOwningState>();
		foreach (JsonData key in jsonData["NeedCollectionDict"].Keys)
		{
			JsonData needCollectionJsonData = jsonData["NeedCollectionDict"][key.ToString()];
			NeedCollectionDict.Add(int.Parse(key.ToString()), (EOwningState)System.Enum.Parse(typeof(EOwningState), needCollectionJsonData.ToString()));
		}

		OwningState = (EOwningState)System.Enum.Parse(typeof(EOwningState), jsonData["OwningState"].ToString());
	}

	public void RegistNeedCollection(int templateID)
	{
		if (!NeedCollectionDict.ContainsKey(templateID))
        {
            return;
        }

		NeedCollectionDict[templateID] = EOwningState.Owned;

		CheckOwningState();
    }

	private void CheckOwningState()
	{
		foreach(EOwningState owningState in NeedCollectionDict.Values)
        {
            if (owningState == EOwningState.Unowned)
            {
                return;
            }
        }

        OwningState = EOwningState.Owned;
	}
}
