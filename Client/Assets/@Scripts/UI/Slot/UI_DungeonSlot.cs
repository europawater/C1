using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_DungeonSlot : UI_Slot
{
	private enum Texts
	{
		Text_DungeonName,
		Text_RewardItemDescription,
		Text_DungeonDifficulty,
		Text_DungeonTicketCount,
	}

	private enum Images
	{
		Image_RewardIcon,
	}

	private enum Buttons
	{
		Button_EnterDungeon,
		Button_Sweep,
	}

	private DungeonData _dungeonData;

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindTexts(typeof(Texts));
		BindImages(typeof(Images));
		BindButtons(typeof(Buttons));

		// Bind Event
		GetButton((int)Buttons.Button_EnterDungeon).gameObject.BindEvent(OnClickEnterDungeon);
		GetButton((int)Buttons.Button_Sweep).gameObject.BindEvent(OnClickSweep);
	}

	public void SetInfo(EDungeonType dungeonType, int dungeonLevel)
	{
		switch (dungeonType)
		{
			case EDungeonType.Cube:
				_dungeonData = Managers.Backend.Chart.DungeonChart.CubeDungeonDataDict[dungeonLevel];
				break;
			case EDungeonType.Gold:
				_dungeonData = Managers.Backend.Chart.DungeonChart.GoldDungeonDataDict[dungeonLevel];
				break;
			case EDungeonType.Diamond:
				_dungeonData = Managers.Backend.Chart.DungeonChart.DiamondDungeonDataDict[dungeonLevel];
				break;

			default:
				break;
		}

		RefreshUI();
	}

	private void RefreshUI()
	{
        // Button
        GetButton((int)Buttons.Button_Sweep).interactable = Managers.Backend.GameData.Currency.CurrencyDict[ECurrency.DungeonTicket] > 0;
		GetButton((int)Buttons.Button_EnterDungeon).interactable = Managers.Backend.GameData.Currency.CurrencyDict[ECurrency.DungeonTicket] > 0;

        // Name
        GetText((int)Texts.Text_DungeonName).text = $"{_dungeonData.Remark}";

		// Reward
		GetText((int)Texts.Text_RewardItemDescription).text = $"{_dungeonData.RewardValue} °³";

		// Difficulty
		GetText((int)Texts.Text_DungeonDifficulty).text = $"{_dungeonData.DifficultyLevel:N0}";

		// Ticket
		GetText((int)Texts.Text_DungeonTicketCount).text = $"{Managers.Backend.GameData.Currency.CurrencyDict[ECurrency.DungeonTicket]:N0}";
	}

	#region UI Event

	private void OnClickEnterDungeon(PointerEventData data)
	{
		if(Managers.Backend.GameData.Currency.CurrencyDict[ECurrency.DungeonTicket] <= 0)
        {
            return;
        }

        Managers.Game.SelectDungeonType = _dungeonData.DungeonType;
		Managers.Backend.GameData.Currency.RemoveAmount(ECurrency.DungeonTicket, 1);
        Managers.Scene.LoadScene(EScene.Dungeon);
    }

	private void OnClickSweep(PointerEventData data)
	{
        if (Managers.Backend.GameData.Currency.CurrencyDict[ECurrency.DungeonTicket] <= 0)
        {
            return;
        }

        Managers.Backend.GameData.Currency.RemoveAmount(ECurrency.DungeonTicket, 1);
		Managers.Game.HandleReward(_dungeonData.RewardType, _dungeonData.RewardValue);
    }

    #endregion
}
