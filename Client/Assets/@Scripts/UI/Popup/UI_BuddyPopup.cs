using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_BuddyPopup : UI_Popup
{
	private enum GameObjects
	{
		CloseArea,

		BuddyInfo,

		EquipmentBuddyList,
		BuddyList,
	}

	private enum Texts
	{
		Text_BuddyGrade,
		Text_buddyName,
		Text_SkillDetail,
		Text_Attack,
		Text_Defense,
		Text_Stamina,
		Text_BuddExpInfo,
		Text_OneStarBonus,
		Text_TwoStarBonus,
		Text_ThreeStarBonus,
		Text_FourStarBonus,
		Text_FiveStarBonus,
	}

	private enum Images
	{
		Image_StarGrade1,
		Image_StarGrade2,
		Image_StarGrade3,
		Image_StarGrade4,
		Image_StarGrade5,
		Image_BuddyGrade,
	}

	private enum Buttons
	{
		Button_Close,
		Button_PopupClose,
		Button_Equip,
		Button_EquipAll,
		Button_Upgrade,
		Button_UpgradeAll,
	}

	private enum Sliders
	{
		Slider_BuddyExpInfo
	}

	private enum SkeletonGraphics
	{
		Spine_Buddy,
	}

	private List<UI_EquipmentBuddySlot> _equipmentBuddySlotUIList = new List<UI_EquipmentBuddySlot>();
	private List<UI_BuddySlot> _buddySlotUIList = new List<UI_BuddySlot>();
	private List<Image> _startImageList = new List<Image>();

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindTexts(typeof(Texts));
		BindImages(typeof(Images));
		BindButtons(typeof(Buttons));
		BindSliders(typeof(Sliders));
		BindSkeletonGraphic(typeof(SkeletonGraphics));

		// Init
		foreach (Transform child in GetGameObject((int)GameObjects.EquipmentBuddyList).transform)
		{
			UI_EquipmentBuddySlot equipmentBuddySlotUI = child.GetComponent<UI_EquipmentBuddySlot>();
			if (equipmentBuddySlotUI != null)
			{
				_equipmentBuddySlotUIList.Add(equipmentBuddySlotUI);
			}
		}

		GetGameObject((int)GameObjects.BuddyList).transform.DestroyChildren();
		for (int index = 0; index < Managers.Backend.GameData.Buddy.BuddyInfoDict.Count; index++)
		{
			UI_BuddySlot slot = Managers.UI.MakeSubItem<UI_BuddySlot>(GetGameObject((int)GameObjects.BuddyList).transform);
			_buddySlotUIList.Add(slot);
		}

		_startImageList.Clear();
		_startImageList.Add(GetImage((int)Images.Image_StarGrade1));
		_startImageList.Add(GetImage((int)Images.Image_StarGrade2));
		_startImageList.Add(GetImage((int)Images.Image_StarGrade3));
		_startImageList.Add(GetImage((int)Images.Image_StarGrade4));
		_startImageList.Add(GetImage((int)Images.Image_StarGrade5));

		// Bind Event
		GetGameObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
		GetButton((int)Buttons.Button_PopupClose).gameObject.BindEvent(OnClickCloseArea);
		GetButton((int)Buttons.Button_Close).gameObject.BindEvent(OnClickCloseButton);
		GetButton((int)Buttons.Button_Equip).gameObject.BindEvent(OnClickEquipButton);
		GetButton((int)Buttons.Button_EquipAll).gameObject.BindEvent(OnClickEquipAllButton);
		GetButton((int)Buttons.Button_Upgrade).gameObject.BindEvent(OnClickUpgradeButton);
		GetButton((int)Buttons.Button_UpgradeAll).gameObject.BindEvent(OnClickUpgradeAllButton);
	}

	private void OnEnable()
	{
		Managers.Event.AddEvent(EEventType.OnBuddyChanged, RefreshUI);
		Managers.Event.AddEvent(EEventType.OnSelectedBuddySlotIndex, RefreshUI);
	}

	private void OnDisable()
	{
		Managers.Event.RemoveEvent(EEventType.OnBuddyChanged, RefreshUI);
		Managers.Event.RemoveEvent(EEventType.OnSelectedBuddySlotIndex, RefreshUI);
	}

	public void SetInfo()
	{
		RefreshUI();
	}

	private void RefreshUI()
	{
		for (int slotIndex = 0; slotIndex < _equipmentBuddySlotUIList.Count; slotIndex++)
		{
			BuddyInfo equippedBuddyInfo = null;
			if (!Managers.Game.EquippedBuddyInfoSlotDict.TryGetValue((EBuddySlot)slotIndex, out equippedBuddyInfo))
			{
				_equipmentBuddySlotUIList[slotIndex].SetInfo(slotIndex, null);
			}
			else
			{
				_equipmentBuddySlotUIList[slotIndex].SetInfo(slotIndex, equippedBuddyInfo);
			}
		}

		int buddySlotUIIndex = 0;
		foreach (BuddyInfo buddyInfo in Managers.Backend.GameData.Buddy.BuddyInfoDict.Values)
		{
			_buddySlotUIList[buddySlotUIIndex].SetInfo(UI_BuddySlot.BuddySlotToUse.BuddyPopup, buddyInfo);
			buddySlotUIIndex++;
		}

		GetButton((int)Buttons.Button_EquipAll).gameObject.SetActive(true);
		GetButton((int)Buttons.Button_UpgradeAll).gameObject.SetActive(true);

		// Info
		if (Managers.Game.SelectedBuddySlotIndex != null)
		{
			BuddyInfo buddyInfo = Managers.Backend.GameData.Buddy.BuddyInfoDict[(int)Managers.Game.SelectedBuddySlotIndex];
			BuddyData buddyData = Managers.Backend.Chart.BuddyChart.DataDict[buddyInfo.TemplateID];
			GetImage((int)Images.Image_BuddyGrade).sprite = Managers.Resource.Load<Sprite>(buddyData.SlotKey);
			GetText((int)Texts.Text_BuddyGrade).text = $"{Util.GetBuddyGradeString(buddyData.BuddyGrade)}";
			GetText((int)Texts.Text_buddyName).text = $"{buddyData.Remark}";
			GetText((int)Texts.Text_SkillDetail).text = $"{Managers.Backend.Chart.SkillChart.BuddySkillDataDict[buddyData.DefaultSkill].Remark}";
			GetText((int)Texts.Text_Attack).text = $"{buddyInfo.Stats[EStat.Attack]:N0}";
			GetText((int)Texts.Text_Defense).text = $"{buddyInfo.Stats[EStat.Defense]:N0}";
			GetText((int)Texts.Text_Stamina).text = $"{buddyInfo.Stats[EStat.MaxHP]:N0}";
			GetText((int)Texts.Text_OneStarBonus).text = $"{Util.GetBuddyBonusString(buddyData.OneStarBonusDict)}";
			GetText((int)Texts.Text_TwoStarBonus).text = $"{Util.GetBuddyBonusString(buddyData.TwoStarBonusDict)}";
			GetText((int)Texts.Text_ThreeStarBonus).text = $"{Util.GetBuddyBonusString(buddyData.ThreeStarBonusDict)}";
			GetText((int)Texts.Text_FourStarBonus).text = $"{Util.GetBuddyBonusString(buddyData.FourStarBonusDict)}";
			GetText((int)Texts.Text_FiveStarBonus).text = $"{Util.GetBuddyBonusString(buddyData.FiveStarBonusDict)}";

			GetText((int)Texts.Text_BuddExpInfo).text = $"{buddyInfo.PieceCount:N0}/{buddyData.LevelUpPiece:N0}";
			GetSlider((int)Sliders.Slider_BuddyExpInfo).value = (float)buddyInfo.PieceCount / buddyData.LevelUpPiece;
			for (int index = 0; index < _startImageList.Count; index++)
			{
				_startImageList[index].gameObject.SetActive(index < buddyInfo.Level);
			}
			GetSkeletonGraphic((int)SkeletonGraphics.Spine_Buddy).skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(buddyData.SkeletondataKey);
			GetSkeletonGraphic((int)SkeletonGraphics.Spine_Buddy).Initialize(true);

			GetButton((int)Buttons.Button_EquipAll).gameObject.SetActive(false);
			GetButton((int)Buttons.Button_UpgradeAll).gameObject.SetActive(false);

			GetButton((int)Buttons.Button_Equip).gameObject.SetActive(!buddyInfo.IsEquipped);
			GetButton((int)Buttons.Button_Upgrade).gameObject.SetActive(buddyInfo.CanLevelUp);
		}

		GetGameObject((int)GameObjects.BuddyInfo).SetActive(Managers.Game.SelectedBuddySlotIndex != null);
	}

	#region UI Event

	private void OnClickCloseArea(PointerEventData data)
	{
		Managers.Game.SelectedBuddySlotIndex = null;
		ClosePopupUI();
	}

	private void OnClickCloseButton(PointerEventData data)
	{
		Managers.Game.SelectedBuddySlotIndex = null;
	}

	private void OnClickEquipButton(PointerEventData data)
	{
		if (Managers.Backend.GameData.Buddy.BuddySlotDict[EBuddySlot.BuddySlot01] == 0)
		{
			Managers.Backend.GameData.Buddy.EquipBuddy(EBuddySlot.BuddySlot01, (int)Managers.Game.SelectedBuddySlotIndex);
		}
		else
		{
			Managers.Backend.GameData.Buddy.EquipBuddy(EBuddySlot.BuddySlot02, (int)Managers.Game.SelectedBuddySlotIndex);
		}
	}

	private void OnClickEquipAllButton(PointerEventData data)
	{
		Managers.Backend.GameData.Buddy.UnEquipAllBuddy();
	}

	private void OnClickUpgradeButton(PointerEventData data)
	{
		Managers.Backend.GameData.Buddy.LevelUpBuddy((int)Managers.Game.SelectedBuddySlotIndex);
	}

	private void OnClickUpgradeAllButton(PointerEventData data)
	{
		Managers.Backend.GameData.Buddy.LevelUpBuddys();
	}

	#endregion
}
