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
	}

	private EquipmentSlotToUse _equipmentSlotToUse;
	private Equipment _equipment;

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

	private void RefreshUI()
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

		switch (_equipmentSlotToUse)
		{
			case EquipmentSlotToUse.GameScene:
				RefreshByGameScene();
				break;
			case EquipmentSlotToUse.EquipmentPopup:
				RefreshByEquipmentPopup();
				break;
		}
	}

	private void RefreshByGameScene()
	{
	}

	private void RefreshByEquipmentPopup()
	{
	}
}
