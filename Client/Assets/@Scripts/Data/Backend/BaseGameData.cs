using BackEnd;
using LitJson;
using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// �ڳ����� �����ϴ� ���� ������ �Դϴ�.
/// </summary>
public abstract class BaseGameData
{
	/// <summary>���� �������� ���� �ĺ����Դϴ�.</summary>
	public string InData { get; private set; }
	/// <summary>�����Ͱ� ����Ǿ����� ���θ� ��Ÿ���ϴ�.</summary>
	public bool IsChangedBackendGameData { get; private set; }
	/// <summary>�ڳ��� ��ϵ� ���� ������ ���̺� �̸��� �����մϴ�.</summary>
	public abstract string GetTableName();
	/// <summary>�ڳ��� ��ϵ� ���� ������ ���̺� ������ �����͸� ��ȯ�մϴ�.</summary>
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

		// ���� ���� �ε� �Լ�
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

						// �������� �ε�� �����͸� ���� �����ͷ� ����
						SetServerDataToLocal(bro.FlattenRows()[0]);

						isSuccess = true;
						onComplete?.Invoke(isSuccess, className, funcName, errorInfo);
					}
					else
					{
						// �ε��� ���� ���ٸ�	 �ʱ�ȭ �� ����
						// *SendQueue�� �񵿱������� �����ϱ� ������, ���� �Լ��� ȣ���� ��, SendQueue�� ���� �Լ��� �־���� ȣ���ؾ���.
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

	/// <summary>���� �ε��� ���� ���ٸ� ���ÿ��� �ʱ�ȭ �� �ڳ��� �����ϴ� �Լ��Դϴ�.</summary>
	private void InsertBackendGameData(Action<bool, string, string, string> onComplete)
	{
		bool isSuccess = false;
		string className = GetType().Name;
		string funcName = MethodBase.GetCurrentMethod()?.Name;
		string errorInfo = string.Empty;

		string tableName = GetTableName();

		// ������ �ʱ�ȭ
		InitializeData();

		// ���� ���� ���� �Լ�
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
		// ���� ���� ������Ʈ �Լ�(V2)
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

	/// <summary>���̺� ������Ʈ�� �����͸� Ʈ��������� ����� ��ȯ</summary>
	public TransactionValue GetTransactionUpdateValue()
	{
		return TransactionValue.SetUpdateV2(GetTableName(), InData, Backend.UserInDate, GetParam());
	}
}
