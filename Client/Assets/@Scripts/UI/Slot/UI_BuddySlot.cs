using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;
using static UI_BuddySlot;

public class UI_BuddySlot : UI_Slot
{
	private enum GameObjects
	{
		StarArea,
		Image_UnlockedSlot,
	}

	private enum Texts
	{
		Text_Count,
	}

	private enum Images
	{
		Image_BuddyIcon,
		Image_Selected,
		Image_BattleLabel,
	}

	private enum Sliders
	{
		Slider_BuddyExp,
	}

	public enum BuddySlotToUse
	{
		None,
		BuddyPopup,
		CollectionPopup,
	}

	private BuddySlotToUse _buddySlotToUse;
	private BuddyInfo _buddyInfo;
	private BuddyData _buddyData;

	private List<Image> _buddyLevelImageList = new List<Image>();

	// Collection
	private EOwningState _owningState;

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindTexts(typeof(Texts));
		BindImages(typeof(Images));
		BindSliders(typeof(Sliders));

		// Init
		_buddyLevelImageList.Clear();
		foreach (Transform child in GetGameObject((int)GameObjects.StarArea).transform)
		{
			_buddyLevelImageList.Add(child.GetComponent<Image>());
		}

		// Bind Event
		gameObject.BindEvent(OnClickBuddySlot);
	}

	private void OnEnable()
	{
		Managers.Event.AddEvent(EEventType.OnSelectedBuddySlotIndex, RefreshUI);
		Managers.Event.AddEvent(EEventType.OnBuddyChanged, RefreshUI);
	}

	private void OnDisable()
	{
		Managers.Event.RemoveEvent(EEventType.OnSelectedBuddySlotIndex, RefreshUI);
		Managers.Event.RemoveEvent(EEventType.OnBuddyChanged, RefreshUI);
	}

	public void SetInfo(BuddySlotToUse buddySlotToUse, BuddyInfo buddyInfo)
	{
		_buddySlotToUse = buddySlotToUse;
		_buddyInfo = buddyInfo;
		_buddyData = Managers.Backend.Chart.BuddyChart.DataDict[_buddyInfo.TemplateID];

		RefreshUI();
	}

	public void SetInfo(BuddySlotToUse buddySlotToUse, int templateID, EOwningState owningState)
	{
		_buddySlotToUse = buddySlotToUse;
		_buddyData = Managers.Backend.Chart.BuddyChart.DataDict[templateID];
		_owningState = owningState;

		RefreshUI();
	}

	private void RefreshUI()
	{
		switch (_buddySlotToUse)
		{
			case BuddySlotToUse.BuddyPopup:
				RefreshByBuddyPopup();
				break;
			case BuddySlotToUse.CollectionPopup:
				RefreshByCollectionPopup();
				break;

			default:
				break;
		}
	}

	private void RefreshByBuddyPopup()
	{
		if (_buddyInfo == null)
		{
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

		GetGameObject((int)GameObjects.Image_UnlockedSlot).SetActive(_buddyInfo.OwningState != EOwningState.Owned);
		GetImage((int)Images.Image_BuddyIcon).sprite = Managers.Resource.Load<Sprite>(_buddyData.IconKey);
		GetImage((int)Images.Image_BattleLabel).gameObject.SetActive(_buddyInfo.IsEquipped);
		GetImage((int)Images.Image_Selected).gameObject.SetActive(Managers.Game.SelectedBuddySlotIndex == _buddyInfo.TemplateID);
		GetText((int)Texts.Text_Count).text = $"{_buddyInfo.PieceCount:N0}/{_buddyData.LevelUpPiece:N0}";
		GetSlider((int)Sliders.Slider_BuddyExp).value = (float)_buddyInfo.PieceCount / _buddyData.LevelUpPiece;
		GetSlider((int)Sliders.Slider_BuddyExp).gameObject.SetActive(true);
	}

	private void RefreshByCollectionPopup()
	{
		foreach (Image starImage in _buddyLevelImageList)
		{
			starImage.gameObject.SetActive(false);
		}
		for (int index = 0; index < 1; index++)
		{
			_buddyLevelImageList[index].gameObject.SetActive(true);
		}

		GetGameObject((int)GameObjects.Image_UnlockedSlot).SetActive(false);
		GetImage((int)Images.Image_BuddyIcon).sprite = Managers.Resource.Load<Sprite>(_buddyData.IconKey);
		GetImage((int)Images.Image_BuddyIcon).color = _owningState == EOwningState.Owned ? Color.white : Color.gray;
		GetImage((int)Images.Image_BattleLabel).gameObject.SetActive(false);
		GetImage((int)Images.Image_Selected).gameObject.SetActive(false);
		GetSlider((int)Sliders.Slider_BuddyExp).gameObject.SetActive(false);
	}

	#region UI Event

	private void OnClickBuddySlot(PointerEventData data)
	{
		if (_buddyInfo.OwningState != EOwningState.Owned)
		{
			return;
		}

		if (_buddySlotToUse == BuddySlotToUse.BuddyPopup)
		{
			Managers.Game.SelectedBuddySlotIndex = _buddyInfo.TemplateID;
		}
	}

	#endregion
}
