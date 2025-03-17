using System.Collections.Generic;
using UnityEngine;

public class UI_CollectionBuddySlot : UI_Slot
{
	private enum GameObjects
	{
		BuddyArea,
	}

	private enum Texts
	{
		Text_CollectionBuddyTitle,
		Text_CollectionBuddyStats,
	}

	private List<UI_BuddySlot> _slotUIList = new List<UI_BuddySlot>();

	private Collection _collection;

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindTexts(typeof(Texts));

		// Init
		_slotUIList.Clear();
		foreach (Transform child in GetGameObject((int)GameObjects.BuddyArea).transform)
		{
			UI_BuddySlot slotUI = child.GetComponent<UI_BuddySlot>();
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
		GetText((int)Texts.Text_CollectionBuddyTitle).text = $"{_collection.CollectionData.Remark}";

		string stats = string.Empty;
		foreach (var stat in _collection.ValidCollectionValueDict)
		{
			stats += $"{Util.GetStatusString(stat.Key)} {stat.Value:N0}\n";
		}
		GetText((int)Texts.Text_CollectionBuddyStats).text = stats;

		foreach (UI_BuddySlot slot in _slotUIList)
		{
			slot.gameObject.SetActive(false);
		}

		int index = 0;
		foreach (var needCollection in _collection.CollectionInfo.NeedCollectionDict)
		{
			_slotUIList[index].SetInfo(UI_BuddySlot.BuddySlotToUse.CollectionPopup, needCollection.Key, needCollection.Value);
			_slotUIList[index].gameObject.SetActive(true);
			index++;
		}
	}
}
