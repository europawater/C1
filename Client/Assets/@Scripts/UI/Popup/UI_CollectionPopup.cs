using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_CollectionPopup : UI_Popup
{
	private enum GameObjects
	{
		CloseArea,

		// ScrollView
		ScrollView_CollectionItem,
		ScrollView_CollectionBuddy,
		ScrollView_CollectionSkill,

		// Content
		CollectionItemArea,
		CollectionBuddyArea,
		CollectionSkillArea,
	}

	private enum Buttons
	{
		Button_Close,
	}

	private enum Toggles
	{ 
		Toggle_Item,
		Toggle_Buddy,
		Toggle_Skill,
	}

	public enum CollectionPopupState
	{ 
		None,
		Item,
		Buddy,
		Skill,
	}

	private CollectionPopupState _collectionPopupState = CollectionPopupState.None;

	// Toggle
	private Toggle _itemToggle;
	private Toggle _buddyToggle;
	private Toggle _skillToggle;

	// SlotList
	private List<UI_CollectionItemSlot> _itemCollectionUISlotList = new List<UI_CollectionItemSlot>();
	private List<UI_CollectionBuddySlot> _buddyCollectionUISlotList = new List<UI_CollectionBuddySlot>();
	private List<UI_CollectionSkillSlot> _skillCollectionUISlotList = new List<UI_CollectionSkillSlot>();

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindButtons(typeof(Buttons));
		BindToggles(typeof(Toggles));

		// Init
		_itemToggle = GetToggle((int)Toggles.Toggle_Item);
		_buddyToggle = GetToggle((int)Toggles.Toggle_Buddy);
		_skillToggle = GetToggle((int)Toggles.Toggle_Skill);

		GetGameObject((int)GameObjects.CollectionItemArea).transform.DestroyChildren();
		foreach (Collection collection in Managers.Game.ItemCollectionList)
		{
			UI_CollectionItemSlot slot = Managers.UI.MakeSubItem<UI_CollectionItemSlot>(GetGameObject((int)GameObjects.CollectionItemArea).transform);
			if (slot != null)
			{
				_itemCollectionUISlotList.Add(slot);
			}
		}
		
		GetGameObject((int)GameObjects.CollectionBuddyArea).transform.DestroyChildren();
		foreach (Collection collection in Managers.Game.BuddyCollectionList)
		{
			UI_CollectionBuddySlot slot = Managers.UI.MakeSubItem<UI_CollectionBuddySlot>(GetGameObject((int)GameObjects.CollectionBuddyArea).transform);
			if (slot != null)
			{
				_buddyCollectionUISlotList.Add(slot);
			}
		}

		GetGameObject((int)GameObjects.CollectionSkillArea).transform.DestroyChildren();
		foreach (Collection collection in Managers.Game.SkillCollectionList)
		{
			UI_CollectionSkillSlot slot = Managers.UI.MakeSubItem<UI_CollectionSkillSlot>(GetGameObject((int)GameObjects.CollectionSkillArea).transform);
			if (slot != null)
			{
				_skillCollectionUISlotList.Add(slot);
			}
		}

		_collectionPopupState = CollectionPopupState.Item;

		// Bind Event
		GetGameObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
		GetButton((int)Buttons.Button_Close).gameObject.BindEvent(OnClickCloseArea);

		_itemToggle.gameObject.BindEvent(OnClickItemToggle);
		_buddyToggle.gameObject.BindEvent(OnClickBuddyToggle);
		_skillToggle.gameObject.BindEvent(OnClickSkillToggle);
	}

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.OnCollectionChanged, RefreshUI);
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.OnCollectionChanged, RefreshUI);
    }

    public void SetInfo()
	{
		RefreshUI();
	}

	private void RefreshUI()
	{
		switch (_collectionPopupState)
		{
			case CollectionPopupState.Item:
				RefreshItemUI();
				GetGameObject((int)GameObjects.ScrollView_CollectionItem).SetActive(true);
				GetGameObject((int)GameObjects.ScrollView_CollectionBuddy).SetActive(false);
				GetGameObject((int)GameObjects.ScrollView_CollectionSkill).SetActive(false);
				break;
			case CollectionPopupState.Buddy:
				RefreshBuddyUI();
				GetGameObject((int)GameObjects.ScrollView_CollectionItem).SetActive(false);
				GetGameObject((int)GameObjects.ScrollView_CollectionBuddy).SetActive(true);
				GetGameObject((int)GameObjects.ScrollView_CollectionSkill).SetActive(false);
				break;
			case CollectionPopupState.Skill:
				RefreshSkillUI();
				GetGameObject((int)GameObjects.ScrollView_CollectionItem).SetActive(false);
				GetGameObject((int)GameObjects.ScrollView_CollectionBuddy).SetActive(false);
				GetGameObject((int)GameObjects.ScrollView_CollectionSkill).SetActive(true);
				break;

			default:
				break;
		}
	}

	private void RefreshItemUI()
	{
		int index = 0;
		foreach (Collection collection in Managers.Game.ItemCollectionList)
		{ 
			_itemCollectionUISlotList[index].SetInfo(collection);
			index++;
		}
	}

	private void RefreshBuddyUI()
	{
		int index = 0;
		foreach (Collection collection in Managers.Game.BuddyCollectionList)
		{
			_buddyCollectionUISlotList[index].SetInfo(collection);
			index++;
		}
	}

	private void RefreshSkillUI()
	{
		int index = 0;
		foreach (Collection collection in Managers.Game.SkillCollectionList)
		{
			_skillCollectionUISlotList[index].SetInfo(collection);
			index++;
		}
	}

	#region UI Event

	private void OnClickCloseArea(PointerEventData data)
	{
		ClosePopupUI();
	}

	private void OnClickItemToggle(PointerEventData data)
	{
		_collectionPopupState = CollectionPopupState.Item;
		RefreshUI();
	}

	private void OnClickBuddyToggle(PointerEventData data)
	{
		_collectionPopupState = CollectionPopupState.Buddy;
		RefreshUI();
	}

	private void OnClickSkillToggle(PointerEventData data)
	{
		_collectionPopupState = CollectionPopupState.Skill;
		RefreshUI();
	}

	#endregion
}
