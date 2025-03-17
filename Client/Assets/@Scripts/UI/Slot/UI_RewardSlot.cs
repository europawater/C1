using UnityEngine;
using static Define;
using static UI_DrawPopup;

public class UI_RewardSlot : UI_Slot
{
	private enum Images
	{
		Image_Icon,
	}

	private enum Texts
	{
		Text_ItemCount,
	}

	private ERewardType _rewardType;
	private int _rewardAmount;

	private DrawPopupState _drawPopupState;
	private int _rewardId;

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindImages(typeof(Images));
		BindTexts(typeof(Texts));
	}

	public void SetInfo(ERewardType rewardType, int rewardAmount)
	{
		_rewardType = rewardType;
		_rewardAmount = rewardAmount;

		RefreshUI();
	}

	public void SetInfo(DrawPopupState drawPopupState, int rewardId)
	{
		_drawPopupState = drawPopupState;
		_rewardId = rewardId;

		RefreshDrawUI();
	}

	private void RefreshUI()
	{
		GetImage((int)Images.Image_Icon).sprite = Managers.Resource.Load<Sprite>(Managers.Backend.Chart.RewardChart.DataDict[(int)_rewardType].IconKey);
		GetText((int)Texts.Text_ItemCount).text = $"{_rewardAmount:N0}";
	}

	private void RefreshDrawUI()
	{
		switch (_drawPopupState)
		{
			case DrawPopupState.Skill:
				GetImage((int)Images.Image_Icon).sprite = Managers.Resource.Load<Sprite>(Managers.Backend.Chart.SkillChart.HeroSkillDataDict[_rewardId].IconKey);
				GetText((int)Texts.Text_ItemCount).text = string.Empty;
				break;
			case DrawPopupState.Buddy:
				GetImage((int)Images.Image_Icon).sprite = Managers.Resource.Load<Sprite>(Managers.Backend.Chart.BuddyChart.DataDict[_rewardId].IconKey);
				GetText((int)Texts.Text_ItemCount).text = string.Empty;
				break;

			default:
				break;
		}
	}
}
