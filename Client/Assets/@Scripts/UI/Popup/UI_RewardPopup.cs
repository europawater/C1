using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Reward
{
    public ERewardType RewardType { get; private set; }
    public int RewardAmount { get; private set; }

    public Reward(ERewardType rewardType, int rewardAmount)
    {
        RewardType = rewardType;
        RewardAmount = rewardAmount;
    }
}

public class UI_RewardPopup : UI_Popup
{
    private enum GameObjects
    {
        Content_Reward,
    }

    private enum Buttons
    {
        Button_Config,
    }

    private List<UI_RewardSlot> _slotList = new List<UI_RewardSlot>();

    private List<Reward> _rewardList = new List<Reward>();

    protected override void Awake()
    {
        base.Awake();

        // Bind
        BindGameObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));

        // Init
        foreach(Transform child in GetGameObject((int)GameObjects.Content_Reward).transform)
        {
            UI_RewardSlot slot = child.GetComponent<UI_RewardSlot>();
            _slotList.Add(slot);
        }

        // Bind Event
        GetButton((int)Buttons.Button_Config).onClick.AddListener(OnClickConfig);
    }

    public void SetInfo(List<Reward> rewardList)
    {
        _rewardList = rewardList;

        RefreshUI();
    }

    private void RefreshUI()
    {
        for (int index = 0; index < _slotList.Count; index++)
        {
            if (index < _rewardList.Count)
            {
                _slotList[index].SetInfo(_rewardList[index].RewardType, _rewardList[index].RewardAmount);
                _slotList[index].gameObject.SetActive(true);
            }
            else
            {
                _slotList[index].gameObject.SetActive(false);
            }
        }
    }

    #region UI Event

    private void OnClickConfig()
    {
        ClosePopupUI();
    }

    #endregion
}
