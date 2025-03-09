using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_GameScene : UI_Scene
{
	private enum GameObjects
	{
		SkillArea,

		// Dice
		Dice,

		// Equipment
		UI_EquipmentSlot_Weapon,
		UI_EquipmentSlot_Helmet,
		UI_EquipmentSlot_Armor,
		UI_EquipmentSlot_Pants,
		UI_EquipmentSlot_Pauldrons,
		UI_EquipmentSlot_Gloves,
		UI_EquipmentSlot_Necklace,
		UI_EquipmentSlot_Ring,
	}

	private enum Texts
	{
		// Player
		Text_Nickname,
		Text_Level,
		Text_Experience,
		Text_BattlePower,

		// Stage Info
		Text_Stage,

		// Currency
		Text_GoldAmount,
		Text_DiamondAmount,

		// Dice
		Text_DiceLevel,
		Text_DiceCount,
	}

	private enum Buttons
	{
		// Stage
		Button_Boss,

		// Dice
		Button_Dice,

		// Popup
		Button_Hero,

		// Side Popup
		Button_Post,
		Button_Rank,
	}

	private enum Sliders
	{
		// Player
		Slider_Experience,

		// Stage Info
		Slider_Stage,
	}

	private GameScene _gameScene;
	private Animator _gameSceneUIAnimator;

	// Skill
	private List<UI_EquipmentSkillSlot> _equipmentSkillSlotUIList = new List<UI_EquipmentSkillSlot>();

	// Equipment
	private Dictionary<EEquipmentPart, UI_EquipmentSlot> _equipmentSlotDict = new Dictionary<EEquipmentPart, UI_EquipmentSlot>();

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindTexts(typeof(Texts));
		BindButtons(typeof(Buttons));
		BindSliders(typeof(Sliders));

        // Init
        _gameSceneUIAnimator = GetComponent<Animator>();

        foreach (Transform child in GetGameObject((int)GameObjects.SkillArea).transform)
        {
            UI_EquipmentSkillSlot skillSlotUI = child.GetComponent<UI_EquipmentSkillSlot>();
            if (skillSlotUI != null)
            {
                _equipmentSkillSlotUIList.Add(skillSlotUI);
            }
        }

        _equipmentSlotDict.Add(EEquipmentPart.Weapon, GetGameObject((int)GameObjects.UI_EquipmentSlot_Weapon).GetComponent<UI_EquipmentSlot>());
        _equipmentSlotDict.Add(EEquipmentPart.Helmet, GetGameObject((int)GameObjects.UI_EquipmentSlot_Helmet).GetComponent<UI_EquipmentSlot>());
        _equipmentSlotDict.Add(EEquipmentPart.Armor, GetGameObject((int)GameObjects.UI_EquipmentSlot_Armor).GetComponent<UI_EquipmentSlot>());
        _equipmentSlotDict.Add(EEquipmentPart.Pants, GetGameObject((int)GameObjects.UI_EquipmentSlot_Pants).GetComponent<UI_EquipmentSlot>());
        _equipmentSlotDict.Add(EEquipmentPart.Pauldrons, GetGameObject((int)GameObjects.UI_EquipmentSlot_Pauldrons).GetComponent<UI_EquipmentSlot>());
        _equipmentSlotDict.Add(EEquipmentPart.Gloves, GetGameObject((int)GameObjects.UI_EquipmentSlot_Gloves).GetComponent<UI_EquipmentSlot>());
        _equipmentSlotDict.Add(EEquipmentPart.Necklace, GetGameObject((int)GameObjects.UI_EquipmentSlot_Necklace).GetComponent<UI_EquipmentSlot>());
        _equipmentSlotDict.Add(EEquipmentPart.Ring, GetGameObject((int)GameObjects.UI_EquipmentSlot_Ring).GetComponent<UI_EquipmentSlot>());

        // Bind Event
        GetButton((int)Buttons.Button_Boss).gameObject.BindEvent(OnClickBossButton);
		GetButton((int)Buttons.Button_Dice).gameObject.BindEvent(OnClickDiceButton);
		GetButton((int)Buttons.Button_Hero).gameObject.BindEvent(OnClickHeroButton);
		GetButton((int)Buttons.Button_Post).gameObject.BindEvent(OnClickPostButton);
		GetButton((int)Buttons.Button_Rank).gameObject.BindEvent(OnClickRankButton);
	}

	private void OnEnable()
	{
		Managers.Event.AddEvent(EEventType.OnCurrencyChanged, RefreshCurrencyTextUI);
		Managers.Event.AddEvent(EEventType.OnStageWaveIndexChanged, RefreshSliderStageUI);
		Managers.Event.AddEvent(EEventType.OnEquipmentChanged, RefreshEquipmentSlotUI);
		Managers.Event.AddEvent(EEventType.OnDiceChanged, RefreshDiceUI);
		Managers.Event.AddEvent(EEventType.OnSkillChanged, RefreshEquipmentSkillSlotUI);
		Managers.Event.AddEvent(EEventType.OnPlayerChanged, RefreshPlayerUI);
	}

	private void OnDisable()
	{
		Managers.Event.RemoveEvent(EEventType.OnCurrencyChanged, RefreshCurrencyTextUI);
		Managers.Event.RemoveEvent(EEventType.OnStageWaveIndexChanged, RefreshSliderStageUI);
		Managers.Event.RemoveEvent(EEventType.OnEquipmentChanged, RefreshEquipmentSlotUI);
		Managers.Event.RemoveEvent(EEventType.OnDiceChanged, RefreshDiceUI);
		Managers.Event.RemoveEvent(EEventType.OnSkillChanged, RefreshEquipmentSkillSlotUI);
		Managers.Event.RemoveEvent(EEventType.OnPlayerChanged, RefreshPlayerUI);
	}

	public void SetInfo(GameScene gameScene)
	{
		_gameScene = gameScene;

        Managers.Game.SetDiceObject(GetGameObject((int)GameObjects.Dice).GetComponent<Dice>());

		GetText((int)Texts.Text_Nickname).text = $"{Managers.Backend.NickName}";

		RefreshUI();
	}

	private void RefreshUI()
	{
		// Game Info
		GetText((int)Texts.Text_Stage).text = $"Stage {_gameScene.StageData.Remark}";

		RefreshEquipmentSkillSlotUI();
		RefreshCurrencyTextUI();
		RefreshSliderStageUI();
		RefreshEquipmentSlotUI();
		RefreshDiceUI();
		RefreshPlayerUI();
	}

	private void RefreshEquipmentSkillSlotUI()
	{
		for (int slotIndex = 0; slotIndex < _equipmentSkillSlotUIList.Count; slotIndex++)
		{
			Skill equippedSkill = null;
			if (!Managers.Game.EquippedSkillSlotDict.TryGetValue((ESkillSlot)slotIndex, out equippedSkill))
			{
				_equipmentSkillSlotUIList[slotIndex].SetInfo(slotIndex, UI_EquipmentSkillSlot.EquipmentSkillSlotToUse.GameScene, null);
			}
			else
			{
				_equipmentSkillSlotUIList[slotIndex].SetInfo(slotIndex, UI_EquipmentSkillSlot.EquipmentSkillSlotToUse.GameScene, equippedSkill);
			}
		}
	}

	private void RefreshCurrencyTextUI()
	{
		GetText((int)Texts.Text_GoldAmount).text = $"{Managers.Backend.GameData.Currency.CurrencyDict[ECurrency.Gold]:N0}";
		GetText((int)Texts.Text_DiamondAmount).text = $"{Managers.Backend.GameData.Currency.CurrencyDict[ECurrency.Diamond]:N0}";
	}

	private const int MAX_STAGE_WAVE_INDEX = 5;
	private void RefreshSliderStageUI()
	{
		GetSlider((int)Sliders.Slider_Stage).value = (float)_gameScene.StageWaveIndex / MAX_STAGE_WAVE_INDEX;
	}

	private void RefreshEquipmentSlotUI()
	{
		foreach (var equipmentSlot in _equipmentSlotDict)
		{
			equipmentSlot.Value.SetInfo(UI_EquipmentSlot.EquipmentSlotToUse.GameScene, Managers.Game.EquipmentDict[equipmentSlot.Key]);
		}
	}

	private void RefreshDiceUI()
	{
		GetText((int)Texts.Text_DiceLevel).text = $"Lv.{Managers.Backend.GameData.Dice.DiceLevel}";
		GetText((int)Texts.Text_DiceCount).text = $"{Managers.Backend.GameData.Dice.DiceCount:N0}";
	}

	private void RefreshPlayerUI()
	{
		GetText((int)Texts.Text_Level).text = $"Lv.{Managers.Backend.GameData.Player.PlayerLevel}";
		GetText((int)Texts.Text_Experience).text = $"{Managers.Backend.GameData.Player.EXP:N0}/{Managers.Backend.GameData.Player.NeedToLevelUpEXP:N0}";
		GetText((int)Texts.Text_BattlePower).text = $"{100}"; // TODO : BattlePower °è»ê
		GetSlider((int)Sliders.Slider_Experience).value = (float)Managers.Backend.GameData.Player.EXP / Managers.Backend.GameData.Player.NeedToLevelUpEXP;
	}

	#region Animation

	public void PlayAnimation(string triggerName)
	{
		_gameSceneUIAnimator.Rebind();
		_gameSceneUIAnimator.CrossFade(triggerName, 0.0f);
	}

	#endregion

	#region UI Event

	private void OnClickBossButton(PointerEventData data)
	{
	}

	private void OnClickDiceButton(PointerEventData data)
	{
		if (!Managers.Game.CanDrawEquipment())
		{
			return;
		}

		Managers.Game.Dice.DiceState = EDiceState.Draw;
	}

	private void OnClickHeroButton(PointerEventData data)
	{
		UI_HeroPopup popup = Managers.UI.ShowPopupUI<UI_HeroPopup>();
		popup.SetInfo();
	}

	private void OnClickPostButton(PointerEventData data)
	{
		UI_PostPopup popup = Managers.UI.ShowPopupUI<UI_PostPopup>();
		popup.SetInfo();
	}

	private void OnClickRankButton(PointerEventData data)
	{
		UI_RankPopup popup = Managers.UI.ShowPopupUI<UI_RankPopup>();
		popup.SetInfo();
	}

	#endregion
}
