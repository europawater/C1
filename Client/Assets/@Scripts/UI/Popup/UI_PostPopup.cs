using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_PostPopup : UI_Popup
{
	private enum GameObjects
	{
		CloseArea,

		// Post
		Content_PostList,
	}

	private enum Buttons
	{
		Button_Close,
		Button_ReceiveAll,
	}

	private List<UI_PostSlot> _postSlotUIList = new List<UI_PostSlot>();

	protected override void Awake()
	{ 
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindButtons(typeof(Buttons));

		// Bind Event
		GetGameObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseButton);
		GetButton((int)Buttons.Button_Close).gameObject.BindEvent(OnClickCloseButton);
	}

	private void OnEnable()
	{
		Managers.Event.AddEvent(EEventType.OnPostListChanged, RefreshUI);
	}

	private void OnDisable()
	{
		Managers.Event.RemoveEvent(EEventType.OnPostListChanged, RefreshUI);
	}

	public void SetInfo()
	{
		// юс╫ц
		GetButton((int)Buttons.Button_ReceiveAll).gameObject.SetActive(false);

		_postSlotUIList.Clear();
		foreach (Transform child in GetGameObject((int)GameObjects.Content_PostList).transform)
		{ 
			_postSlotUIList.Add(child.GetComponent<UI_PostSlot>());
		}
		
		RefreshUI();
	}

	private void RefreshUI()
	{
		for (int i = 0; i < _postSlotUIList.Count; i++)
		{ 
			_postSlotUIList[i].gameObject.SetActive(false);
		}

		int index = 0;
		foreach (var post in Managers.Post.PostDict)
		{
			_postSlotUIList[index].SetInfo(post.Key);
			_postSlotUIList[index].gameObject.SetActive(true);
			index++;
		}
	}

	#region UI Event

	private void OnClickCloseButton(PointerEventData data)
	{
		ClosePopupUI();
	}

	#endregion
}
