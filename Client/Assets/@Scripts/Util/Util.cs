using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Define;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Util
{
	public static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
	{
		T component = gameObject.GetComponent<T>();
		if (component == null)
		{
			component = gameObject.AddComponent<T>();
		}

		return component;
	}

	public static GameObject FindChild(GameObject gameObject, string name = null, bool recursive = false)
	{
		Transform transform = FindChild<Transform>(gameObject, name, recursive);
		if (transform == null)
		{
			return null;
		}

		return transform.gameObject;
	}

	public static T FindChild<T>(GameObject gameObject, string name = null, bool recursive = false) where T : Object
	{
		if (gameObject == null)
		{
			return null;
		}

		if (recursive == false)
		{
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				Transform transform = gameObject.transform.GetChild(i);
				if (string.IsNullOrEmpty(name) || transform.name == name)
				{
					T component = transform.GetComponent<T>();
					if (component != null)
					{
						return component;
					}
				}
			}
		}
		else
		{
			foreach (T component in gameObject.GetComponentsInChildren<T>())
			{
				if (string.IsNullOrEmpty(name) || component.name == name)
				{
					return component;
				}
			}
		}

		return null;
	}

	public static T ParseEnum<T>(string value)
	{
		return (T)Enum.Parse(typeof(T), value, true);
	}

	public static int GetRandomInt(int min, int max)
	{
		return Random.Range(min, max + 1);
	}

	public static float GetRadomfloat(float min, float max)
	{
		return Random.Range(min, max + Mathf.Epsilon);
	}

	public static string GetStatusString(EStat stat)
	{
		switch (stat)
		{
			case EStat.Attack:
				return "���ݷ�";
			case EStat.Defense:
				return "����";
			case EStat.MaxHP:
				return "ü��";
			case EStat.CriticalValue:
				return "ũ��Ƽ�� ����(%)";
			case EStat.CriticalRate:
				return "ũ��Ƽ�� Ȯ��(%)";
			case EStat.SkillDamageValue:
				return "��ų ����(%)";
			case EStat.SkillCriticalValue:
				return "��ų ũ��Ƽ�� ����(%)";
			case EStat.DodgeRate:
				return "ȸ�� Ȯ��(%)";
			case EStat.ComboAttackRate:
				return "�޺� Ȯ��(%)";
			case EStat.CounterAttackRate:
				return "�ݰ� Ȯ��(%)";
			case EStat.BossExtraValue:
				return "���� �ߴ�(%)";

			default:
				return string.Empty;
		}
	}

	public static string GetBuddyGradeString(EBuddyGrade grade)
	{
		switch (grade)
		{
			case EBuddyGrade.Common:
				return "�Ϲ�";
			case EBuddyGrade.Rare:
				return "����";
			case EBuddyGrade.Unique:
				return "����ũ";
			case EBuddyGrade.Epic:
				return "����";
			case EBuddyGrade.Legend:
				return "����";

			default:
				return string.Empty;
		}
	}

	public static string GetBuddyBonusString(Dictionary<EStat, float> bonusDict)
	{ 
		string bonusString = string.Empty;
		foreach (KeyValuePair<EStat, float> bonus in bonusDict)
		{
			if (bonus.Value == 0)
			{
				continue;
			}
			bonusString += $"{GetStatusString(bonus.Key)} +{bonus.Value}\n";
		}

		return bonusString;
	}

	public static string GetHeroSkeletonDataKey()
	{
		if (Managers.Backend.GameData.Player.IsMale)
		{
			return "spi_hero_m_knight_SkeletonData";
		}
		else
		{
			return "spi_hero_f_knight_SkeletonData";
		}
	}
}
