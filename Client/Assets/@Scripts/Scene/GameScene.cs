using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
	#region Object Transform

	[SerializeField]
	private Transform[] _heroTransformArray;
	[SerializeField]
	private Transform[] _singleMonsterTransformArray;
	[SerializeField]
	private Transform[] _doubleMonsterTransformArray;
	[SerializeField]
	private Transform[] _tripleMonsterTransformArray;

	#endregion

	private Animator _gameSceneAnimator;
	private UI_GameScene _gameSceneUI;

	// ������
	private StageData _stageData;
	public StageData StageData => _stageData;

	// ����
	private int _stageWaveIndex = 0;
	public int StageWaveIndex
	{
		get { return _stageWaveIndex; }
		set
		{
			if (_stageWaveIndex == value)
			{
				return;
			}

			_stageWaveIndex = value;

			Managers.Object.RemoveAllMonsters();
			SpawnMonsterByWaveIndex(_stageWaveIndex);

			Managers.Event.TriggerEvent(EEventType.OnStageWaveIndexChanged);
		}
	}

	protected override void Awake()
	{
		base.Awake();

		SceneType = EScene.Game;
		Managers.Scene.SetCurrentScene(this);

		// UI
		_gameSceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();

		_gameSceneAnimator = GetComponent<Animator>();

		Initialize();
	}

	private void Initialize()
	{
		_stageData = Managers.Backend.Chart.StageChart.DataList[1];

		if (_stageData == null)
		{
			Debug.LogError("StageData is null");
			return;
		}

		// �� ����
		SpawnMap();
		// ���� ����
		SpawnHero();
		// SceneUI ����
		_gameSceneUI.SetInfo(this);

		// �������� ���� ����
		StageState = EStageState.Start;
	}

	#region Stage State

	private const string ANIMATION_INTRO = "anim_game_scene_direction_intro";
	private const string ANIMATION_MOVE = "anim_game_scene_driection_move";
	private const string UI_ANIMATION_FADE_IN = "anim_game_scene_ui_fade_in";
	private const string UI_ANIMATION_FADE_OUT = "anim_game_scene_ui_fade_out";

	private IEnumerator _currentCoroutine = null;
	private EStageState _stageState = EStageState.None;
	public EStageState StageState
	{
		get { return _stageState; }
		set
		{
			if (_stageState == value)
			{
				return;
			}

			_stageState = value;

			SwitchStageCoroutine();
		}
	}

	private void SwitchStageCoroutine()
	{
		IEnumerator coroutine = GetStageCoroutineForState(StageState);
		if (coroutine == null || _currentCoroutine == coroutine)
		{
			return;
		}

		if (_currentCoroutine != null)
		{
			StopCoroutine(_currentCoroutine);
		}

		_currentCoroutine = coroutine;
		StartCoroutine(_currentCoroutine);
	}

	private IEnumerator GetStageCoroutineForState(EStageState state)
	{
		switch (state)
		{
			case EStageState.Start:
				return CoStartState();
			case EStageState.Battle:
				return CoBattleState();
			case EStageState.Move:
				return CoMoveState();
			case EStageState.Over:
				return CoOverState();
			case EStageState.Clear:
				return CoClearState();

			default:
				return null;
		}
	}

	private IEnumerator CoStartState()
	{
		// �������� ���̺� �ε����� 1���� ����
		StageWaveIndex = 1;

		// ����
		_gameSceneAnimator.Rebind();
		_gameSceneAnimator.CrossFade(ANIMATION_INTRO, 0.0f);
		// Fade In
		_gameSceneUI.PlayAnimation(UI_ANIMATION_FADE_IN);

		yield return null;
	}

	private IEnumerator CoBattleState()
	{
		Managers.Object.SetAllAIObjectState(EAIObjectState.Idle);

		StartCoroutine(Managers.Turn.CoTurnBattleHandler(() => { StageState = EStageState.Over; }, () => { StageState = EStageState.Move; }));

		yield return null;
	}

	private IEnumerator CoMoveState()
	{
		Managers.Object.Hero.AIObjectState = EAIObjectState.Move;

		StageWaveIndex++;
		if (StageWaveIndex > 5)
		{
			StageState = EStageState.Clear;
			yield return null;
		}

		_gameSceneAnimator.Rebind();
		_gameSceneAnimator.CrossFade(ANIMATION_MOVE, 0.0f);

		yield return null;
	}

	private IEnumerator CoOverState()
	{
		// Fade Out
		_gameSceneUI.PlayAnimation(UI_ANIMATION_FADE_OUT);

		yield return new WaitForSeconds(0.5f);

		Managers.Object.Hero.ReboneObject();

		StageState = EStageState.Start;
		yield return null;
	}

	private IEnumerator CoClearState()
	{
		// Fade Out
		_gameSceneUI.PlayAnimation(UI_ANIMATION_FADE_OUT);

		yield return new WaitForSeconds(0.5f);

		Managers.Object.RemoveMap();
		Managers.Object.RemoveHero();

		//Managers.Backend.UserInfoData.StageLevel++;

		Initialize();

		yield return null;
	}

	#endregion

	private Map SpawnMap()
	{
		Map map = Managers.Object.SpawnMap(new Vector3(0.0f, 11.0f, 0.0f), _stageData.PrefabKey);

		return map;
	}

	private Hero SpawnHero()
	{
		Hero hero = Managers.Object.SpawnAIObject<Hero>(_heroTransformArray[0], 100001);
		hero.LookLeft = false;

		return hero;
	}

	private void SpawnMonsterByWaveIndex(int waveIndex)
	{
		switch (waveIndex)
		{
			case 1:
				SpawnMonsters(_stageData.FirstWaveMonsterList, _stageData.DifficultyLevel);
				break;
			case 2:
				SpawnMonsters(_stageData.SecondWaveMonsterList, _stageData.DifficultyLevel);
				break;
			case 3:
				SpawnMonsters(_stageData.ThirdWaveMonsterList, _stageData.DifficultyLevel);
				break;
			case 4:
				SpawnMonsters(_stageData.FourthWaveMonsterList, _stageData.DifficultyLevel);
				break;
			case 5:
				SpawnMonsters(_stageData.BossWaveMonsterList, _stageData.DifficultyLevel);
				break;

			default:
				break;
		}
	}

	private void SpawnMonsters(List<int> monsterList, int difficultyLevel)
	{
		int spawnIndex = 0;
		switch (monsterList.Count)
		{
			case 1:
				foreach (int monsterIndex in monsterList)
				{
					Managers.Object.SpawnAIObject<NormalMonster>(_singleMonsterTransformArray[spawnIndex], monsterIndex, difficultyLevel);
					spawnIndex++;
				}
				break;
			case 2:
				foreach (int monsterIndex in monsterList)
				{
					Managers.Object.SpawnAIObject<NormalMonster>(_doubleMonsterTransformArray[spawnIndex], monsterIndex, difficultyLevel);
					spawnIndex++;
				}
				break;
			case 3:
				foreach (int monsterIndex in monsterList)
				{
					Managers.Object.SpawnAIObject<NormalMonster>(_tripleMonsterTransformArray[spawnIndex], monsterIndex, difficultyLevel);
					spawnIndex++;
				}
				break;

			default:
				break;
		}
	}

	public override void Clear()
	{
		_currentCoroutine = null;
		StopAllCoroutines();
	}

	#region Animation Event

	public void OnGameSceneAnimMapMoveHandler()
	{
		Managers.Object.Map.MapState = EMapState.Move;
	}

	public void OnAllObjectStateIdleHandler()
	{
		Managers.Object.SetAllAIObjectState(EAIObjectState.Idle);
	}

	public void OnGameSceneAnimCompleteHandler()
	{
		Managers.Object.Map.MapState = EMapState.Stop;

		// �ִϸ��̼� ������ ������ ���� ���·� ����
		StageState = EStageState.Battle;
	}

	#endregion
}
