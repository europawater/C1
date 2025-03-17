using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
	#region Object Transform

	[SerializeField]
	private Transform _heroTransform;
	[SerializeField]
	private Transform[] _singleMonsterTransformArray;
	[SerializeField]
	private Transform[] _doubleMonsterTransformArray;
	[SerializeField]
	private Transform[] _tripleMonsterTransformArray;
	[SerializeField]
	private Transform[] _buddyTransformArray;

	#endregion

	private Animator _gameSceneAnimator;
	protected UI_GameScene _gameSceneUI;

	// 데이터
	private StageData _stageData;
	public StageData StageData => _stageData;
	
    // 정보
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

	private void OnEnable()
	{
		Managers.Event.AddEvent(EEventType.OnBuddyChanged, SpawnBuddy);	
	}

	private void OnDisable()
	{
		Managers.Event.RemoveEvent(EEventType.OnBuddyChanged, SpawnBuddy);
	}

	protected virtual void Initialize()
	{
		_stageData = Managers.Backend.Chart.StageChart.DataList[Managers.Backend.GameData.Player.StageLevel];

		if (_stageData == null)
		{
			Debug.LogError("StageData is null");
			return;
		}

		// 맵 생성
		SpawnMap();
		// 영웅 생성
		SpawnHero();

		Managers.Game.SetBuddy();
		SpawnBuddy();

		// SceneUI 설정
		_gameSceneUI.SetInfo(this);

		// 스테이지 상태 설정
		StageState = EStageState.Start;
	}

	#region Stage State

	private const string ANIMATION_INTRO = "anim_game_scene_direction_intro";
	private const string ANIMATION_MOVE = "anim_game_scene_driection_move";
	private const string UI_ANIMATION_FADE_IN = "anim_game_scene_ui_fade_in";
	protected const string UI_ANIMATION_FADE_OUT = "anim_game_scene_ui_fade_out";

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
		// 스테이지 웨이브 인덱스는 1부터 시작
		StageWaveIndex = 1;

		// 연출
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

	protected virtual IEnumerator CoOverState()
	{
		// Fade Out
		_gameSceneUI.PlayAnimation(UI_ANIMATION_FADE_OUT);

		yield return new WaitForSeconds(0.5f);

		Managers.Object.Hero.ReboneObject();

		StageState = EStageState.Start;
		yield return null;
	}

    protected virtual IEnumerator CoClearState()
	{
		// Fade Out
		_gameSceneUI.PlayAnimation(UI_ANIMATION_FADE_OUT);

		yield return new WaitForSeconds(0.5f);

		Managers.Object.RemoveMap();
		Managers.Object.RemoveHero();

		Managers.Backend.GameData.Player.AddStageLevel(1);

		Initialize();

		yield return null;
	}

	#endregion

	protected virtual Map SpawnMap()
	{
		Map map = Managers.Object.SpawnMap(new Vector3(0.0f, 11.0f, 0.0f), _stageData.PrefabKey);

		return map;
	}

    protected Hero SpawnHero()
	{
		Hero hero = Managers.Object.SpawnAIObject<Hero>(_heroTransform, 100001);
		hero.LookLeft = false;

		return hero;
	}

    protected void SpawnBuddy()
	{
		int slotIndex = 0;
		Managers.Object.RemoveAllBuddy();
		foreach (var buddyInfo in Managers.Game.EquippedBuddyInfoSlotDict.Values)
		{
			Buddy buddy = Managers.Object.SpawnBuddy(_buddyTransformArray[slotIndex], buddyInfo.TemplateID);
			buddy.LookLeft = false;
			slotIndex++;
		}
	}

	protected virtual void SpawnMonsterByWaveIndex(int waveIndex)
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

	protected void SpawnMonsters(List<int> monsterList, int difficultyLevel)
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

		// 애니메이션 연출이 끝나면 전투 상태로 변경
		StageState = EStageState.Battle;
	}

	#endregion
}
