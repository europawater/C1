using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class UI_RankSlot : UI_Slot
{
	private enum  Texts
	{
		Text_Rank,
		Text_Name,
		Text_Score,
	}

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindTexts(typeof(Texts));
	}

	private RankUser _rankUser = null;

	public void SetInfo(RankUser rankUser)
	{
		_rankUser = rankUser;

		RefreshUI();
	}

	private void RefreshUI()
	{
		GetText((int)Texts.Text_Rank).text = $"{_rankUser.Rank}";
		GetText((int)Texts.Text_Name).text = $"{_rankUser.NickName}";
		GetText((int)Texts.Text_Score).text = $"몬스터 킬 카운트 : {_rankUser.Score}";
	}
}
