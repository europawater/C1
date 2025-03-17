using BackEnd;
using LitJson;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CollectionGameData : BaseGameData
{
    public Dictionary<int, CollectionInfo> ItemCollectionInfoDict { get; private set; } = new Dictionary<int, CollectionInfo>();
    public Dictionary<int, CollectionInfo> BuddyCollectionInfoDict { get; private set; } = new Dictionary<int, CollectionInfo>();
    public Dictionary<int, CollectionInfo> SkillCollectionInfoDict { get; private set; } = new Dictionary<int, CollectionInfo>();

    public override string GetTableName()
    {
        return "Collection";
    }

    public override Param GetParam()
    {
        Param param = new Param();

        param.Add("ItemCollectionInfoDict", ItemCollectionInfoDict);
        param.Add("BuddyCollectionInfoDict", BuddyCollectionInfoDict);
        param.Add("SkillCollectionInfoDict", SkillCollectionInfoDict);

        return param;
    }

    protected override void InitializeData()
    {
        ItemCollectionInfoDict.Clear();
        foreach (CollectionData data in Managers.Backend.Chart.CollectionChart.ItemCollectionDataDict.Values)
        {
            Dictionary<int, EOwningState> needCollectionDict = new Dictionary<int, EOwningState>();
            foreach (int key in data.NeedCollectionList)
            {
                needCollectionDict.Add(key, EOwningState.Unowned);
            }

            CollectionInfo info = new CollectionInfo(data.TemplateID, needCollectionDict, EOwningState.Unowned);
            ItemCollectionInfoDict.Add(data.TemplateID, info);
        }

        BuddyCollectionInfoDict.Clear();
        foreach (CollectionData data in Managers.Backend.Chart.CollectionChart.BuddyCollectionDataDict.Values)
        {
            Dictionary<int, EOwningState> needCollectionDict = new Dictionary<int, EOwningState>();
            foreach (int key in data.NeedCollectionList)
            {
                needCollectionDict.Add(key, EOwningState.Unowned);
            }

            CollectionInfo info = new CollectionInfo(data.TemplateID, needCollectionDict, EOwningState.Unowned);
            BuddyCollectionInfoDict.Add(data.TemplateID, info);
        }

        SkillCollectionInfoDict.Clear();
        foreach (CollectionData data in Managers.Backend.Chart.CollectionChart.SkillCollectionDataDict.Values)
        {
            Dictionary<int, EOwningState> needCollectionDict = new Dictionary<int, EOwningState>();
            foreach (int key in data.NeedCollectionList)
            {
                needCollectionDict.Add(key, EOwningState.Unowned);
            }

            CollectionInfo info = new CollectionInfo(data.TemplateID, needCollectionDict, EOwningState.Unowned);
            SkillCollectionInfoDict.Add(data.TemplateID, info);
        }

        Managers.Game.CollectionInit();
    }

    protected override void SetServerDataToLocal(JsonData gameDataJson)
    {
        ItemCollectionInfoDict.Clear();
        foreach (JsonData key in gameDataJson["ItemCollectionInfoDict"].Keys)
        {
            JsonData jsonData = gameDataJson["ItemCollectionInfoDict"][key.ToString()];
            CollectionInfo info = new CollectionInfo(jsonData);
            ItemCollectionInfoDict.Add(info.TemplateID, info);
        }

        BuddyCollectionInfoDict.Clear();
        foreach (JsonData key in gameDataJson["BuddyCollectionInfoDict"].Keys)
        {
            JsonData jsonData = gameDataJson["BuddyCollectionInfoDict"][key.ToString()];
            CollectionInfo info = new CollectionInfo(jsonData);
            BuddyCollectionInfoDict.Add(info.TemplateID, info);
        }

        SkillCollectionInfoDict.Clear();
        foreach (JsonData key in gameDataJson["SkillCollectionInfoDict"].Keys)
        {
            JsonData jsonData = gameDataJson["SkillCollectionInfoDict"][key.ToString()];
            CollectionInfo info = new CollectionInfo(jsonData);
            SkillCollectionInfoDict.Add(info.TemplateID, info);
        }

        Managers.Game.CollectionInit();
    }

    protected override void UpdateData()
    {
        base.UpdateData();

        Managers.Event.TriggerEvent(EEventType.OnCollectionChanged);
    }

    #region Cotnets

    public void AddNeedCollectionDict(ECollectionType collectionType, int templateID)
    {
        switch (collectionType)
        {
            case ECollectionType.Item:
                foreach(CollectionInfo itemCollectionInfo in ItemCollectionInfoDict.Values)
                {
                    itemCollectionInfo.RegistNeedCollection(templateID);
                }
                break;
            case ECollectionType.Buddy:
                foreach (CollectionInfo buddyCollectionInfo in BuddyCollectionInfoDict.Values)
                {
                    buddyCollectionInfo.RegistNeedCollection(templateID);
                }
                break;
            case ECollectionType.Skill:
                foreach (CollectionInfo skillCollectionInfo in SkillCollectionInfoDict.Values)
                {
                    skillCollectionInfo.RegistNeedCollection(templateID);
                }
                break;

            default:
                break;
        }

        UpdateData();
    }

    #endregion
}
