using UnityEngine;
using static Define;

public class UI_EquipmentPopup : UI_Popup
{
	private enum GameObjects
	{
		UI_OldEquipmentSub,
		UI_NewEquipmentSub,
	}

	private Equipment _oldEquipment;
	private Equipment _newEquipment;

	private UI_OldEquipmentSub _oldEquipmentSub;
	private UI_NewEquipmentSub _newEquipmentSub;

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));

		// Init
		_oldEquipmentSub = GetGameObject((int)GameObjects.UI_OldEquipmentSub).GetComponent<UI_OldEquipmentSub>();
		_newEquipmentSub = GetGameObject((int)GameObjects.UI_NewEquipmentSub).GetComponent<UI_NewEquipmentSub>();
	}

	private void OnEnable()
	{
		Managers.Event.AddEvent(EEventType.OnEquipmentChanged, CloseEquipmentPopupUI);
	}

	private void OnDisable()
	{
		Managers.Event.RemoveEvent(EEventType.OnEquipmentChanged, CloseEquipmentPopupUI);
	}

	public void SetInfo(Equipment oldEquipment, Equipment newEquipment)
	{
		_oldEquipment = oldEquipment;
		_newEquipment = newEquipment;

		RefreshUI();
	}

	private void RefreshUI()
	{
		if (_oldEquipment == null)
		{
			_oldEquipmentSub.gameObject.SetActive(false);
		}
		else
		{ 
			_oldEquipmentSub.SetInfo(_oldEquipment);
			_oldEquipmentSub.gameObject.SetActive(true);
		}

		_newEquipmentSub.SetInfo(_oldEquipment, _newEquipment);
	}

	private void CloseEquipmentPopupUI()
	{
		ClosePopupUI();
	}
}
