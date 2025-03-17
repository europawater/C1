using BackEnd;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_TitleScene : UI_Scene
{
	private enum GameObjects
	{
		TouchArea,
		LoginArea,
		LoadingArea,

		NickName,
	}

	private enum Texts
	{
		Text_Loading,
		Text_NameInfo,
	}

	private enum Images
	{
		Image_Touch,
	}

	private enum Buttons
	{
		Button_GuestLogin,
		Button_NickName,
	}

	private enum Sliders
	{
		Slider_Loading,
	}

	private enum InputFields
	{
		InputField_NickName,
	}

	private enum TitleSceneUIState
	{
		None,
		Touch,
		Login,
		NickName,
		Loading,
	}

	private TitleSceneUIState _currentUIState = TitleSceneUIState.None;
	private TitleSceneUIState CurrentUIState
	{
		get { return _currentUIState; }
		set
		{
			if (_currentUIState == value)
			{
				return;
			}

			_currentUIState = value;

			switch (_currentUIState)
			{
				case TitleSceneUIState.Touch:
					GetGameObject((int)GameObjects.TouchArea).SetActive(true);
					GetGameObject((int)GameObjects.LoginArea).SetActive(false);
					GetGameObject((int)GameObjects.LoadingArea).SetActive(false);
					GetGameObject((int)GameObjects.NickName).SetActive(false);
					break;
				case TitleSceneUIState.Login:
					GetGameObject((int)GameObjects.TouchArea).SetActive(false);
					GetGameObject((int)GameObjects.LoginArea).SetActive(true);
					GetGameObject((int)GameObjects.LoadingArea).SetActive(false);
					GetGameObject((int)GameObjects.NickName).SetActive(false);
					break;
				case TitleSceneUIState.NickName:
					GetGameObject((int)GameObjects.TouchArea).SetActive(false);
					GetGameObject((int)GameObjects.LoginArea).SetActive(false);
					GetGameObject((int)GameObjects.LoadingArea).SetActive(false);
					GetGameObject((int)GameObjects.NickName).SetActive(true);
					break;
				case TitleSceneUIState.Loading:
					GetGameObject((int)GameObjects.TouchArea).SetActive(false);
					GetGameObject((int)GameObjects.LoginArea).SetActive(false);
					GetGameObject((int)GameObjects.LoadingArea).SetActive(true);
					GetGameObject((int)GameObjects.NickName).SetActive(false);

					Managers.Backend.GetUserInfo((success, errorInfo) =>
					{
						if (success)
						{
							// �ε� ����
							HandleBackendInitializeStep(true, string.Empty, string.Empty, string.Empty);
						}
					});

					break;
			}
		}
	}

	private string _loadingText;
	private int _maxLoadingCount;
	private int _currentLoadingCount;

	private delegate void BackendLoadStep();
	private readonly Queue<BackendLoadStep> _initializeStep = new Queue<BackendLoadStep>();

	protected override void Awake()
	{
		base.Awake();

		// Bind
		BindGameObjects(typeof(GameObjects));
		BindTexts(typeof(Texts));
		BindImages(typeof(Images));
		BindButtons(typeof(Buttons));
		BindSliders(typeof(Sliders));
		BindInputFields(typeof(InputFields));

		// Bind Event
		GetImage((int)Images.Image_Touch).gameObject.BindEvent(OnClickTouchImage);
		GetButton((int)Buttons.Button_GuestLogin).gameObject.BindEvent(OnClickGuestLoginButton);
		GetButton((int)Buttons.Button_NickName).gameObject.BindEvent(OnClickNickNameButton);
	}

	public void SetInfo()
	{
		#region �ڳ� ������ �ε� �۾� ����

		_initializeStep.Clear();

		// Chart
		_initializeStep.Enqueue(() => { _loadingText = "�÷��̾� ���� ��Ʈ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.Chart.PlayerLevelChart.LoadChartData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "���� ��Ʈ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.Chart.HeroChart.LoadChartData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "���� ��Ʈ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.Chart.MonsterChart.LoadChartData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "���� ��ȭ ��Ʈ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.Chart.MonsterLevelScalingChart.LoadChartData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "��ų ��Ʈ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.Chart.SkillChart.LoadChartData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "�����͸� ��Ʈ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.Chart.MasteryChart.LoadChartData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "��� ��Ʈ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.Chart.EquipmentChart.LoadChartData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "��� ��ȭ ��Ʈ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.Chart.EquipmentLevelScalingChart.LoadChartData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "�������� ��Ʈ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.Chart.StageChart.LoadChartData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "���̽� ��Ʈ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.Chart.DiceChart.LoadChartData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "���� ��Ʈ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.Chart.RewardChart.LoadChartData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "���� ��Ʈ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.Chart.BuddyChart.LoadChartData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "���� ��Ʈ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.Chart.DungeonChart.LoadChartData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "�÷��� ��Ʈ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.Chart.CollectionChart.LoadChartData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "�̼� ��Ʈ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.Chart.MissionChart.LoadChartData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "���� ��Ʈ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.Chart.ShopChart.LoadChartData(HandleBackendInitializeStep); });
		// GameData
		_initializeStep.Enqueue(() => { _loadingText = "�÷��̾� ������ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.GameData.Player.LoadGameData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "��ȭ ������ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.GameData.Currency.LoadGameData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "�����͸� ������ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.GameData.Mastery.LoadGameData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "���̽� ������ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.GameData.Dice.LoadGameData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "��� ������ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.GameData.Equipment.LoadGameData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "��ų ������ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.GameData.Skill.LoadGameData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "���� ������ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.GameData.Buddy.LoadGameData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "�÷��� ������ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.GameData.Collection.LoadGameData(HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "�̼� ������ �ҷ����� ��..."; RefreshLoadingUI(); Managers.Backend.GameData.Mission.LoadGameData(HandleBackendInitializeStep); });
		// Post
		_initializeStep.Enqueue(() => { _loadingText = "������ ���� ���� �ҷ����� ��..."; RefreshLoadingUI(); Managers.Post.LoadPostList(PostType.Admin, HandleBackendInitializeStep); });
		_initializeStep.Enqueue(() => { _loadingText = "�������� ���� ���� �ҷ����� ��..."; RefreshLoadingUI(); Managers.Post.LoadPostList(PostType.Rank, HandleBackendInitializeStep); });
		// Rank
		_initializeStep.Enqueue(() => { _loadingText = "�������� �ҷ����� ��..."; RefreshLoadingUI(); Managers.Rank.LoadRankList(HandleBackendInitializeStep); });

		_loadingText = string.Empty;
		_maxLoadingCount = _initializeStep.Count;
		_currentLoadingCount = 0;

		GetSlider((int)Sliders.Slider_Loading).maxValue = _maxLoadingCount;
		GetSlider((int)Sliders.Slider_Loading).value = _currentLoadingCount;

		#endregion

		// ��ū �α��� �õ�
		Managers.Backend.LoginWithBackendToken((success, info) =>
		{
			if (success)
			{
				// ��ū �α��� ����
				CurrentUIState = TitleSceneUIState.Touch;
			}
			else
			{
				// ��ū �α��� ���� �ٸ� �α��� �õ��ؾ���..
				CurrentUIState = TitleSceneUIState.Login;
			}
		});

		RefreshUI();
	}

	private void RefreshUI()
	{
	}

	private void RefreshLoadingUI()
	{
		GetText((int)Texts.Text_Loading).text = $"{_loadingText}({_currentLoadingCount}/{_maxLoadingCount})";
	}

	private void HandleBackendInitializeStep(bool isSuccess, string className, string funcName, string errorInfo)
	{
		if (isSuccess)
		{
			_currentLoadingCount++;
			GetSlider((int)Sliders.Slider_Loading).value = _currentLoadingCount;

			if (_initializeStep.Count > 0)
			{
				_initializeStep.Dequeue().Invoke();
			}
			else
			{
				Managers.Scene.LoadScene(EScene.Game);
			}
		}
		else
		{
			Debug.LogError($"Error : {className} - {funcName} - {errorInfo}");
		}
	}

	#region UI Event

	private void OnClickTouchImage(PointerEventData data)
	{
		CurrentUIState = TitleSceneUIState.Loading;
	}

	private void OnClickGuestLoginButton(PointerEventData data)
	{
		Managers.Backend.GuestLogin((success, firstLogin, info) =>
		{
			if (success)
			{
				if (firstLogin)
				{
					CurrentUIState = TitleSceneUIState.NickName;
				}
				else
				{
					// �Խ�Ʈ �α��� ����
					CurrentUIState = TitleSceneUIState.Loading;
				}
			}
		});
	}

	private void OnClickNickNameButton(PointerEventData data)
	{
		string nickName = GetInputField((int)InputFields.InputField_NickName).text;

		Managers.Backend.UpdateNickName(nickName, (success, info) =>
		{
			if (success)
			{
				// �Խ�Ʈ �α��� ����
				CurrentUIState = TitleSceneUIState.Loading;
			}
			else
			{
				GetGameObject((int)GameObjects.NickName).GetComponent<Animation>().Play();
				GetText((int)Texts.Text_NameInfo).text = info;
			}
		});
	}

	#endregion
}
