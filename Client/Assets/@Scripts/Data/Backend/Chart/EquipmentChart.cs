using LitJson;
using System.Collections.Generic;

public class EquipmentChart : BaseChart<EquipmentData>
{
	public Dictionary<int, EquipmentData> DataDict { get; private set; } = new Dictionary<int, EquipmentData>();

	public override string GetChartFileName()
	{
		return "EquipmentChart";
	}

	protected override void InitializeData(JsonData jsonData)
	{
		foreach (JsonData data in jsonData)
		{
			EquipmentData equipmentData = new EquipmentData(data);
			DataDict.Add(equipmentData.TemplateID, equipmentData);
		}
	}
}
