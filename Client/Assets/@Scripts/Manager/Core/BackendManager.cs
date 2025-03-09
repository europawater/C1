using BackEnd;
using LitJson;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using UnityEngine;
using static Define;

/// <summary>뒤끝 콘솔에 업로드한 차트(데이터 시트) 데이터들 입니다.</summary>
public class BackendChart
{
	/// <summary>전체 차트의 파일 id를 관리하는 Dictionary</summary>
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
		// 차트 ID 불러오기
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
					Debug.LogError($"동일한 차트 키 값이 존재합니다 : {chartName} - {selectedChartFileId}");
				}
				else
				{
					_allChartIDDict.Add(chartName, selectedChartFileId);
				}
			}
		});
	}
}

/// <summary>뒤끝 콘솔에서 관리하는 게임 데이터들 입니다.</summary>
public class BackendGameData
{
	/// <summary>게임 데이터 집합입니다.</summary>
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

	/// <summary>치명적인 에러 발생 여부를 확인합니다.</summary>
	private bool _isErrorOccured = false;

	public void Init(Action onBackendInit)
	{
		var initializeBro = Backend.Initialize(); // 뒤끝 초기화

		if (initializeBro.IsSuccess())
		{
			Debug.Log("Backend 초기화 성공 : " + initializeBro);

			CreateSendQueueMgr();
			SetErrorHandler();
			UpdateBackend();

			// FIXME : 스킬작업 후 모든 데이터는 뒤끝에서 처리하기 대문에 해당 함수는 주석처리합니다.
			onBackendInit?.Invoke();
		}
		else
		{
			Debug.LogError("Backend 초기화 실패 : " + initializeBro);
		}
	}

	/// <summary>SendQueueMgr(뒤끝 SendQueue 관리) 오브젝트를 생성합니다.</summary>
	/// *SendQueue는 함수 호출 시 바로 호출하지 않고 큐에 적재한 후 순차적으로 함수를 호출하는 방식입니다.
	private void CreateSendQueueMgr()
	{
		UnityEngine.Object sendQueueMgr = Managers.Resource.Instantiate("SendQueueMgr");
		sendQueueMgr.name = "SendQueueMgr";
		UnityEngine.Object.DontDestroyOnLoad(sendQueueMgr);
	}

