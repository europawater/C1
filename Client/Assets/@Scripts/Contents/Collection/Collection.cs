using System.Collections.Generic;
using System.Linq;
using static Define;

public class Collection
{
	public CollectionData CollectionData { get; private set; }
	public CollectionInfo CollectionInfo { get; private set; }

	public Dictionary<EStat, float> CollectionValueDict { get; private set; } = new Dictionary<EStat, float>();
	public Dictionary<EStat, float> ValidCollectionValueDict
	{
		get { return CollectionValueDict.Where(kv => kv.Value != 0).ToDictionary(kv => kv.Key, kv => kv.Value); }
	}

	public Collection(CollectionInfo collectionInfo)
	{
		CollectionData = Managers.Backend.Chart.CollectionChart.CollectionDataDict[collectionInfo.TemplateID];
		CollectionInfo = collectionInfo;

		Initialize();
	}

	private void Initialize()
	{
		CollectionValueDict.Clear();
		CollectionValueDict.Add(EStat.Attack, CollectionData.Attack);
		CollectionValueDict.Add(EStat.Defense, CollectionData.Defense);
		CollectionValueDict.Add(EStat.MaxHP, CollectionData.MaxHP);
		CollectionValueDict.Add(EStat.CriticalValue, CollectionData.CriticalValue);
		CollectionValueDict.Add(EStat.CriticalRate, CollectionData.CriticalRate);
		CollectionValueDict.Add(EStat.SkillDamageValue, CollectionData.SkillDamageValue);
		CollectionValueDict.Add(EStat.SkillCriticalValue, CollectionData.SkillCriticalValue);
		CollectionValueDict.Add(EStat.DodgeRate, CollectionData.DodgeRate);
		CollectionValueDict.Add(EStat.ComboAttackRate, CollectionData.ComboAttackRate);
		CollectionValueDict.Add(EStat.CounterAttackRate, CollectionData.CounterAttackRate);
		CollectionValueDict.Add(EStat.BossExtraValue, CollectionData.BossExtraValue);
	}

	public void SetCollectionInfo(CollectionInfo collectionInfo)
	{
		CollectionInfo = collectionInfo;
	}
}
