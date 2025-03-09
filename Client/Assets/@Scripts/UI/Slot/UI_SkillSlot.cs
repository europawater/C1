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

	private enum Images
	{
		Image_SkillIcon,
		Image_EquippedIcon,
		Image_Selected,
	}

	private SkillInfo _skillInfo;
	private SkillData _skillData;

	private List<Image> _skillLevelImageList = new List<Image>();

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindImages(typeof(Images));

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

	public void SetInfo(SkillInfo skillInfo)
	{
		_skillInfo = skillInfo;
		_skillData = Managers.Backend.Chart.SkillChart.HeroSkillDataDict[_skillInfo.TemplateID];

		RefreshUI();
	}

	private void RefreshUI()
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
		GetImage((int)Images.Image_EquippedIcon).gameObject.SetActive(_skillInfo.IsEquipped);
		GetImage((int)Images.Image_Selected).gameObject.SetActive(Managers.Game.SelectedSkillSlotIndex == _skillInfo.TemplateID);
	}

	#region UI Event

	private void OnClickSkillSlot(PointerEventData data)
	{
		Managers.Game.SelectedSkillSlotIndex = _skillInfo.TemplateID;
	}

	#endregion
}
