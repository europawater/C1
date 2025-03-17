using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_EquipmentPopup : UI_Popup
{
    private enum GameObjects
    {
        UI_OldEquipmentSub,
        UI_NewEquipmentSub,
    }

    private enum Images
    {
        Image_Registration,
    }

    private enum Buttons
    {
        Button_Collection,
        Button_Enchant,
    }

    private Equipment _oldEquipment;

    private UI_OldEquipmentSub _oldEquipmentSub;
    private UI_NewEquipmentSub _newEquipmentSub;

    protected override void Awake()
    {
        base.Awake();

        // Bind
        BindGameObjects(typeof(GameObjects));
        BindImages(typeof(Images));
        BindButtons(typeof(Buttons));

        // Init
        _oldEquipmentSub = GetGameObject((int)GameObjects.UI_OldEquipmentSub).GetComponent<UI_OldEquipmentSub>();
        _newEquipmentSub = GetGameObject((int)GameObjects.UI_NewEquipmentSub).GetComponent<UI_NewEquipmentSub>();

        // Bind Event
        GetButton((int)Buttons.Button_Collection).gameObject.BindEvent(OnClickCollection);
        GetButton((int)Buttons.Button_Enchant).gameObject.BindEvent(OnClickEnchant);
    }

    private void OnEnable()
    {
        Managers.Event.AddEvent(EEventType.OnEquipmentChanged, CloseEquipmentPopupUI);
        Managers.Event.AddEvent(EEventType.OnCollectionChanged, CloseEquipmentPopupUI);
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent(EEventType.OnEquipmentChanged, CloseEquipmentPopupUI);
        Managers.Event.RemoveEvent(EEventType.OnCollectionChanged, CloseEquipmentPopupUI);
    }

    public void SetInfo(Equipment oldEquipment)
    {
        _oldEquipment = oldEquipment;

        RefreshUI();
    }

    public void RefreshUI()
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

        _newEquipmentSub.SetInfo(_oldEquipment);

        bool canRegist = Managers.Game.CanRegistCollection(ECollectionType.Item, Managers.Game.NewEquipment.EquipmentInfo.TemplateID);
        GetButton((int)Buttons.Button_Collection).interactable = canRegist;
        GetImage((int)Images.Image_Registration).gameObject.SetActive(canRegist);
    }

    private void CloseEquipmentPopupUI()
    {
        ClosePopupUI();
    }

    #region UI Event

    private void OnClickCollection(PointerEventData data)
    {
        if (GetButton((int)Buttons.Button_Collection).interactable)
        {
            Managers.Game.RegistCollection(ECollectionType.Item, Managers.Game.NewEquipment.EquipmentInfo.TemplateID);
        }
    }

    private void OnClickEnchant(PointerEventData data)
    {
        UI_EnchantPopup popup = Managers.UI.ShowPopupUI<UI_EnchantPopup>();
        popup.SetInfo(this);
    }

    #endregion
}
