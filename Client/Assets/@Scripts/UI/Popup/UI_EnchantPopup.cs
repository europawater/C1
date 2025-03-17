using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_EnchantPopup : UI_Popup
{
	private enum GameObjects
	{
		CloseArea,

		UI_EnchantStatSlot1,
		UI_EnchantStatSlot2,
	}

	private enum Texts
	{
		Text_EnchantCount,
		Text_ItemName,
		Text_SuccessCount,
		Text_FaillCount,
	}

	private enum Images
	{
		Image_ItemBg,
		Image_ItemIcon,

		Image_ItemDefenseIcon1,
		Image_ItemDefenseIcon2,
		Image_ItemDefenseIcon3,
	}

	private enum Buttons
	{
		Button_Close,
		Button_Collection,
		Button_Enchant,
	}

	private List<UI_EnchantStatSlot> _enchantStatUIList = new List<UI_EnchantStatSlot>();
	private List<Image> _shieldImageList = new List<Image>();

	private UI_EquipmentPopup _equipmentPopup;

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindTexts(typeof(Texts));
		BindImages(typeof(Images));
		BindButtons(typeof(Buttons));

		// Init
		_enchantStatUIList.Clear();
		_enchantStatUIList.Add(GetGameObject((int)GameObjects.UI_EnchantStatSlot1).GetComponent<UI_EnchantStatSlot>());
		_enchantStatUIList.Add(GetGameObject((int)GameObjects.UI_EnchantStatSlot2).GetComponent<UI_EnchantStatSlot>());

		_shieldImageList.Clear();
		_shieldImageList.Add(GetImage((int)Images.Image_ItemDefenseIcon1));
		_shieldImageList.Add(GetImage((int)Images.Image_ItemDefenseIcon2));
		_shieldImageList.Add(GetImage((int)Images.Image_ItemDefenseIcon3));

		// Bind Event
		GetGameObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
		GetButton((int)Buttons.Button_Close).gameObject.BindEvent(OnClickCloseArea);
		GetButton((int)Buttons.Button_Collection).gameObject.BindEvent(OnClickCollection);
		GetButton((int)Buttons.Button_Enchant).gameObject.BindEvent(OnClickEnchant);
	}

	private void OnEnable()
	{
		Managers.Event.AddEvent(EEventType.OnEquipmentChanged, RefreshUI);
		Managers.Event.AddEvent(EEventType.OnEquipmentChanged, RefreshEquipmentPopup);
		Managers.Event.AddEvent(EEventType.OnEquipmentEnchantFail, RefreshUI);
	}

	private void OnDisable()
	{
		Managers.Event.RemoveEvent(EEventType.OnEquipmentChanged, RefreshUI);
		Managers.Event.RemoveEvent(EEventType.OnEquipmentChanged, RefreshEquipmentPopup);
		Managers.Event.RemoveEvent(EEventType.OnEquipmentEnchantFail, RefreshUI);
	}

	public void SetInfo(UI_EquipmentPopup equipmentPopup)
	{
		_equipmentPopup = equipmentPopup;

		RefreshUI();
	}

	private void RefreshUI()
	{
		if (Managers.Game.NewEquipment == null)
		{
			ClosePopupUI();
			_equipmentPopup.ClosePopupUI();
			return;
		}

		GetText((int)Texts.Text_EnchantCount).text = $"+{Managers.Game.NewEquipment.EnchantLevel}";
		GetText((int)Texts.Text_ItemName).text = $"{Managers.Game.NewEquipment.EquipmentData.Remark}";
		GetText((int)Texts.Text_SuccessCount).text = $"{Managers.Game.NewEquipment.EquipmentData.EnchantRate}%";
		GetText((int)Texts.Text_FaillCount).text = $"{100 - Managers.Game.NewEquipment.EquipmentData.EnchantRate}%";

		GetImage((int)Images.Image_ItemBg).sprite = Managers.Resource.Load<Sprite>(Managers.Game.NewEquipment.EquipmentData.SlotKey);
		GetImage((int)Images.Image_ItemIcon).sprite = Managers.Resource.Load<Sprite>(Managers.Game.NewEquipment.EquipmentData.IconKey);

		int index = 0;
		foreach (var validEquipmentValue in Managers.Game.NewEquipment.ValidEquipmentValueDict)
		{
			float increaseValue = 0.0f;
			switch (validEquipmentValue.Key)
			{
				case EStat.Attack:
					increaseValue = Managers.Backend.Chart.EquipmentLevelScalingChart.DataDict[Managers.Game.NewEquipment.EquipmentData.TemplateID].IncreaseAttack;
					break;
				case EStat.Defense:
					increaseValue = Managers.Backend.Chart.EquipmentLevelScalingChart.DataDict[Managers.Game.NewEquipment.EquipmentData.TemplateID].IncreaseDefense;
					break;
				case EStat.MaxHP:
					increaseValue = Managers.Backend.Chart.EquipmentLevelScalingChart.DataDict[Managers.Game.NewEquipment.EquipmentData.TemplateID].IncreaseMaxHP;
					break;
				case EStat.CriticalValue:
					increaseValue = Managers.Backend.Chart.EquipmentLevelScalingChart.DataDict[Managers.Game.NewEquipment.EquipmentData.TemplateID].IncreaseCriticalValue;
					break;
				case EStat.CriticalRate:
					increaseValue = Managers.Backend.Chart.EquipmentLevelScalingChart.DataDict[Managers.Game.NewEquipment.EquipmentData.TemplateID].IncreaseCriticalRate;
					break;
				case EStat.SkillDamageValue:
					increaseValue = Managers.Backend.Chart.EquipmentLevelScalingChart.DataDict[Managers.Game.NewEquipment.EquipmentData.TemplateID].IncreaseSkillDamageValue;
					break;
				case EStat.SkillCriticalValue:
					increaseValue = Managers.Backend.Chart.EquipmentLevelScalingChart.DataDict[Managers.Game.NewEquipment.EquipmentData.TemplateID].IncreaseSkillCriticalValue;
					break;
				case EStat.DodgeRate:
					increaseValue = Managers.Backend.Chart.EquipmentLevelScalingChart.DataDict[Managers.Game.NewEquipment.EquipmentData.TemplateID].IncreaseDodgeRate;
					break;
				case EStat.ComboAttackRate:
					increaseValue = Managers.Backend.Chart.EquipmentLevelScalingChart.DataDict[Managers.Game.NewEquipment.EquipmentData.TemplateID].IncreaseComboAttackRate;
					break;
				case EStat.CounterAttackRate:
					increaseValue = Managers.Backend.Chart.EquipmentLevelScalingChart.DataDict[Managers.Game.NewEquipment.EquipmentData.TemplateID].IncreaseCounterAttackRate;
					break;
				case EStat.BossExtraValue:
					increaseValue = Managers.Backend.Chart.EquipmentLevelScalingChart.DataDict[Managers.Game.NewEquipment.EquipmentData.TemplateID].IncreaseBossExtraValue;
					break;

				default:
					break;
			}

			_enchantStatUIList[index].SetInfo(validEquipmentValue.Key, validEquipmentValue.Value, increaseValue);
			index++;
		}

		for (int count = 0; count < _shieldImageList.Count; count++)
		{
			if (count < Managers.Game.NewEquipment.EnchantSafe)
			{
				_shieldImageList[count].color = Color.white;
			}
			else
			{
				_shieldImageList[count].color = Color.gray;
			}
		}

		bool canRegist = Managers.Game.CanRegistCollection(ECollectionType.Item, Managers.Game.NewEquipment.EquipmentInfo.TemplateID);
		GetButton((int)Buttons.Button_Collection).interactable = canRegist;
	}

	private void RefreshEquipmentPopup()
	{ 
		_equipmentPopup.RefreshUI();
	}

	#region UI Event

	private void OnClickCloseArea(PointerEventData data)
	{
		ClosePopupUI();
	}

	private void OnClickCollection(PointerEventData data)
	{
		if (GetButton((int)Buttons.Button_Collection).interactable)
		{
			Managers.Game.RegistCollection(ECollectionType.Item, Managers.Game.NewEquipment.EquipmentInfo.TemplateID);
			ClosePopupUI();
			_equipmentPopup.ClosePopupUI();
		}
	}

	private void OnClickEnchant(PointerEventData data)
	{
		Managers.Game.HandleEnchantNewEquipment();
	}

	#endregion
}
