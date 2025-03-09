using BackEnd;
using LitJson;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using UnityEngine;
using static Define;

/// <summary>�ڳ� �ֿܼ� ���ε��� ��Ʈ(������ ��Ʈ) �����͵� �Դϴ�.</summary>
public class BackendChart
{
	/// <summary>��ü ��Ʈ�� ���� id�� �����ϴ� Dictionary</summary>
	private readonly Dictionary<string, string> _allChartIDDict = new Dictionary<string, string>();
	public IReadOnlyDictionary<string, string> AllChartIDDict => _allChartIDDict;

	// Player Chart
	public readonly PlayerLevelChart PlayerLevelChart = new PlayerLevelChart();

	// Object Chart
	public readonly HeroChart HeroChart = new HeroChart();
	public readonly MonsterChart MonsterChart = new MonsterChart();
	public readonly MonsterLevelScalingChart MonsterLevelScalingChart = new MonsterLevelScalingChart();

	// Skill Chart
	public readonly SkillChart SkillChart = new SkillChart();

	// Mastery Chart
	public readonly MasteryChart MasteryChart = new MasteryChart();

	// Equipment Chart
	public readonly EquipmentChart EquipmentChart = new EquipmentChart();
	public readonly EquipmentLevelScalingChart EquipmentLevelScalingChart = new EquipmentLevelScalingChart();

	// Stage Chart
	public readonly StageChart StageChart = new StageChart();

	// Dice Chart
	public readonly DiceChart DiceChart = new DiceChart();

	// Reward Chart
	public readonly RewardChart RewardChart = new RewardChart();

	public void LoadAllChartID()
	{
		// ��Ʈ ID �ҷ�����
		SendQueue.Enqueue(Backend.Chart.GetChartList, bro =>
		{
			if (!bro.IsSuccess())
			{
				Debug.LogError(bro.ToString());
			}

			JsonData json = bro.FlattenRows();

			for (int i = 0; i < json.Count; i++)
			{
				string chartName = json[i]["chartName"].ToString();
				string selectedChartFileId = json[i]["selectedChartFileId"].ToString();

				if (_allChartIDDict.ContainsKey(chartName))
				{
					Debug.LogError($"������ ��Ʈ Ű ���� �����մϴ� : {chartName} - {selectedChartFileId}");
				}
				else
				{
					_allChartIDDict.Add(chartName, selectedChartFileId);
				}
			}
		});
	}
}

/// <summary>�ڳ� �ֿܼ��� �����ϴ� ���� �����͵� �Դϴ�.</summary>
public class BackendGameData
{
	/// <summary>���� ������ �����Դϴ�.</summary>
	public Dictionary<string, BaseGameData> _gameDataDict = new Dictionary<string, BaseGameData>();
	public Dictionary<string, BaseGameData> GameDataDict => _gameDataDict;

	public PlayerGameData Player { get; private set; } = new PlayerGameData();
	public CurrencyGameData Currency { get; private set; } = new CurrencyGameData();
	public MasteryGameData Mastery { get; private set; } = new MasteryGameData();
	public EquipmentGameData Equipment { get; private set; } = new EquipmentGameData();
	public DiceGameData Dice { get; private set; } = new DiceGameData();
	public SkillGameData Skill { get; private set; } = new SkillGameData();

	public void SetGameDataDict()
	{
		_gameDataDict.Clear();
		_gameDataDict.Add(Player.GetTableName(), Player);
		_gameDataDict.Add(Currency.GetTableName(), Currency);
		_gameDataDict.Add(Mastery.GetTableName(), Mastery);
		_gameDataDict.Add(Equipment.GetTableName(), Equipment);
		_gameDataDict.Add(Dice.GetTableName(), Dice);
		_gameDataDict.Add(Skill.GetTableName(), Skill);
	}
}

public class BackendManager
{
	public string NickName { get; private set; }
	public BackendChart Chart { get; private set; } = new();
	public BackendGameData GameData { get; private set; } = new();

	/// <summary>ġ������ ���� �߻� ���θ� Ȯ���մϴ�.</summary>
	private bool _isErrorOccured = false;

