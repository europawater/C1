using UnityEngine;
using static Define;

public class BossMonster : BaseMonsterObject
{
    protected override void Init()
    {
        MonsterType = EMonsterType.BossMonster;

        base.Init();
    }
}
