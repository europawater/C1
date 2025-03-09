using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_RankPopup : UI_Popup
{
	private enum GameObjects
	{
		CloseArea,
		LoadingArea,
		
		// Rank
		MyRankSlot,
		Content_RankList,
	}

	private enum Buttons
	{
		Button_Close,
	}

	private List<UI_RankSlot> _rankSlotUIList = new List<UI_RankSlot>();

	private List<RankUser> _rankUserList = new List<RankUser>();
	private RankUser _myRank = null;

	protected override void Awake()
	{
		base.Awake();
	
		// Bind
		BindGameObjects(typeof(GameObjects));
		BindButtons(typeof(Buttons));

		// Init
		foreach (Transform child in GetGameObject((int)GameObjects.Content_RankList).transform)
		{
			_rankSlotUIList.Add(child.GetComponent<UI_RankSlot>());
		}

		// Bind Event
		GetGameObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseButton);
		GetButton((int)Buttons.Button_Close).gameObject.BindEvent(OnClickCloseButton);
	}

	public void SetInfo()
	{
		_rankUserList.Clear();
		_myRank = null;

		GetGameObject((int)GameObjects.LoadingArea).SetActive(true);
		GetGameObject((int)GameObjects.MyRankSlot).SetActive(false);
		GetGameObject((int)GameObjects.Content_RankList).SetActive(false);

		Managers.Rank.LeaderBoardList[0].GetRankList(RefreshRankListUI);
		Managers.Rank.LeaderBoardList[0].GetMyRank(RefreshMyRankUI);
	}

	private void RefreshRankListUI(List<RankUser> rankUserList)
	{
		_rankUserList = rankUserList;
		for (int i = 0; i < _rankSlotUIList.Count; i++)
		{
			if (i < _rankUserList.Count)
			{
				_rankSlotUIList[i].SetInfo(_rankUserList[i]);
			}
			else
			{
				_rankSlotUIList[i].gameObject.SetActive(false);
			}
		}

		GetGameObject((int)GameObjects.LoadingArea).SetActive(false);
		GetGameObject((int)GameObjects.Content_RankList).SetActive(true);
	}

	private void RefreshMyRankUI(RankUser myRank)
	{
		_myRank = myRank;
		if (_myRank == null)
		{
			return;
		}
		GetGameObject((int)GameObjects.MyRankSlot).GetComponent<UI_RankSlot>().SetInfo(_myRank);

		GetGameObject((int)GameObjects.MyRankSlot).SetActive(true);
	}

	#region UI Event

	private void OnClickCloseButton(PointerEventData data)
	{
		ClosePopupUI();
	}

	#endregion
}
