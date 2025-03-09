using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_EquipmentSkillSlot : UI_Slot
{
	private enum Images
	{
		Image_SkillIcon,
		Image_Selected,
	}

	private enum Texts
	{
		Text_SkillCount,
	}

	public enum EquipmentSkillSlotToUse
	{
		None,
		GameScene,
		HeroPopup,
	}

	private int _slotIndex;
	private EquipmentSkillSlotToUse _equipmentSkillSlotToUse = EquipmentSkillSlotToUse.None;
	private Skill _skill;

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindImages(typeof(Images));
		BindTexts(typeof(Texts));

		// Bind Event
		gameObject.BindEvent(OnClickEquipmentSkillSlot);
	}

	private void OnEnable()
	{
		Managers.Event.AddEvent(EEventType.OnSelectedSkillSlotIndex, RefreshUI);
		Managers.Event.AddEvent(EEventType.OnSkillStackedTurnCountChanged, RefreshUI);
	}

	private void OnDisable()
	{
		Managers.Event.RemoveEvent(EEventType.OnSelectedSkillSlotIndex, RefreshUI);
		Managers.Event.RemoveEvent(EEventType.OnSkillStackedTurnCountChanged, RefreshUI);
	}

	public void SetInfo(int slotIndex, EquipmentSkillSlotToUse equipmentSkillSlotToUse, Skill skill)
	{
		_slotIndex = slotIndex;
		_equipmentSkillSlotToUse = equipmentSkillSlotToUse;
		_skill = skill;

		RefreshUI();
	}
	
	private void RefreshUI()
	{
		switch (_equipmentSkillSlotToUse)
		{
			case EquipmentSkillSlotToUse.GameScene:
				RefreshByGameScene();
				break;
			case EquipmentSkillSlotToUse.HeroPopup:
				RefreshByHeroPopup();
				break;
		}
	}

	private void RefreshByGameScene()
	{
		if (_skill == null)
		{
			GetImage((int)Images.Image_SkillIcon).gameObject.SetActive(false);
			GetImage((int)Images.Image_Selected).gameObject.SetActive(false);
			GetText((int)Texts.Text_SkillCount).gameObject.SetActive(false);
		}
		else
		{
			GetImage((int)Images.Image_SkillIcon).sprite = Managers.Resource.Load<Sprite>(_skill.SkillData.IconKey);
			GetText((int)Texts.Text_SkillCount).text = $"{_skill.StackedTurnCount}/{_skill.NeedTurnCount}";

			GetImage((int)Images.Image_SkillIcon).gameObject.SetActive(true);
			GetImage((int)Images.Image_Selected).gameObject.SetActive(false);
			GetText((int)Texts.Text_SkillCount).gameObject.SetActive(true);
		}
	}

	private void RefreshByHeroPopup()
	{
		if (_skill == null)
		{
			GetImage((int)Images.Image_SkillIcon).gameObject.SetActive(false);
			GetImage((int)Images.Image_Selected).gameObject.SetActive(false);
			GetText((int)Texts.Text_SkillCount).gameObject.SetActive(false);
		}
		else
		{
			GetImage((int)Images.Image_SkillIcon).sprite = Managers.Resource.Load<Sprite>(_skill.SkillData.IconKey);

			GetImage((int)Images.Image_SkillIcon).gameObject.SetActive(true);
			GetText((int)Texts.Text_SkillCount).gameObject.SetActive(false);
		}

		GetImage((int)Images.Image_Selected).gameObject.SetActive(Managers.Game.SelectedSkillSlotIndex != null);
	}

	#region UI Event

	private void OnClickEquipmentSkillSlot(PointerEventData data)
	{
		switch (_equipmentSkillSlotToUse)
		{
			case EquipmentSkillSlotToUse.GameScene:
				break;
			case EquipmentSkillSlotToUse.HeroPopup:
				if (Managers.Game.SelectedSkillSlotIndex != null)
				{
					Managers.Backend.GameData.Skill.EquipSkill((ESkillSlot)_slotIndex, (int)Managers.Game.SelectedSkillSlotIndex);
					Managers.Game.SelectedSkillSlotIndex = null;
				}
				else
				{
					Managers.Backend.GameData.Skill.EquipSkill((ESkillSlot)_slotIndex, 0);
					Managers.Game.SelectedSkillSlotIndex = null;
				}
				break;
		}
	}

	#endregion
}
