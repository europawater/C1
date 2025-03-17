using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

/// <summary>
/// 미션 정보를 담는 클래스입니다.
/// 뒤끝에 저장되는 데이터입니다.
/// </summary>
[Serializable]
public class MissionInfo
{
	public int TemplateID { get; private set; }
	public EOwningState OwningState { get; private set; }
	public int StackedPoint { get; private set; }
    public List<EOwningState> PointStepOwningStateList { get; private set; } = new List<EOwningState>();

    public int Point { get; private set; }

    public MissionInfo(int templateID, EOwningState owningState, int stackedPoint, List<EOwningState> pointStepOwningStateList)
	{
		TemplateID = templateID;
		OwningState = owningState;
		StackedPoint = stackedPoint;
		PointStepOwningStateList = pointStepOwningStateList;

        Point = Managers.Backend.Chart.MissionChart.MissionDataDict[TemplateID].Point;
    }

	public MissionInfo(JsonData jsonData)
	{
		TemplateID = int.Parse(jsonData["TemplateID"].ToString());
		OwningState = (EOwningState)Enum.Parse(typeof(EOwningState), jsonData["OwningState"].ToString());
		StackedPoint = int.Parse(jsonData["StackedPoint"].ToString());

		PointStepOwningStateList = new List<EOwningState>();
		foreach (JsonData pointStepOwningStateJsonData in jsonData["PointStepOwningStateList"])
		{
			PointStepOwningStateList.Add((EOwningState)Enum.Parse(typeof(EOwningState), pointStepOwningStateJsonData.ToString()));
		}

        Point = Managers.Backend.Chart.MissionChart.MissionDataDict[TemplateID].Point;
    }

    public void AddStackedPoint(int point)
    {
        StackedPoint += point;
    }

	public void ResetStackPoint()
	{
		StackedPoint = 0;
	}

	public void SetOwningState(EOwningState owningState)
    {
        OwningState = owningState;
    }

	public void ResetPointStepOwningStateList()
	{ 
		for(int index = 0; index < PointStepOwningStateList.Count; index++)
		{
			PointStepOwningStateList[index] = EOwningState.Unowned;
		}
	}
}
