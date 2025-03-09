using UnityEngine;
using static Define;

public class NormalMonster : BaseMonsterObject
{
    protected override void Init()
    {
        MonsterType = EMonsterType.NormalMonster;

        base.Init();
    }
}
