using BackEnd;
using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public enum PostChartType
{
	RewardChart,

	// TODO : 보상 구분
}

/// <summary>
/// 우편에서 보상에 대한 정보를 담고 있는 클래스입니다.
/// </summary>
public class PostChart
{
	public int Count { get; private set; }
	public PostChartType PostChartType { get; private set; }
	public int TemplateID { get; private set; }
	public string Name { get; private set; }

	public PostChart(JsonData jsonData)
	{
		Count = int.Parse(jsonData["itemCount"].ToString());

		string chartName = jsonData["chartName"].ToString();
		if (!Enum.TryParse(chartName, out PostChartType postChartType))
		{
			Debug.LogError($"PostChartType을 찾을 수 없습니다 : {chartName}");
			return;
		}
		PostChartType = postChartType;

		switch (PostChartType)
		{
			case PostChartType.RewardChart:
				TemplateID = int.Parse(jsonData["item"]["TemplateID"].ToString());
				Name = jsonData["item"]["Remark"].ToString();
				break;
		}
	}

	public void Receive()
	{
		// 타입에 따라 처리
		switch (PostChartType)
		{
			case PostChartType.RewardChart:
				switch (TemplateID)
				{
					case 1:
						Managers.Backend.GameData.Currency.AddAmount(ECurrency.Gold, Count);
						break;
					case 2:
						Managers.Backend.GameData.Currency.AddAmount(ECurrency.Diamond, Count);
						break;
					case 3:
						break;
				}
				break;
		}
	}
}

/// <summary>
/// 우편에 대한 정보를 담고 있는 클래스입니다.
/// </summary>
public class Post
{
	public PostType PostType { get; private set; }
	public string InDate { get; private set; }
	public string Title { get; private set; }
	public string Content { get; private set; }
	public DateTime ExpirationDate { get; private set; }

	public List<PostChart> PostChartList { get; private set; } = new List<PostChart>();

	public Post(PostType postType, JsonData jsonData)
	{
		PostType = postType;
		InDate = jsonData["inDate"].ToString();
		Title = jsonData["title"].ToString();
		Content = jsonData["content"].ToString();
		ExpirationDate = DateTime.Parse(jsonData["expirationDate"].ToString());

		if (jsonData["items"].Count > 0)
		{
			for (int index = 0; index < jsonData["items"].Count; index++)
			{
				PostChart postChart = new PostChart(jsonData["items"][index]);
				PostChartList.Add(postChart);
			}
		}
	}

	public void Receive(Action<bool, string> onComplete)
	{
		bool isSuccess = false;
		string errorInfo = string.Empty;

		SendQueue.Enqueue(Backend.UPost.ReceivePostItem, PostType, InDate, bro =>
		{
			try
			{
				Debug.Log($"Backend.UPost.ReceivePostItem({PostType}, {InDate}) : {bro}");

				if (bro.IsSuccess())
				{
					if (PostChartList.Count > 0)
					{
						foreach (PostChart postChart in PostChartList)
						{
							postChart.Receive();
						}
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
				onComplete?.Invoke(isSuccess, errorInfo);
			}
		});
	}
}
