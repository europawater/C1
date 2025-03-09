using LitJson;
using System.Collections.Generic;

public class EquipmentLevelScalingChart : BaseChart<EquipmentLevelScalingData>
{
	public Dictionary<int, EquipmentLevelScalingData> DataDict { get; private set; } = new Dictionary<int, EquipmentLevelScalingData>();

	public override string GetChartFileName()
	{
		return "EquipmentLevelScalingChart";
	}

	protected override void InitializeData(JsonData jsonData)
	{
		foreach (JsonData data in jsonData)
		{
			EquipmentLevelScalingData equipmentLevelScalingData = new EquipmentLevelScalingData(data);
			DataDict.Add(equipmentLevelScalingData.TemplateID, equipmentLevelScalingData);
		}
	}
}
