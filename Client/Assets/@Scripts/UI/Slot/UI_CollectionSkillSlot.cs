using System.Collections.Generic;
using UnityEngine;

public class UI_CollectionSkillSlot : UI_Slot
{
	private enum GameObjects
	{
		SkillArea,
	}

	private enum Texts
	{
		Text_CollectionSkillTitle,
		Text_CollectionSkillStats,
	}

	private List<UI_SkillSlot> _slotUIList = new List<UI_SkillSlot>();

	private Collection _collection;

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindTexts(typeof(Texts));

		// Init
		_slotUIList.Clear();
		foreach (Transform child in GetGameObject((int)GameObjects.SkillArea).transform)
		{
			UI_SkillSlot slotUI = child.GetComponent<UI_SkillSlot>();
			_slotUIList.Add(slotUI);
		}
	}

	public void SetInfo(Collection collection)
	{
		_collection = collection;

		RefreshUI();
	}

	private void RefreshUI()
	{
		GetText((int)Texts.Text_CollectionSkillTitle).text = $"{_collection.CollectionData.Remark}";

		string stats = string.Empty;
		foreach (var stat in _collection.ValidCollectionValueDict)
		{
			stats += $"{Util.GetStatusString(stat.Key)} {stat.Value:N0}\n";
		}
		GetText((int)Texts.Text_CollectionSkillStats).text = stats;

		foreach (UI_SkillSlot slot in _slotUIList)
		{
			slot.gameObject.SetActive(false);
		}

		int index = 0;
		foreach (var needCollection in _collection.CollectionInfo.NeedCollectionDict)
		{
			_slotUIList[index].SetInfo(UI_SkillSlot.SkillSlotToUse.CollectionPopup, needCollection.Key, needCollection.Value);
			_slotUIList[index].gameObject.SetActive(true);
			index++;
		}
	}
}
