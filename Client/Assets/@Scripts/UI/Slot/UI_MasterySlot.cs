using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_MasterySlot : UI_Slot
{
	private enum Texts
	{
		Text_MasteryLevel,
		Text_MasteryValue,
		Text_Cost,
	}

	private enum Images
	{
		// Off
		Image_Background_Max,
		Image_Off,
	}

	private enum Buttons
	{
		Button_LevelUp,
	}

	private bool _isActive = false;
	private EMasteryType _masteryType = EMasteryType.None;
	private int _masteryLevel = 0;
	private int _masteryValue = 0;
	private int _needGoldAmount = 0;

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindTexts(typeof(Texts));
		BindImages(typeof(Images));
		BindButtons(typeof(Buttons));

		// Bind Event
		GetButton((int)Buttons.Button_LevelUp).gameObject.BindEvent(OnClickLevelUp);
	}

	public void SetInfo(bool isActive, EMasteryType masteryType, int masteryLevel, int masteryValue, int needGoldAmount)
	{
		_isActive = isActive;
		_masteryType = masteryType;
		_masteryLevel = masteryLevel;
		_masteryValue = masteryValue;
		_needGoldAmount = needGoldAmount;

		RefreshUI();
	}

	private void RefreshUI()
	{
		GetText((int)Texts.Text_MasteryLevel).text = $"LV.{_masteryLevel}";

		string statString;
		switch(_masteryType)
		{
			case EMasteryType.Attack:
				statString = "공격력";
				break;
			case EMasteryType.Defense:
				statString = "방어력";
				break;
			case EMasteryType.MaxHP:
				statString = "최대 체력";
				break;

			default:
				statString = "";
				break;
		}
		GetText((int)Texts.Text_MasteryValue).text = $"{statString} + {_masteryValue:N0}";
		GetText((int)Texts.Text_Cost).text = $"{_needGoldAmount:N0}";

		GetImage((int)Images.Image_Background_Max).gameObject.SetActive(!_isActive);
		GetImage((int)Images.Image_Off).gameObject.SetActive(!_isActive);
	}

	#region UI Event

	private void OnClickLevelUp(PointerEventData data)
	{
		if (!_isActive)
		{ 
			// TODO: some..
		}

		Managers.Backend.GameData.Mastery.LevelUpMastery(_masteryType);
	}

	#endregion
}
