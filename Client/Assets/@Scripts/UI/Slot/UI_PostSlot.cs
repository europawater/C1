using System;
using UnityEngine;

public class UI_PostSlot : UI_Slot
{
	private enum Texts
	{ 
		Text_PostTitle,
		Text_RemainingTime,
	}

	private enum Buttons
	{ 
		Button_Receive,
	}

	private string _postKey;

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindTexts(typeof(Texts));
		BindButtons(typeof(Buttons));

		// Bind Event
		GetButton((int)Buttons.Button_Receive).onClick.AddListener(OnClickReceiveButton);
	}

	public void SetInfo(string postKey)
	{
		_postKey = postKey;
		RefreshUI();
	}

	private void RefreshUI()
	{
		if (Managers.Post.PostDict.ContainsKey(_postKey))
		{
			Post post = Managers.Post.PostDict[_postKey];
			GetText((int)Texts.Text_PostTitle).text = post.Title;

			TimeSpan remainingTime = post.ExpirationDate - DateTime.Now;
			string formattedRemainingTime = $"남은시간 : {remainingTime.Days}일 {remainingTime.Hours}시 {remainingTime.Minutes}분";
			GetText((int)Texts.Text_RemainingTime).text = formattedRemainingTime;
		}
	}

	#region UI Event

	private void OnClickReceiveButton()
	{
		Managers.Post.ReceivePost(_postKey);
	}

	#endregion
}
