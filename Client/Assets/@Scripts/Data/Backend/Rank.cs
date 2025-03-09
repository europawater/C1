using BackEnd;
using LitJson;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class RankUser
{ 
	public string GamerInData { get; private set; }
	public string NickName { get; private set; }
	public string Score { get; private set; }
	public string Index { get; private set; }
	public string Rank { get; private set; }
	public string TableName { get; private set; }
	public string ExtraData { get; private set; }

	public RankUser(JsonData jsonData, string tableName, string extraData)
	{
		GamerInData = jsonData["gamerInDate"].ToString();
		NickName = jsonData["nickname"].ToString();
		Score = jsonData["score"].ToString();
		Index = jsonData["index"].ToString();
		Rank = jsonData["rank"].ToString();
		TableName = tableName;
		
		if (!string.IsNullOrEmpty(extraData) && jsonData.ContainsKey(extraData))
		{ 
			ExtraData = jsonData[extraData].ToString();
		}
	}
}

public enum RankType
{
	user,
}

public class LeaderBoard
{
	public RankType RankType { get; private set; }
	public string UUID { get; private set; }
	public string Order { get; private set; }
	public bool IsReset { get; private set; }
	public string Title { get; private set; }
	public string Table { get; private set; }
	public string Column { get; private set; }
	public string ExtraDataColumn { get; private set; }
	public string ExtraDataType { get; private set; }
	public int TotalUserCount { get; private set; }
	public DateTime UpdateTime { get; private set; }
	public DateTime MyRankUpdateTime { get; private set; }
	public RankUser MyRank { get; private set; }

	public List<RankUser> _userRankList = new List<RankUser>();

	public LeaderBoard(JsonData jsonData)
	{ 
		if(!Enum.TryParse(jsonData["rankType"].ToString(), out RankType rankType))
		{
			Debug.LogError($"RankType을 찾을 수 없습니다 : {jsonData["rankType"].ToString()}");
			return;
		}

		RankType = rankType;
		UUID = jsonData["uuid"].ToString();
		Order = jsonData["order"].ToString();
		IsReset = jsonData["isReset"].ToString() == "true" ? true : false;
		Title = jsonData["title"].ToString();
		Table = jsonData["table"].ToString();
		Column = jsonData["column"].ToString();

		if (jsonData.ContainsKey("extraDataColum"))
		{ 
			ExtraDataColumn = jsonData["extraDataColumn"].ToString();
			ExtraDataType = jsonData["extraDataType"].ToString();
		}

		UpdateTime = DateTime.MinValue;
		MyRankUpdateTime = DateTime.MinValue;
	}

	public void GetRankList(Action<List<RankUser>> onComplete)
	{ 
		// 1~10위까지만 가져옴
		int limit = 10;
		SendQueue.Enqueue(Backend.URank.User.GetRankList, UUID, limit, bro =>
		{
			try
			{
				Debug.Log($"Backend.URank.User.GetRankList : {bro}");

				if (bro.IsSuccess())
				{
					JsonData rankListJson = bro.GetFlattenJSON();

					_userRankList.Clear();
					foreach (JsonData josnData in rankListJson["rows"])
					{
						RankUser rankUser = new RankUser(josnData, Table, ExtraDataColumn);
						_userRankList.Add(rankUser);
					}

					TotalUserCount = int.Parse(rankListJson["totalCount"].ToString());
					UpdateTime = DateTime.UtcNow;
				}
				else
				{ 
				}
			}
			catch (Exception e)
			{
			}
			finally
			{ 
				onComplete?.Invoke(_userRankList);
			}
		});
	}

	public void GetMyRank(Action<RankUser> onComplete)
	{
		SendQueue.Enqueue(Backend.URank.User.GetMyRank, UUID, bro =>
		{
			try
			{
				Debug.Log($"Backend.URank.User.GetMyRank : {bro}");

				if (bro.IsSuccess())
				{
					JsonData myRankJson = bro.Rows();
					MyRank = new RankUser(bro.FlattenRows()[0], Table, ExtraDataColumn);
					MyRankUpdateTime = DateTime.UtcNow;
				}
				else
				{
				}
			}
			catch (Exception e)
			{
			}
			finally
			{
				onComplete?.Invoke(MyRank);
			}
		});
	}
}
