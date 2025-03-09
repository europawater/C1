using LitJson;
using System;

/// <summary>
/// ��� ������ ��� Ŭ�����Դϴ�.
/// �ڳ��� ����Ǵ� �������Դϴ�.
/// </summary>
[Serializable]
public class EquipmentInfo
{
	public int TemplateID { get; private set; }
    public int EnchantLevel { get; private set; }
	public int EnchantSafe { get; private set; }

	public EquipmentInfo(int templateID, int enchantLevel, int enchantSafe)
	{
		TemplateID = templateID;
		EnchantLevel = enchantLevel;
		EnchantSafe = enchantSafe;
	}

	public EquipmentInfo(JsonData jsonData)
	{
		TemplateID = (int)jsonData["TemplateID"];
		EnchantLevel = (int)jsonData["EnchantLevel"];
		EnchantSafe = (int)jsonData["EnchantSafe"];
	}

	public void SetEnchantLevel(int enchantLevel)
	{
		EnchantLevel = enchantLevel;
	}

	public void SetEnchantSafe(int enchantSafe)
	{
		EnchantSafe = enchantSafe;
	}

	public void AddEnchantSafe(int count)
	{
		EnchantSafe += count;
	}

	public void SubEnchantSafe(int count)
	{
		EnchantSafe -= count;
	}
}
