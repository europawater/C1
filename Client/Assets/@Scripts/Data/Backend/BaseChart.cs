using BackEnd;
using LitJson;
using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// �ڳ��� ��ϵ� ��Ʈ(������ ��Ʈ) �Դϴ�.
/// </summary>
/// <typeparam name="T">������ Ÿ��</typeparam>
public abstract class BaseChart<T>
{
	/// <summary>�ڳ��� ��ϵ� ��Ʈ �̸��� �����մϴ�.</summary>
	public abstract string GetChartFileName();
	protected abstract void InitializeData(JsonData jsonData);

	public void LoadChartData(Action<bool, string, string, string> onComplete)
	{
		bool isSuccess = false;
		string className = GetType().Name;
		string funcName = MethodBase.GetCurrentMethod()?.Name;
		string errorInfo = string.Empty;

		string chartFileName = GetChartFileName();
		string chartId = Managers.Backend.Chart.AllChartIDDict[chartFileName];

		// ��Ʈ ������ ��û �Լ�
		SendQueue.Enqueue(Backend.Chart.GetChartContents, chartId, bro =>
		{
			try
			{
				Debug.Log($"Backend.Chart.GetChartContents({chartId}) : {bro}");

				if (bro.IsSuccess())
				{
					JsonData jsonData = bro.FlattenRows();

					// ��Ʈ ������ �ʱ�ȭ
					InitializeData(jsonData);

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
}
