using UnityEngine;
using static Define;

public class UI_EnchantStatSlot : UI_Slot
{
	private enum Texts
	{ 
		Text_State,
		Text_BeforeAbility,
		Text_IncreaseValue,
	}

	private EStat _stat;
	private float _value;
	private float _increaseValue;

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindTexts(typeof(Texts));
	}

	public void SetInfo(EStat stat, float value, float increaseValue)
	{
		_stat = stat;
		_value = value;
		_increaseValue = increaseValue;

		RefreshUI();
	}

	private void RefreshUI()
	{ 
		GetText((int)Texts.Text_State).text = Util.GetStatusString(_stat);
		GetText((int)Texts.Text_BeforeAbility).text = $"{_value:N0}";
		GetText((int)Texts.Text_IncreaseValue).text = $"+{_increaseValue:N0}";
	}
}
