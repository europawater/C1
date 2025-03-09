using BackEnd;
using LitJson;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class RankManager
{
	private List<LeaderBoard> _leaderBoardList = new List<LeaderBoard>();
	public List<LeaderBoard> LeaderBoardList => _leaderBoardList;

	public void LoadRankList(Action<bool, string, string, string> onComplete)
	{ 
		bool isSuccess = false;
		string className = GetType().Name;
		string funcName = MethodBase.GetCurrentMethod()?.Name;
		string errorInfo = string.Empty;

		SendQueue.Enqueue(Backend.URank.User.GetRankTableList, bro =>
		{
			try
			{
				Debug.Log($"Backend.URank.User.GetRankTableList : {bro}");

				if (bro.IsSuccess())
				{
					JsonData leaderBoardListJson = bro.FlattenRows();
				
					for(int i=0; i < leaderBoardListJson.Count; i++)
					{
						LeaderBoard leaderBoard = new LeaderBoard(leaderBoardListJson[i]);
						_leaderBoardList.Add(leaderBoard);
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
				onComplete(isSuccess, className, funcName, errorInfo);
			}
		});
	}
}