	public void Init(Action onBackendInit)
	{
		var initializeBro = Backend.Initialize(); // �ڳ� �ʱ�ȭ

		if (initializeBro.IsSuccess())
		{
			Debug.Log("Backend �ʱ�ȭ ���� : " + initializeBro);

			CreateSendQueueMgr();
			SetErrorHandler();
			UpdateBackend();

			// FIXME : ��ų�۾� �� ��� �����ʹ� �ڳ����� ó���ϱ� �빮�� �ش� �Լ��� �ּ�ó���մϴ�.
			onBackendInit?.Invoke();
		}
		else
		{
			Debug.LogError("Backend �ʱ�ȭ ���� : " + initializeBro);
		}
	}

	/// <summary>SendQueueMgr(�ڳ� SendQueue ����) ������Ʈ�� �����մϴ�.</summary>
	/// *SendQueue�� �Լ� ȣ�� �� �ٷ� ȣ������ �ʰ� ť�� ������ �� ���������� �Լ��� ȣ���ϴ� ����Դϴ�.
	private void CreateSendQueueMgr()
	{
		UnityEngine.Object sendQueueMgr = Managers.Resource.Instantiate("SendQueueMgr");
		sendQueueMgr.name = "SendQueueMgr";
		UnityEngine.Object.DontDestroyOnLoad(sendQueueMgr);
	}

	/// <summary>�ڳ� ���� �ڵ鷯�� �����մϴ�.</summary>
	private void SetErrorHandler()
	{
		Backend.ErrorHandler.OnOtherDeviceLoginDetectedError = () =>
		{
			Debug.Log("�ٸ� ��ġ���� ������ �������� �α����� �����Ǿ����ϴ�.");
			StopUpdateBackend();
		};

		Backend.ErrorHandler.OnMaintenanceError = () =>
		{
			Debug.Log("���� ������ �Դϴ�");
			StopUpdateBackend();
		};

		Backend.ErrorHandler.OnTooManyRequestError = () =>
		{
			Debug.Log("Ŭ���̾�Ʈ�� �ʹ� ���� ��û�� ���½��ϴ�.");
			StopUpdateBackend();
		};

		Backend.ErrorHandler.OnTooManyRequestByLocalError = () =>
		{
			Debug.Log("���ÿ��� �ʹ� ���� ��û�� ���½��ϴ�.");
			StopUpdateBackend();
		};

		Backend.ErrorHandler.OnDeviceBlockError = () =>
		{
			Debug.Log("���ܵ� �������� �α����� �õ��߽��ϴ�.");
			StopUpdateBackend();
		};
	}

	#region Update

	private void UpdateBackend()
	{
		Managers.Instance.StartCoroutine(CoUpdateGameDataTransaction());
		Managers.Instance.StartCoroutine(CoUpdatePostScore());
		Managers.Instance.StartCoroutine(CoUpdateRankScore());
	}

	public void StopUpdateBackend()
	{
		Debug.LogError("�ڳ� ������Ʈ�� �����մϴ�.");
		_isErrorOccured = true;
	}

	private const float DATA_UPDATE_TICK = 10.0f;
	private const float POST_UPDATE_TICK = 600.0f;
	private const float RANK_UPDATE_TICK = 10.0f;

	private IEnumerator CoUpdateGameDataTransaction()
	{
		while (!_isErrorOccured)
		{
			UpdateAllGameData(null);

			yield return new WaitForSeconds(DATA_UPDATE_TICK);
		}
	}

