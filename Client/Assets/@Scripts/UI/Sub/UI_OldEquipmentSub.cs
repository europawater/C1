using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_OldEquipmentSub : UI_Sub
{
	private enum GameObjects
	{
		UI_EquipmentSlot,
		StatArea,
	}

	private enum Texts
	{ 
		Text_GradeAndName,
	}

	private enum Images
	{
		Image_ShieldIcon1,
		Image_ShieldIcon2,
		Image_ShieldIcon3,
	}

	private Equipment _equipment;
	private UI_EquipmentSlot _equipmentSlot;
	private List<UI_EquipmentStatSlot> _statSlotList = new List<UI_EquipmentStatSlot>();
	private List<Image> _shieldImageList = new List<Image>();

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindTexts(typeof(Texts));
		BindImages(typeof(Images));

		// Init
		_equipmentSlot = GetGameObject((int)GameObjects.UI_EquipmentSlot).GetComponent<UI_EquipmentSlot>();
		
		_statSlotList.Clear();
		foreach(Transform child in GetGameObject((int)GameObjects.StatArea).transform)
		{
			UI_EquipmentStatSlot statSlot = child.GetComponent<UI_EquipmentStatSlot>();
			if (statSlot != null)
			{
				_statSlotList.Add(statSlot);
			}
		}

		_shieldImageList.Clear();
		_shieldImageList.Add(GetImage((int)Images.Image_ShieldIcon1));
		_shieldImageList.Add(GetImage((int)Images.Image_ShieldIcon2));
		_shieldImageList.Add(GetImage((int)Images.Image_ShieldIcon3));
	}

	public void SetInfo(Equipment equipment)
	{
		_equipment = equipment;

		RefreshUI();
	}

	private void RefreshUI()
	{
		GetText((int)Texts.Text_GradeAndName).text = $"{_equipment.EquipmentData.Remark}";

		_equipmentSlot.SetInfo(UI_EquipmentSlot.EquipmentSlotToUse.EquipmentPopup, _equipment);

		int index = 0;
		foreach(var validEquipmentValue in _equipment.ValidEquipmentValueDict)
		{
			_statSlotList[index].SetInfo(validEquipmentValue.Key, validEquipmentValue.Value, validEquipmentValue.Value);
			index++;
		}

		for(int count = 0; count < _shieldImageList.Count; count++)
		{
			if(count < _equipment.EnchantSafe)
			{
				_shieldImageList[count].color = Color.white;
			}
			else
			{
				_shieldImageList[count].color = Color.gray;
			}
		}
	}
}
