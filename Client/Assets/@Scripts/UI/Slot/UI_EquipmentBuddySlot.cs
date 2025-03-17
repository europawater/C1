using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_EquipmentBuddySlot : UI_Slot
{
	private enum GameObjects
	{
		StarArea,
	}

	private enum Images
	{
		Image_BuddyIcon,
		Image_Empty,
		Image_BattleLabel,
	}

	private int _slotIndex;
	private BuddyInfo _buddyInfo;
	private BuddyData _buddyData;

	private List<Image> _buddyLevelImageList = new List<Image>();

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindImages(typeof(Images));

		// Init
		_buddyLevelImageList.Clear();
		foreach (Transform child in GetGameObject((int)GameObjects.StarArea).transform)
		{
			_buddyLevelImageList.Add(child.GetComponent<Image>());
		}

		// Bind Event
		gameObject.BindEvent(OnClickEquipmentBuddySlot);
	}

	private void OnEnable()
	{
		Managers.Event.AddEvent(EEventType.OnSelectedBuddySlotIndex, RefreshUI);
	}

	private void OnDisable()
	{
		Managers.Event.RemoveEvent(EEventType.OnSelectedBuddySlotIndex, RefreshUI);
	}

	public void SetInfo(int slotIndex, BuddyInfo buddyInfo)
	{
		_slotIndex = slotIndex;
		_buddyInfo = buddyInfo;
		_buddyData = _buddyInfo == null ? null : Managers.Backend.Chart.BuddyChart.DataDict[_buddyInfo.TemplateID];

		RefreshUI();
	}

	private void RefreshUI()
	{
		if (_buddyInfo == null)
		{
			GetImage((int)Images.Image_Empty).gameObject.SetActive(true);
			GetImage((int)Images.Image_BattleLabel).gameObject.SetActive(false);
			return;
		}

		foreach (Image starImage in _buddyLevelImageList)
		{
			starImage.gameObject.SetActive(false);
		}
		for (int index = 0; index < _buddyInfo.Level; index++)
		{
			_buddyLevelImageList[index].gameObject.SetActive(true);
		}

		GetImage((int)Images.Image_Empty).gameObject.SetActive(false);
		GetImage((int)Images.Image_BuddyIcon).sprite = Managers.Resource.Load<Sprite>(_buddyData.IconKey);
		GetImage((int)Images.Image_BattleLabel).gameObject.SetActive(_buddyInfo.IsEquipped);
	}

	#region UI Event

	private void OnClickEquipmentBuddySlot(PointerEventData data)
	{
		if (Managers.Game.SelectedBuddySlotIndex != null)
		{
			return;
		}

		if (!Managers.Game.EquippedBuddyInfoSlotDict.ContainsKey((EBuddySlot)_slotIndex))
		{
			return;
		}

		Managers.Backend.GameData.Buddy.EquipBuddy((EBuddySlot)_slotIndex, 0);
		Managers.Game.SelectedBuddySlotIndex = null;
	}

	#endregion
}
