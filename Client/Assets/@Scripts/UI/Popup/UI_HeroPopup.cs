using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_HeroPopup : UI_Popup
{
	private enum GameObjects
	{
		CloseArea,

		// Page
		HeroInfoPage,
		SkillInfoPage,

		// Mastery Slot
		UI_MasterySlot_Attack,
		UI_MasterySlot_Defense,
		UI_MasterySlot_MaxHP,

		// Skill Info
		EquippedSkillListArea,
		Content_SkillInven,
	}

	private enum Texts
	{
		Text_MasteryGrade,
	}

	private enum Buttons
	{
		Button_Close,
		Button_MasteryUpgrade,
	}

	private enum Toggles
	{
		Toggle_HeroInfo,
		Toggle_SkillInfo,
	}

	private enum Sliders
	{
		Slider_Mastery,
	}

	public enum HeroPopupState
	{
		None,
		HeroInfo,
		SkillInfo,
	}

	private HeroPopupState _heroPopupState = HeroPopupState.None;

	// Page Toggle
	private Toggle _heroInfoToggle;
	private Toggle _skillInfoToggle;

	// Mastery Slot
	private UI_MasterySlot _attackSlot;
	private UI_MasterySlot _defenseSlot;
	private UI_MasterySlot _maxHPSlot;

	// Skill Info
	private List<UI_EquipmentSkillSlot> _equipmentSkillSlotUIList = new List<UI_EquipmentSkillSlot>();
	private List<UI_SkillSlot> _skillSlotUIList = new List<UI_SkillSlot>();

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindTexts(typeof(Texts));
		BindButtons(typeof(Buttons));
		BindToggles(typeof(Toggles));
		BindSliders(typeof(Sliders));

		// Init
		_heroInfoToggle = GetToggle((int)Toggles.Toggle_HeroInfo);
		_skillInfoToggle = GetToggle((int)Toggles.Toggle_SkillInfo);

		_attackSlot = GetGameObject((int)GameObjects.UI_MasterySlot_Attack).GetComponent<UI_MasterySlot>();
		_defenseSlot = GetGameObject((int)GameObjects.UI_MasterySlot_Defense).GetComponent<UI_MasterySlot>();
		_maxHPSlot = GetGameObject((int)GameObjects.UI_MasterySlot_MaxHP).GetComponent<UI_MasterySlot>();

		foreach (Transform child in GetGameObject((int)GameObjects.EquippedSkillListArea).transform)
		{
			UI_EquipmentSkillSlot equipmentSkillSlotUI = child.GetComponent<UI_EquipmentSkillSlot>();
			if (equipmentSkillSlotUI != null)
			{
				_equipmentSkillSlotUIList.Add(equipmentSkillSlotUI);
			}
		}

		GetGameObject((int)GameObjects.Content_SkillInven).transform.DestroyChildren();
		for (int index = 0; index < Managers.Backend.GameData.Skill.SkillInfoDict.Count; index++)
		{
			UI_SkillSlot slot = Managers.UI.MakeSubItem<UI_SkillSlot>(GetGameObject((int)GameObjects.Content_SkillInven).transform);
			_skillSlotUIList.Add(slot);
		}

		_heroPopupState = HeroPopupState.HeroInfo;

		// Bind Event
		GetGameObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
		GetButton((int)Buttons.Button_Close).gameObject.BindEvent(OnClickCloseArea);

		_heroInfoToggle.gameObject.BindEvent(OnClickHeroInfoToggle);
		_skillInfoToggle.gameObject.BindEvent(OnClickSkillInfoToggle);

		GetButton((int)Buttons.Button_MasteryUpgrade).gameObject.BindEvent(OnClickMasteryUpgrade);
	}

	private void OnEnable()
	{
		Managers.Event.AddEvent(EEventType.OnMasteryChanged, RefreshUI);
		Managers.Event.AddEvent(EEventType.OnSkillChanged, RefreshSkillInfoUI);
	}

	private void OnDisable()
	{
		Managers.Event.RemoveEvent(EEventType.OnMasteryChanged, RefreshUI);
		Managers.Event.RemoveEvent(EEventType.OnSkillChanged, RefreshSkillInfoUI);
	}

	public void SetInfo()
	{
		RefreshUI();
	}

	private void RefreshUI()
	{
		switch (_heroPopupState)
		{
			case HeroPopupState.HeroInfo:
				RefreshHeroInfoUI();
				GetGameObject((int)GameObjects.HeroInfoPage).SetActive(true);
				GetGameObject((int)GameObjects.SkillInfoPage).SetActive(false);
				break;
			case HeroPopupState.SkillInfo:
				RefreshSkillInfoUI();
				GetGameObject((int)GameObjects.HeroInfoPage).SetActive(false);
				GetGameObject((int)GameObjects.SkillInfoPage).SetActive(true);
				break;
		}
	}

	private void RefreshHeroInfoUI()
	{
		GetButton((int)Buttons.Button_MasteryUpgrade).interactable = Managers.Backend.GameData.Mastery.GetCanMasterytUpgrade();
		GetSlider((int)Sliders.Slider_Mastery).value = Managers.Backend.GameData.Mastery.GetMasteryProgress();
		GetText((int)Texts.Text_MasteryGrade).text = Managers.Backend.Chart.MasteryChart.DataDict[Managers.Backend.GameData.Mastery.MasteryGrade].Remark;

		_attackSlot.SetInfo(Managers.Backend.GameData.Mastery.GetCanMasterLevelUp(EMasteryType.Attack), EMasteryType.Attack, Managers.Backend.GameData.Mastery.MasteryLevelDict[EMasteryType.Attack], Managers.Backend.GameData.Mastery.MasteryValueDict[EMasteryType.Attack], Managers.Backend.GameData.Mastery.GetNeedToGold(EMasteryType.Attack));
		_defenseSlot.SetInfo(Managers.Backend.GameData.Mastery.GetCanMasterLevelUp(EMasteryType.Defense), EMasteryType.Defense, Managers.Backend.GameData.Mastery.MasteryLevelDict[EMasteryType.Defense], Managers.Backend.GameData.Mastery.MasteryValueDict[EMasteryType.Defense], Managers.Backend.GameData.Mastery.GetNeedToGold(EMasteryType.Defense));
		_maxHPSlot.SetInfo(Managers.Backend.GameData.Mastery.GetCanMasterLevelUp(EMasteryType.MaxHP), EMasteryType.MaxHP, Managers.Backend.GameData.Mastery.MasteryLevelDict[EMasteryType.MaxHP], Managers.Backend.GameData.Mastery.MasteryValueDict[EMasteryType.MaxHP], Managers.Backend.GameData.Mastery.GetNeedToGold(EMasteryType.MaxHP));
	}

	private void RefreshSkillInfoUI()
	{
		for (int slotIndex = 0; slotIndex < _equipmentSkillSlotUIList.Count; slotIndex++)
		{
			Skill equippedSkill = null;
			if (!Managers.Game.EquippedSkillSlotDict.TryGetValue((ESkillSlot)slotIndex, out equippedSkill))
			{
				_equipmentSkillSlotUIList[slotIndex].SetInfo(slotIndex, UI_EquipmentSkillSlot.EquipmentSkillSlotToUse.HeroPopup, null);
			}
			else
			{
				_equipmentSkillSlotUIList[slotIndex].SetInfo(slotIndex, UI_EquipmentSkillSlot.EquipmentSkillSlotToUse.HeroPopup, equippedSkill);
			}
		}

		int skillSlotUIIndex = 0;
		foreach (SkillInfo skillInfo in Managers.Backend.GameData.Skill.SkillInfoDict.Values)
		{
			_skillSlotUIList[skillSlotUIIndex].SetInfo(skillInfo);
			skillSlotUIIndex++;
		}
	}

	#region UI Event

	private void OnClickCloseArea(PointerEventData data)
	{
		ClosePopupUI();
	}

	private void OnClickMasteryUpgrade(PointerEventData data)
	{
		if (!Managers.Backend.GameData.Mastery.GetCanMasterytUpgrade())
		{
			return;
		}

		Managers.Backend.GameData.Mastery.UpgradeMastery();
	}

	private void OnClickHeroInfoToggle(PointerEventData data)
	{
		_heroPopupState = HeroPopupState.HeroInfo;
		RefreshUI();
	}

	private void OnClickSkillInfoToggle(PointerEventData data)
	{
		_heroPopupState = HeroPopupState.SkillInfo;
		RefreshUI();
	}

	#endregion
}