	/// <summary>뒤끝 에러 핸들러를 설정합니다.</summary>
	private void SetErrorHandler()
	{
		Backend.ErrorHandler.OnOtherDeviceLoginDetectedError = () =>
		{
			Debug.Log("다른 장치에서 동일한 계정으로 로그인이 감지되었습니다.");
			StopUpdateBackend();
		};

		Backend.ErrorHandler.OnMaintenanceError = () =>
		{
			Debug.Log("서버 점검중 입니다");
			StopUpdateBackend();
		};

		Backend.ErrorHandler.OnTooManyRequestError = () =>
		{
			Debug.Log("클라이언트가 너무 많은 요청을 보냈습니다.");
			StopUpdateBackend();
		};

		Backend.ErrorHandler.OnTooManyRequestByLocalError = () =>
		{
			Debug.Log("로컬에서 너무 많은 요청을 보냈습니다.");
			StopUpdateBackend();
		};

		Backend.ErrorHandler.OnDeviceBlockError = () =>
		{
			Debug.Log("차단된 계정으로 로그인을 시도했습니다.");
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
		Debug.LogError("뒤끝 업데이트를 중지합니다.");
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

		// 업데이트할 데이터가 있는지 확인
		List<BaseGameData> changedGameDataList = new List<BaseGameData>();
		foreach (BaseGameData gameData in GameData.GameDataDict.Values)
		{
			if (gameData.IsChangedBackendGameData)
			{
				info += gameData.GetTableName() + "\n";
				changedGameDataList.Add(gameData);
			}
		}

		// 업데이트할 목록이 존재하지 않음
		if (changedGameDataList.Count <= 0)
		{
			onUpdateComplete?.Invoke(null);
		}
		// 업데이트 목록이 하나라면 해당 테이블만 업데이트
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

						Debug.Log($"UpdateV2 : {bro}\n업데이트 테이블 : \n{info}");

						onUpdateComplete?.Invoke(bro);
					});
				}
			}
		}
		// 2개 이상이라면 트랜잭션에 묶어서 업데이트
		// *10개 이상이면 트랜잭션 실패할 수 있습니다.
		else
		{
			List<TransactionValue> transactionList = new List<TransactionValue>();

			// 변경된 데이터만큼 트랜잭션 추가
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
		// 우편 갯수 확인
		int postCount = Managers.Post.PostDict.Count;

		Managers.Post.LoadPostList(PostType.Admin, (success, className, funcName, errorInfo) =>
		{
			if (success)
			{
				// 우편 갯수 비교
				if (postCount != Managers.Post.PostDict.Count)
				{
					// 우편이 추가되었을 경우
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
		// 랭킹에 기준이되는 정보를 가지고 있는 데이터	추가
		List<BaseGameData> rankGameDataList = new List<BaseGameData>();
		rankGameDataList.Add(GameData.Player);

		foreach (LeaderBoard leaderBoard in Managers.Rank.LeaderBoardList)
		{
			// 'rankGameDataList'에 있는 테이블 이름과 'leaderBoard'을 비교 같다면 해당 게임 데이터를 업데이트한다.
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
					Debug.Log("랭킹 업데이트 성공");
				}
				else
				{
					SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), bro.ToString());
				}
			});
		}
	}

	private const int LOG_EXPIRATION_DAYS = 7;

	/// <summary>에러 발생시 게임로그 보내기</summary>
	public void SendBugReport(string className, string functionName, string errorInfo, int repeatCount = 3)
	{
		// 로그 보내기 실패 시 재귀함수를 통해 최대 3번까지 호출을 시도합니다.
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
			// 에러 발생 시 재귀호출
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
					Debug.Log("닉네임 업데이트 성공 : " + bro);

					isSuccess = true;
				}
				else
				{
					if (bro.GetStatusCode() == "400")
					{
						if (bro.GetMessage().Contains("undefined nickname"))
						{
							errorInfo = $"이름이 비어있습니다.";
						}
						else if (bro.GetMessage().Contains("bad nickname is too long"))
						{
							errorInfo = $"20자 이상은 입력할 수 없습니다.";
						}
						else if (bro.GetMessage().Contains("bad beginning or end"))
						{
							errorInfo = $"이름 앞 혹은 뒤에 공백이 존재합니다.";
						}
						else
						{
							errorInfo = $"알 수 없는 에러입니다.";
						}
					}
					else if (bro.GetStatusCode() == "409")
					{
						errorInfo = "이미 존재하는 닉네임입니다.";
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

	/// <summary>토큰을 이용한 자동로그인 함수 호출.</summary>
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
					Debug.Log("토큰 로그인 성공 : " + bro);

					// 차트 ID 불러오기
					Chart.LoadAllChartID();
					// 게임 데이터 설정(로드x)
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

		// 게스트 정보 삭제
		Backend.BMember.DeleteGuestInfo();

		SendQueue.Enqueue(Backend.BMember.GuestLogin, bro =>
		{
			try
			{
				if (bro.IsSuccess())
				{
					// 새로 가입인 경우에는 statusCode가 201, 기존 로그인일 경우에는 200이 리턴된다.
					if (bro.GetStatusCode() == "201")
					{
						// TODO : 개인정보보호정책 동의창..

						// 차트 ID 불러오기
						Chart.LoadAllChartID();
						// 게임 데이터 설정(로드x)
						GameData.SetGameDataDict();

						isFirstLogin = true;
					}
					else
					{
						Debug.Log("게스트 로그인 성공 : " + bro);

						// 차트 ID 불러오기
						Chart.LoadAllChartID();
						// 게임 데이터 설정(로드x)
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
