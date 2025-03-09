using LitJson;
using System;
using static Define;

/// <summary>
/// 스킬 정보를 담는 클래스입니다.
/// 뒤끝에 저장되는 데이터입니다.
/// </summary>
[Serializable]
public class SkillInfo
{
    public int TemplateID { get; private set; }
    public EOwningState OwningState { get; private set; }
    public int Level { get; private set; }
    public bool IsEquipped { get; private set; }

	public SkillInfo(int templateID, EOwningState owningState, int level, bool isEquipped)
    {
        TemplateID = templateID;
        OwningState = owningState;
        Level = level;
		IsEquipped = isEquipped;
	}

	public SkillInfo(JsonData jsonData)
	{
		TemplateID = int.Parse(jsonData["TemplateID"].ToString());
		OwningState = (EOwningState)Enum.Parse(typeof(EOwningState), jsonData["OwningState"].ToString());
		Level = int.Parse(jsonData["Level"].ToString());
		IsEquipped = bool.Parse(jsonData["IsEquipped"].ToString());
	}

    public void SetOwningState(EOwningState owningState)
	{
		OwningState = owningState;
	}

	public void SetLevel(int level)
	{
		Level = level;
	}

	public void SetIsEquipped(bool isEquipped)
	{
		IsEquipped = isEquipped;
	}
}
