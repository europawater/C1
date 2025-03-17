using UnityEngine;
using static Define;

public class UI_MissionRewardSlot : UI_Slot
{
    private enum GameObjects
    {
        VFX_ui_day_receive_slot,
    }

    private enum Texts
    {
        Text_RewardCount,
    }

    private enum Images
    {
        Image_RewardIcon,
    }

    private ERewardType _rewardType;
    private int _value;
    private bool _isActive;

    protected override void Awake()
    {
        base.Awake();

        // Bind
        BindGameObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));
        BindImages(typeof(Images));
    }

    public void SetInfo(ERewardType rewardType, int value, bool isActive)
    {
        _rewardType = rewardType;
        _value = value;
        _isActive = isActive;

        RefreshUI();
    }

    private void RefreshUI()
    { 
        GetGameObject((int)GameObjects.VFX_ui_day_receive_slot).SetActive(_isActive);
        GetText((int)Texts.Text_RewardCount).text = $"{_value:N0}";
        GetImage((int)Images.Image_RewardIcon).sprite = Managers.Resource.Load<Sprite>(Managers.Backend.Chart.RewardChart.DataDict[(int)_rewardType].IconKey);
        GetImage((int)Images.Image_RewardIcon).color = _isActive ? Color.white : Color.gray;
    }
}
