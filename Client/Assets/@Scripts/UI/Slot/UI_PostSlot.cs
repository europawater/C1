using System;
using UnityEngine;
using static Define;

public class UI_PostSlot : UI_Slot
{
	private enum Texts
	{ 
		Text_PostTitle,
		Text_RemainingTime,
		Text_ItemCount,
	}

	private enum Images
	{
		Image_ItemIcon,
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
		BindImages(typeof(Images));
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

			GetImage((int)Images.Image_ItemIcon).sprite = Managers.Resource.Load<Sprite>(Managers.Backend.Chart.RewardChart.DataDict[post.PostChartList[0].TemplateID].IconKey);
			GetText((int)Texts.Text_ItemCount).text = $"{post.PostChartList[0].Count:N0}";
		}
	}

	#region UI Event

	private void OnClickReceiveButton()
	{
		Managers.Post.ReceivePost(_postKey);
	}

	#endregion
}
