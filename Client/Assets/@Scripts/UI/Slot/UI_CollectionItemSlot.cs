using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_CollectionItemSlot : UI_Slot
{
	private enum GameObjects
	{ 
		ItemArea,
	}

	private enum Texts
	{
		Text_CollectionItemTitle,
		Text_CollectionItemStats,
	}

	private List<UI_EquipmentSlot> _slotUIList = new List<UI_EquipmentSlot>();

	private Collection _collection;

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindTexts(typeof(Texts));

		// Init
		_slotUIList.Clear();
		foreach (Transform child in GetGameObject((int)GameObjects.ItemArea).transform)
		{
			UI_EquipmentSlot slotUI = child.GetComponent<UI_EquipmentSlot>();
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
		GetText((int)Texts.Text_CollectionItemTitle).text = $"{_collection.CollectionData.Remark}";

		string stats = string.Empty;
		foreach (var stat in _collection.ValidCollectionValueDict)
		{
			stats += $"{Util.GetStatusString(stat.Key)} {stat.Value:N0}\n";
		}
		GetText((int)Texts.Text_CollectionItemStats).text = stats;

		foreach (UI_EquipmentSlot slot in _slotUIList)
		{
			slot.gameObject.SetActive(false);
		}

		int index = 0;
		foreach (var needCollection in _collection.CollectionInfo.NeedCollectionDict)
		{
			_slotUIList[index].SetInfo(UI_EquipmentSlot.EquipmentSlotToUse.CollectionPopup, needCollection.Key, needCollection.Value);
			_slotUIList[index].gameObject.SetActive(true);
			index++;
		}
	}
}
