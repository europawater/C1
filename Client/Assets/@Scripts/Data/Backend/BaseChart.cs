using BackEnd;
using LitJson;
using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 뒤끝에 등록된 차트(데이터 시트) 입니다.
/// </summary>
/// <typeparam name="T">데이터 타입</typeparam>
public abstract class BaseChart<T>
{
	/// <summary>뒤끝에 등록된 차트 이름을 참조합니다.</summary>
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

		// 차트 데이터 요청 함수
		SendQueue.Enqueue(Backend.Chart.GetChartContents, chartId, bro =>
		{
			try
			{
				Debug.Log($"Backend.Chart.GetChartContents({chartId}) : {bro}");

				if (bro.IsSuccess())
				{
					JsonData jsonData = bro.FlattenRows();

					// 차트 데이터 초기화
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
