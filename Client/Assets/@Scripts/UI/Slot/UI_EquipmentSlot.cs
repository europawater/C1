using System;
using UnityEngine;
using static Define;

public class UI_EquipmentSlot : UI_Slot
{
	private enum Texts
	{
		Text_Level,
	}

	private enum Images
	{
		Image_Slot,
		Image_ItemIcon,
	}

	public enum EquipmentSlotToUse
	{
		None,
		GameScene,
		EquipmentPopup,
		CollectionPopup,
	}

	private EquipmentSlotToUse _equipmentSlotToUse;
	private Equipment _equipment;

	// Collection
	private EquipmentData _equipmentData;
	private EOwningState _owningState;

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindTexts(typeof(Texts));
		BindImages(typeof(Images));
	}

	public void SetInfo(EquipmentSlotToUse equipmentSlotToUse, Equipment equipment)
	{
		_equipmentSlotToUse = equipmentSlotToUse;
		_equipment = equipment;

		RefreshUI();
	}

	public void SetInfo(EquipmentSlotToUse equipmentSlotToUse, int templateID, EOwningState owningState)
	{
		_equipmentSlotToUse = equipmentSlotToUse;
		_equipmentData = Managers.Backend.Chart.EquipmentChart.DataDict[templateID];
		_owningState = owningState;

		RefreshUI();
	}

	private void RefreshUI()
	{
		switch (_equipmentSlotToUse)
		{
			case EquipmentSlotToUse.GameScene:
				RefreshByGameScene();
				break;
			case EquipmentSlotToUse.EquipmentPopup:
				RefreshByEquipmentPopup();
				break;
			case EquipmentSlotToUse.CollectionPopup:
				RefreshByCollectionPopup();
				break;

			default:
				break;
		}
	}

	private void RefreshByGameScene()
	{
		if (_equipment == null)
		{
			GetText((int)Texts.Text_Level).text = string.Empty;
			GetImage((int)Images.Image_Slot).gameObject.SetActive(false);
			GetImage((int)Images.Image_ItemIcon).gameObject.SetActive(false);
		}
		else
		{
			GetText((int)Texts.Text_Level).text = $"+{_equipment.EnchantLevel}";
			GetImage((int)Images.Image_Slot).sprite = Managers.Resource.Load<Sprite>(_equipment.EquipmentData.SlotKey);
			GetImage((int)Images.Image_ItemIcon).sprite = Managers.Resource.Load<Sprite>(_equipment.EquipmentData.IconKey);
			GetImage((int)Images.Image_Slot).gameObject.SetActive(true);
			GetImage((int)Images.Image_ItemIcon).gameObject.SetActive(true);
		}
	}

	private void RefreshByEquipmentPopup()
	{
		if (_equipment == null)
		{
			GetText((int)Texts.Text_Level).text = string.Empty;
			GetImage((int)Images.Image_Slot).gameObject.SetActive(false);
			GetImage((int)Images.Image_ItemIcon).gameObject.SetActive(false);
		}
		else
		{
			GetText((int)Texts.Text_Level).text = $"+{_equipment.EnchantLevel}";
			GetImage((int)Images.Image_Slot).sprite = Managers.Resource.Load<Sprite>(_equipment.EquipmentData.SlotKey);
			GetImage((int)Images.Image_ItemIcon).sprite = Managers.Resource.Load<Sprite>(_equipment.EquipmentData.IconKey);
			GetImage((int)Images.Image_Slot).gameObject.SetActive(true);
			GetImage((int)Images.Image_ItemIcon).gameObject.SetActive(true);
		}
	}

	private void RefreshByCollectionPopup()
	{
		GetText((int)Texts.Text_Level).text = string.Empty;
		GetImage((int)Images.Image_Slot).sprite = Managers.Resource.Load<Sprite>(_equipmentData.SlotKey);
		GetImage((int)Images.Image_ItemIcon).sprite = Managers.Resource.Load<Sprite>(_equipmentData.IconKey);
		GetImage((int)Images.Image_ItemIcon).color = _owningState == EOwningState.Owned ? Color.white : Color.gray;
	}
}
