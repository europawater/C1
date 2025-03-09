using BackEnd;
using LitJson;
using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 뒤끝에서 관리하는 게임 데이터 입니다.
/// </summary>
public abstract class BaseGameData
{
	/// <summary>게임 데이터의 고유 식별자입니다.</summary>
	public string InData { get; private set; }
	/// <summary>데이터가 변경되었는지 여부를 나타냅니다.</summary>
	public bool IsChangedBackendGameData { get; private set; }
	/// <summary>뒤끝에 등록된 게임 데이터 테이블 이름을 참조합니다.</summary>
	public abstract string GetTableName();
	/// <summary>뒤끝에 등록된 게임 데이터 테이블에 삽입할 데이터를 반환합니다.</summary>
	public abstract Param GetParam();

	protected abstract void InitializeData();
	protected abstract void SetServerDataToLocal(JsonData gameDataJson);

	protected virtual void UpdateData()
	{
		IsChangedBackendGameData = true;
	}

	public void ResetIsChangedBackendGameData()
	{
		IsChangedBackendGameData = false;
	}

	public void LoadGameData(Action<bool, string, string, string> onComplete)
	{
		bool isSuccess = false;
		string className = GetType().Name;
		string funcName = MethodBase.GetCurrentMethod()?.Name;
		string errorInfo = string.Empty;

		string tableName = GetTableName();

		// 게임 정보 로드 함수
		SendQueue.Enqueue(Backend.GameData.GetMyData, tableName, new Where(), bro =>
		{
			try
			{
				Debug.Log($"Backend.GameData.GetMyData({tableName}) : {bro}");

				if (bro.IsSuccess())
				{
					if (bro.FlattenRows().Count > 0)
					{
						InData = bro.FlattenRows()[0]["inDate"].ToString();

						// 서버에서 로드된 데이터를 로컬 데이터로 설정
						SetServerDataToLocal(bro.FlattenRows()[0]);

						isSuccess = true;
						onComplete?.Invoke(isSuccess, className, funcName, errorInfo);
					}
					else
					{
						// 로드할 값이 없다면	 초기화 후 삽입
						// *SendQueue는 비동기적으로 동작하기 때문에, 삽입 함수를 호출할 때, SendQueue에 삽입 함수를 넣어놓고 호출해야함.
						InsertBackendGameData(onComplete);
					}
				}
				else
				{
					errorInfo = bro.ToString();
					onComplete?.Invoke(isSuccess, className, funcName, errorInfo);
				}
			}
			catch (Exception e)
			{
				errorInfo = e.ToString();
				onComplete?.Invoke(isSuccess, className, funcName, errorInfo);
			}
		});
	}

	/// <summary>최초 로드할 값이 없다면 로컬에서 초기화 후 뒤끝에 삽입하는 함수입니다.</summary>
	private void InsertBackendGameData(Action<bool, string, string, string> onComplete)
	{
		bool isSuccess = false;
		string className = GetType().Name;
		string funcName = MethodBase.GetCurrentMethod()?.Name;
		string errorInfo = string.Empty;

		string tableName = GetTableName();

		// 데이터 초기화
		InitializeData();

		// 게임 정보 삽입 함수
		SendQueue.Enqueue(Backend.GameData.Insert, tableName, GetParam(), bro =>
		{
			try
			{
				Debug.Log($"Backend.GameData.Insert({tableName}) : {bro}");

				if (bro.IsSuccess())
				{
					InData = bro.GetInDate();

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
				onComplete?.Invoke(isSuccess, className, funcName, errorInfo);
			}
		});
	}

	public void UpdateBackendGameData(Action<BackendReturnObject> onComplete)
	{
		// 게임 정보 업데이트 함수(V2)
		SendQueue.Enqueue(Backend.GameData.UpdateV2, GetTableName(), InData, Backend.UserInDate, GetParam(), bro =>
		{
			if (!bro.IsSuccess())
			{
				Debug.LogError(bro.ToString());
			}

			Debug.Log($"Backend.GameData.UpdateV2({GetTableName()}, {InData}, {Backend.UserInDate}) : {bro}");
			onComplete?.Invoke(bro);
		});
	}

	/// <summary>테이블에 업데이트할 데이터를 트랜잭션으로 만들어 반환</summary>
	public TransactionValue GetTransactionUpdateValue()
	{
		return TransactionValue.SetUpdateV2(GetTableName(), InData, Backend.UserInDate, GetParam());
	}
}
