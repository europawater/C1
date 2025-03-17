using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_SkillSlot : UI_Slot
{
	private enum GameObjects
	{
		GradeStarArea,
	}

	private enum Texts
	{
		Text_Exp,
	}

	private enum Images
	{
		Image_SkillIcon,
		Image_EquippedIcon,
		Image_Selected,
	}

	private enum Sliders
	{
		Slider_Exp,
	}

	public enum SkillSlotToUse
	{
		None,
		SkillPopup,
		CollectionPopup,
	}

	private SkillSlotToUse _skillSlotToUse;
	private SkillInfo _skillInfo;
	private SkillData _skillData;

	private List<Image> _skillLevelImageList = new List<Image>();

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
		_skillLevelImageList.Clear();
		foreach (Transform child in GetGameObject((int)GameObjects.GradeStarArea).transform)
		{
			_skillLevelImageList.Add(child.GetComponentInChildren<Image>());
		}

		// Bind Event
		gameObject.BindEvent(OnClickSkillSlot);
	}

	private void OnEnable()
	{
		Managers.Event.AddEvent(EEventType.OnSelectedSkillSlotIndex, RefreshUI);
		Managers.Event.AddEvent(EEventType.OnSkillChanged, RefreshUI);
	}

	private void OnDisable()
	{
		Managers.Event.RemoveEvent(EEventType.OnSelectedSkillSlotIndex, RefreshUI);
		Managers.Event.RemoveEvent(EEventType.OnSkillChanged, RefreshUI);
	}

	public void SetInfo(SkillSlotToUse skillSlotToUse, SkillInfo skillInfo)
	{
		_skillSlotToUse = skillSlotToUse;
		_skillInfo = skillInfo;
		_skillData = Managers.Backend.Chart.SkillChart.HeroSkillDataDict[_skillInfo.TemplateID];

		RefreshUI();
	}

	public void SetInfo(SkillSlotToUse skillSlotToUse, int templateID, EOwningState owningState)
	{
		_skillSlotToUse = skillSlotToUse;
		_skillData = Managers.Backend.Chart.SkillChart.HeroSkillDataDict[templateID];
		_owningState = owningState;

		RefreshUI();
	}

	private void RefreshUI()
	{
		switch (_skillSlotToUse)
		{
			case SkillSlotToUse.SkillPopup:
				RefreshBySkillPopup();
				break;
			case SkillSlotToUse.CollectionPopup:
				RefreshByCollectionPopup();
				break;

			default:
				break;
		}
	}

	private void RefreshBySkillPopup()
	{
		if (_skillInfo == null)
		{
			return;
		}

		foreach (Image starImage in _skillLevelImageList)
		{
			starImage.gameObject.SetActive(false);
		}
		for (int index = 0; index < _skillInfo.Level; index++)
		{
			_skillLevelImageList[index].gameObject.SetActive(true);
		}

		GetImage((int)Images.Image_SkillIcon).sprite = Managers.Resource.Load<Sprite>(_skillData.IconKey);
		GetImage((int)Images.Image_SkillIcon).color = _skillInfo.OwningState == EOwningState.Owned ? Color.white : Color.gray;
		GetImage((int)Images.Image_EquippedIcon).gameObject.SetActive(_skillInfo.IsEquipped);
		GetImage((int)Images.Image_Selected).gameObject.SetActive(Managers.Game.SelectedSkillSlotIndex == _skillInfo.TemplateID);
		GetText((int)Texts.Text_Exp).text = $"{_skillInfo.PieceCount:N0}/{_skillData.LevelUpPiece:N0}";
		GetSlider((int)Sliders.Slider_Exp).value = (float)_skillInfo.PieceCount / _skillData.LevelUpPiece;
		GetSlider((int)Sliders.Slider_Exp).gameObject.SetActive(true);
	}

	private void RefreshByCollectionPopup()
	{
		foreach (Image starImage in _skillLevelImageList)
		{
			starImage.gameObject.SetActive(false);
		}
		for (int index = 0; index < 1; index++)
		{
			_skillLevelImageList[index].gameObject.SetActive(true);
		}

		GetImage((int)Images.Image_SkillIcon).sprite = Managers.Resource.Load<Sprite>(_skillData.IconKey);
		GetImage((int)Images.Image_SkillIcon).color = _owningState == EOwningState.Owned ? Color.white : Color.gray;
		GetImage((int)Images.Image_EquippedIcon).gameObject.SetActive(false);
		GetImage((int)Images.Image_Selected).gameObject.SetActive(false);
		GetSlider((int)Sliders.Slider_Exp).gameObject.SetActive(false);
	}

	#region UI Event

	private void OnClickSkillSlot(PointerEventData data)
	{
		if (_skillInfo.OwningState != EOwningState.Owned)
		{
			return;		
		}

		if (_skillSlotToUse == SkillSlotToUse.SkillPopup)
		{
			Managers.Game.SelectedSkillSlotIndex = _skillInfo.TemplateID;
		}
	}

	#endregion
}
