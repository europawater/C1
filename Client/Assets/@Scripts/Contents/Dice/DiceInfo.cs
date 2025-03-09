using System;

/// <summary>
/// ���̽� ������ ��� Ŭ�����Դϴ�.
/// �ڳ��� ����Ǵ� �������Դϴ�.
/// </summary>
[Serializable]
public class DiceInfo
{
	public int TemplateID { get; private set; }
	public int DiceLevel { get; private set; }
	public int DiceCount { get; private set; }

	public DiceInfo(int templateID, int diceLevel, int diceCount)
	{
		TemplateID = templateID;
		DiceLevel = diceLevel;
		DiceCount = diceCount;
	}

	public void SetDiceLevel(int diceLevel)
	{
		DiceLevel = diceLevel;
	}

	public void SetDiceCount(int diceCount)
	{
		DiceCount = diceCount;
	}

	public void AddDiceCount(int count)
	{
		DiceCount += count;
	}

	public void SubDiceCount(int count)
	{
		DiceCount -= count;
	}
}
