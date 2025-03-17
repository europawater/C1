using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_ShopPopup : UI_Popup
{
	private enum GameObjects
	{
		CloseArea,

		SkillArea,
		BuddyArea,
		GoodsArea,
	}

	private enum Buttons
	{
		Button_Close,

		Button_SkillGachaCount10,
		Button_BuddyGachaCount10,
		Button_InApp,
		Button_AD,
	}

	private enum Toggles
	{
		Toggle_SkillGacha,
		Toggle_BuddyGacha,
		Toggle_Goods,
	}

	private enum ShopPopupState
	{ 
		None,
		SkillGacha,
		BuddyGacha,
		Goods,
	}

	private ShopPopupState _shopPopupState = ShopPopupState.None;

	private Toggle _skillToggle;
	private Toggle _buddyToggle;
	private Toggle _goodsToggle;

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindButtons(typeof(Buttons));
		BindToggles(typeof(Toggles));

		// Init
		_skillToggle = GetToggle((int)Toggles.Toggle_SkillGacha);
		_buddyToggle = GetToggle((int)Toggles.Toggle_BuddyGacha);
		_goodsToggle = GetToggle((int)Toggles.Toggle_Goods);

		// Bind Event
		GetGameObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
		GetButton((int)Buttons.Button_Close).gameObject.BindEvent(OnClickCloseArea);

		_skillToggle.gameObject.BindEvent(OnClickSkillToggle);
		_buddyToggle.gameObject.BindEvent(OnClickBuddyToggle);
		_goodsToggle.gameObject.BindEvent(OnClickGoodsToggle);

		GetButton((int)Buttons.Button_SkillGachaCount10).gameObject.BindEvent(OnClickSkillGachaCount10);
		GetButton((int)Buttons.Button_BuddyGachaCount10).gameObject.BindEvent(OnClickBuddyGachaCount10);
		GetButton((int)Buttons.Button_InApp).gameObject.BindEvent(OnClickInApp);
		GetButton((int)Buttons.Button_AD).gameObject.BindEvent(OnClickAD);
	}

	public void SetInfo()
	{
		_shopPopupState = ShopPopupState.SkillGacha;

		RefreshUI();
	}

	private void RefreshUI()
	{
		switch (_shopPopupState)
		{
			case ShopPopupState.SkillGacha:
				GetGameObject((int)GameObjects.SkillArea).SetActive(true);
				GetGameObject((int)GameObjects.BuddyArea).SetActive(false);
				GetGameObject((int)GameObjects.GoodsArea).SetActive(false);
				break;
			case ShopPopupState.BuddyGacha:
				GetGameObject((int)GameObjects.SkillArea).SetActive(false);
				GetGameObject((int)GameObjects.BuddyArea).SetActive(true);
				GetGameObject((int)GameObjects.GoodsArea).SetActive(false);
				break;
			case ShopPopupState.Goods:
				GetGameObject((int)GameObjects.SkillArea).SetActive(false);
				GetGameObject((int)GameObjects.BuddyArea).SetActive(false);
				GetGameObject((int)GameObjects.GoodsArea).SetActive(true);
				break;

			default:
				break;
		}
	}

	#region UI Event

	private void OnClickCloseArea(PointerEventData data)
	{
		ClosePopupUI();
	}

	private void OnClickSkillToggle(PointerEventData data)
	{
		_shopPopupState = ShopPopupState.SkillGacha;
		RefreshUI();
	}

	private void OnClickBuddyToggle(PointerEventData data)
	{
		_shopPopupState = ShopPopupState.BuddyGacha;
		RefreshUI();
	}

	private void OnClickGoodsToggle(PointerEventData data)
	{
		_shopPopupState = ShopPopupState.Goods;
		RefreshUI();
	}

	private void OnClickSkillGachaCount10(PointerEventData data)
	{
		if (Managers.Backend.GameData.Currency.CurrencyDict[ECurrency.Diamond] >= 1100)
		{
			Managers.Game.HandleDrawSkill();
			Managers.Backend.GameData.Currency.RemoveAmount(ECurrency.Diamond, 1100);
		}
	}

	private void OnClickBuddyGachaCount10(PointerEventData data)
	{
		if (Managers.Backend.GameData.Currency.CurrencyDict[ECurrency.Diamond] >= 1100)
		{
			Managers.Game.HandleDrawBuddy();
			Managers.Backend.GameData.Currency.RemoveAmount(ECurrency.Diamond, 1100);
		}
	}

	private void OnClickInApp(PointerEventData data)
	{
		// TODO: 인앱결제 완료 후 진행
		Managers.Backend.GameData.Currency.AddAmount(ECurrency.Diamond, 11000);
	}

	private void OnClickAD(PointerEventData data)
	{
		Managers.Ads.ShowDailyFreeGemRewardedAd(() =>
		{
			Managers.Backend.GameData.Currency.AddAmount(ECurrency.Diamond, 1100);
		});
	}

	#endregion
}
