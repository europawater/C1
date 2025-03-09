using UnityEngine;
using static Define;

public class UI_EquipmentStatSlot : UI_Slot
{
	private enum Texts
	{
		Text_StatValue,
	}

	private enum Images
	{ 
		Image_StatUp,
		Image_StatDown,
	}

	private EStat _stat;
	private float _statValue;
	private float _compareValue;

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindTexts(typeof(Texts));
		BindImages(typeof(Images));
	}

	public void SetInfo(EStat stat, float statValue, float compareValue)
	{
		_stat = stat;
		_statValue = statValue;
		_compareValue = compareValue;

		RefreshUI();
	}

	private void RefreshUI()
	{
		GetText((int)Texts.Text_StatValue).text = $"{Util.GetStatusString(_stat)} {_statValue:N0}";

		if (_statValue > _compareValue)
		{
			GetImage((int)Images.Image_StatUp).gameObject.SetActive(true);
			GetImage((int)Images.Image_StatDown).gameObject.SetActive(false);
		}
		else if (_statValue < _compareValue)
		{
			GetImage((int)Images.Image_StatUp).gameObject.SetActive(false);
			GetImage((int)Images.Image_StatDown).gameObject.SetActive(true);
		}
		else
		{
			GetImage((int)Images.Image_StatUp).gameObject.SetActive(false);
			GetImage((int)Images.Image_StatDown).gameObject.SetActive(false);
		}
	}
}
