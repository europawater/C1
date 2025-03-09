using BackEnd;
using LitJson;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static Define;

public class PostManager
{
	private Dictionary<string, Post> _postDict = new Dictionary<string, Post>();
	public Dictionary<string, Post> PostDict => _postDict;

	public void LoadPostList(PostType postType, Action<bool, string, string, string> onComplete)
	{
		bool isSuccess = false;
		string className = GetType().Name;
		string funcName = MethodBase.GetCurrentMethod()?.Name;
		string errorInfo = string.Empty;

		SendQueue.Enqueue(Backend.UPost.GetPostList, postType, bro =>
		{
			try
			{
				Debug.Log($"Backend.UPost.GetPostList({postType}) : {bro}");

				if (bro.IsSuccess())
				{
                    JsonData jsonData = bro.GetReturnValuetoJSON()["postList"];

					for(int i=0; i< jsonData.Count; i++)
					{
						// 새로운 우편인	경우에만 추가
						if(!_postDict.ContainsKey(jsonData[i]["inDate"].ToString()))
						{
							Post post = new Post(postType, jsonData[i]);
							_postDict.Add(post.InDate, post);
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
				onComplete?.Invoke(isSuccess, className, funcName, errorInfo);
			}
		});
	}

	public void ReceivePost(string postKey)
	{ 
		if(_postDict.ContainsKey(postKey))
		{
			_postDict[postKey].Receive((isSuccess, errorMessage) =>
			{
				if (isSuccess)
				{
					RemovePost(postKey);
				}
			});
		}
	}

	public void RemovePost(string inDate)
	{
		if (_postDict.ContainsKey(inDate))
		{
			_postDict.Remove(inDate);

			Managers.Event.TriggerEvent(EEventType.OnPostListChanged);
		}
	}
}
