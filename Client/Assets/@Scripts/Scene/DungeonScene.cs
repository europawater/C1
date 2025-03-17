using System.Collections;
using UnityEngine;
using static Define;

public class DungeonScene : GameScene
{
    private DungeonData _dungeonData;
    public DungeonData DungeonData => _dungeonData;

    protected override void Initialize()
    {
        switch (Managers.Game.SelectDungeonType)
        {
            case EDungeonType.Cube:
                _dungeonData = Managers.Backend.Chart.DungeonChart.CubeDungeonDataDict[Managers.Backend.GameData.Player.DungeonLevelDict[EDungeonType.Cube]];
                break;
            case EDungeonType.Gold:
                _dungeonData = Managers.Backend.Chart.DungeonChart.GoldDungeonDataDict[Managers.Backend.GameData.Player.DungeonLevelDict[EDungeonType.Gold]];
                break;
            case EDungeonType.Diamond:
                _dungeonData = Managers.Backend.Chart.DungeonChart.DiamondDungeonDataDict[Managers.Backend.GameData.Player.DungeonLevelDict[EDungeonType.Diamond]];
                break;

            default:
                break;
        }

        if (_dungeonData == null)
        {
            Debug.LogError("DungeonData is null");
            return;
        }

        // 맵 생성
        SpawnMap();
        // 영웅 생성
        SpawnHero();

        Managers.Game.SetBuddy();
        SpawnBuddy();

        // SceneUI 설정
        _gameSceneUI.SetInfo(this, true);

        // 스테이지 상태 설정
        StageState = EStageState.Start;
    }

    #region Stage State

    protected override IEnumerator CoOverState()
    {
        // Fade Out
        _gameSceneUI.PlayAnimation(UI_ANIMATION_FADE_OUT);

        yield return new WaitForSeconds(0.5f);

        Managers.Scene.LoadScene(EScene.Game);
    }

    protected override IEnumerator CoClearState()
    {
        // Fade Out
        _gameSceneUI.PlayAnimation(UI_ANIMATION_FADE_OUT);

        yield return new WaitForSeconds(0.5f);

        // 보상 처리
        Managers.Game.HandleReward(_dungeonData.RewardType, _dungeonData.RewardValue);
        Managers.Backend.GameData.Player.DungeonLevelUp(Managers.Game.SelectDungeonType);

        Managers.Scene.LoadScene(EScene.Game);

        yield return null;
    }

    #endregion

    protected override Map SpawnMap()
    {
        Map map = Managers.Object.SpawnMap(new Vector3(0.0f, 11.0f, 0.0f), _dungeonData.PrefabKey);

        return map;
    }

    protected override void SpawnMonsterByWaveIndex(int waveIndex)
    {
        switch (waveIndex)
        {
            case 1:
                SpawnMonsters(_dungeonData.FirstWaveMonsterList, _dungeonData.DifficultyLevel);
                break;
            case 2:
                SpawnMonsters(_dungeonData.SecondWaveMonsterList, _dungeonData.DifficultyLevel);
                break;
            case 3:
                SpawnMonsters(_dungeonData.ThirdWaveMonsterList, _dungeonData.DifficultyLevel);
                break;
            case 4:
                SpawnMonsters(_dungeonData.FourthWaveMonsterList, _dungeonData.DifficultyLevel);
                break;
            case 5:
                SpawnMonsters(_dungeonData.BossWaveMonsterList, _dungeonData.DifficultyLevel);
                break;

            default:
                break;
        }
    }
}