	private void UpdateAllGameData(Action<BackendReturnObject> onUpdateComplete)
	{
		string info = string.Empty;

		// ������Ʈ�� �����Ͱ� �ִ��� Ȯ��
		List<BaseGameData> changedGameDataList = new List<BaseGameData>();
		foreach (BaseGameData gameData in GameData.GameDataDict.Values)
		{
			if (gameData.IsChangedBackendGameData)
			{
				info += gameData.GetTableName() + "\n";
				changedGameDataList.Add(gameData);
			}
		}

		// ������Ʈ�� ����� �������� ����
		if (changedGameDataList.Count <= 0)
		{
			onUpdateComplete?.Invoke(null);
		}
		// ������Ʈ ����� �ϳ���� �ش� ���̺� ������Ʈ
		else if (changedGameDataList.Count == 1)
		{
			foreach (BaseGameData gameData in changedGameDataList)
			{
				if (gameData.IsChangedBackendGameData)
				{
					gameData.UpdateBackendGameData(bro =>
					{
						if (bro.IsSuccess())
						{
							gameData.ResetIsChangedBackendGameData();
						}
						else
						{
							SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), bro.ToString() + "\n" + info);
						}

						Debug.Log($"UpdateV2 : {bro}\n������Ʈ ���̺� : \n{info}");

						onUpdateComplete?.Invoke(bro);
					});
				}
			}
		}
		// 2�� �̻��̶�� Ʈ����ǿ� ��� ������Ʈ
		// *10�� �̻��̸� Ʈ����� ������ �� �ֽ��ϴ�.
		else
		{
			List<TransactionValue> transactionList = new List<TransactionValue>();

			// ����� �����͸�ŭ Ʈ����� �߰�
			foreach (BaseGameData gameData in changedGameDataList)
			{
				transactionList.Add(gameData.GetTransactionUpdateValue());
			}

			SendQueue.Enqueue(Backend.GameData.TransactionWriteV2, transactionList, bro =>
			{
				Debug.Log($"Backend.BMember.TransactionWriteV2 : {bro}");

				if (bro.IsSuccess())
				{
					foreach (BaseGameData gameData in changedGameDataList)
					{
						gameData.ResetIsChangedBackendGameData();
					}
				}
				else
				{
					SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), bro.ToString() + "\n" + info);
				}

				onUpdateComplete?.Invoke(bro);
			});
		}
	}

	private IEnumerator CoUpdatePostScore()
	{
		while (!_isErrorOccured)
		{
			UpdatePost();

			yield return new WaitForSeconds(POST_UPDATE_TICK);
		}
	}

	private void UpdatePost()
	{
		// ���� ���� Ȯ��
		int postCount = Managers.Post.PostDict.Count;

		Managers.Post.LoadPostList(PostType.Admin, (success, className, funcName, errorInfo) =>
		{
			if (success)
			{
				// ���� ���� ��
				if (postCount != Managers.Post.PostDict.Count)
				{
					// ������ �߰��Ǿ��� ���
					Managers.Event.TriggerEvent(EEventType.OnPostListChanged);
				}
			}
			else
			{
				SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), errorInfo);
			}
		});
	}

	private IEnumerator CoUpdateRankScore()
	{
		while (!_isErrorOccured)
		{
			UpdateRankScore();

			yield return new WaitForSeconds(RANK_UPDATE_TICK);
		}
	}

	private void UpdateRankScore()
	{
		// ��ŷ�� �����̵Ǵ� ������ ������ �ִ� ������	�߰�
		List<BaseGameData> rankGameDataList = new List<BaseGameData>();
		rankGameDataList.Add(GameData.Player);

		foreach (LeaderBoard leaderBoard in Managers.Rank.LeaderBoardList)
		{
			// 'rankGameDataList'�� �ִ� ���̺� �̸��� 'leaderBoard'�� �� ���ٸ� �ش� ���� �����͸� ������Ʈ�Ѵ�.
			int index = rankGameDataList.FindIndex(gameData => gameData.GetTableName() == leaderBoard.Table);
			if (index < 0)
			{
				continue;
			}

			SendQueue.Enqueue(Backend.URank.User.UpdateUserScore, leaderBoard.UUID, leaderBoard.Table, rankGameDataList[index].InData, rankGameDataList[index].GetParam(), bro =>
			{
				Debug.Log($"Backend.URank.User.UpdateUserScore : {bro}");

				if (bro.IsSuccess())
				{
					Debug.Log("��ŷ ������Ʈ ����");
				}
				else
				{
					SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), bro.ToString());
				}
			});
		}
	}

	private const int LOG_EXPIRATION_DAYS = 7;

	/// <summary>���� �߻��� ���ӷα� ������</summary>
	public void SendBugReport(string className, string functionName, string errorInfo, int repeatCount = 3)
	{
		// �α� ������ ���� �� ����Լ��� ���� �ִ� 3������ ȣ���� �õ��մϴ�.
		if (repeatCount <= 0)
		{
			return;
		}

		Param param = new Param();
		param.Add("className", className);
		param.Add("functionName", functionName);
		param.Add("errorPath", errorInfo);

		Backend.GameLog.InsertLogV2("error", param, LOG_EXPIRATION_DAYS, callback =>
		{
			// ���� �߻� �� ���ȣ��
			if (callback.IsSuccess() == false)
			{
				SendBugReport(className, functionName, errorInfo, repeatCount - 1);
			}
		});
	}

	#endregion

	#region Login

	public void GetUserInfo(Action<bool, string> onComplete)
	{
		bool isSuccess = false;
		string errorInfo = string.Empty;

		SendQueue.Enqueue(Backend.BMember.GetUserInfo, bro =>
		{
			try
			{
				if (bro.IsSuccess())
				{
					var json = bro.GetReturnValuetoJSON();
					NickName = json["row"]["nickname"].ToString();

					isSuccess = true;
				}
				else
				{
					errorInfo = bro.ToString();
				}
			}
			catch (Exception e)
			{
				errorInfo = e.ToString();
			}
			finally
			{
				onComplete?.Invoke(isSuccess, errorInfo);
			}
		});
	}

	public void UpdateNickName(string nickName, Action<bool, string> onComplete)
	{
		bool isSuccess = false;
		string errorInfo = string.Empty;

		SendQueue.Enqueue(Backend.BMember.UpdateNickname, nickName, bro =>
		{
			try
			{
				if (bro.IsSuccess())
				{
					Debug.Log("�г��� ������Ʈ ���� : " + bro);

					isSuccess = true;
				}
				else
				{
					if (bro.GetStatusCode() == "400")
					{
						if (bro.GetMessage().Contains("undefined nickname"))
						{
							errorInfo = $"�̸��� ����ֽ��ϴ�.";
						}
						else if (bro.GetMessage().Contains("bad nickname is too long"))
						{
							errorInfo = $"20�� �̻��� �Է��� �� �����ϴ�.";
						}
						else if (bro.GetMessage().Contains("bad beginning or end"))
						{
							errorInfo = $"�̸� �� Ȥ�� �ڿ� ������ �����մϴ�.";
						}
						else
						{
							errorInfo = $"�� �� ���� �����Դϴ�.";
						}
					}
					else if (bro.GetStatusCode() == "409")
					{
						errorInfo = "�̹� �����ϴ� �г����Դϴ�.";
					}
				}
			}
			catch (Exception e)
			{
				errorInfo = e.ToString();
			}
			finally
			{
				onComplete?.Invoke(isSuccess, errorInfo);
			}
		});
	}

	/// <summary>��ū�� �̿��� �ڵ��α��� �Լ� ȣ��.</summary>
	public void LoginWithBackendToken(Action<bool, string> onComplete)
	{
		bool isSuccess = false;
		string errorInfo = string.Empty;

		SendQueue.Enqueue(Backend.BMember.LoginWithTheBackendToken, bro =>
		{
			try
			{
				if (bro.IsSuccess())
				{
					Debug.Log("��ū �α��� ���� : " + bro);

					// ��Ʈ ID �ҷ�����
					Chart.LoadAllChartID();
					// ���� ������ ����(�ε�x)
					GameData.SetGameDataDict();

					isSuccess = true;
				}
				else
				{
					errorInfo = bro.ToString();
				}
			}
			catch (Exception e)
			{
				errorInfo = e.ToString();
			}
			finally
			{
				onComplete?.Invoke(isSuccess, errorInfo);
			}
		});
	}

	public void GuestLogin(Action<bool, bool, string> onComplete)
	{
		bool isSuccess = false;
		bool isFirstLogin = false;
		string errorInfo = string.Empty;

		// �Խ�Ʈ ���� ����
		Backend.BMember.DeleteGuestInfo();

		SendQueue.Enqueue(Backend.BMember.GuestLogin, bro =>
		{
			try
			{
				if (bro.IsSuccess())
				{
					// ���� ������ ��쿡�� statusCode�� 201, ���� �α����� ��쿡�� 200�� ���ϵȴ�.
					if (bro.GetStatusCode() == "201")
					{
						// TODO : ����������ȣ��å ����â..

						// ��Ʈ ID �ҷ�����
						Chart.LoadAllChartID();
						// ���� ������ ����(�ε�x)
						GameData.SetGameDataDict();

						isFirstLogin = true;
					}
					else
					{
						Debug.Log("�Խ�Ʈ �α��� ���� : " + bro);

						// ��Ʈ ID �ҷ�����
						Chart.LoadAllChartID();
						// ���� ������ ����(�ε�x)
						GameData.SetGameDataDict();
					}

					isSuccess = true;
				}
				else
				{
					errorInfo = bro.ToString();
				}
			}
			catch (Exception e)
			{
				errorInfo = e.ToString();
			}
			finally
			{
				onComplete?.Invoke(isSuccess, isFirstLogin, errorInfo);
			}
		});
	}

	#endregion
}
