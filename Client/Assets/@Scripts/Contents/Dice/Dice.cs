using Spine;
using Spine.Unity;
using UnityEngine;
using static Define;

/// <summary>
/// ���ӿ��� *���̽��� ���� ��ü(Object)�� �����ϴ� Ŭ�����Դϴ�.
/// *���̽��� ��� �̴� ����� �����մϴ�.
/// </summary>
public class Dice : MonoBehaviour
{
	public SkeletonGraphic SkeletonGraphic { get; private set; }

	private EDiceState _diceState = EDiceState.None;
	public EDiceState DiceState
	{
		get { return _diceState; }
		set
		{
			_diceState = value;
			OnChangedState();
		}
	}

	private void Awake()
	{
		SkeletonGraphic = GetComponent<SkeletonGraphic>();
	}

	private void Start()
	{
		// Bind Event
		SkeletonGraphic.AnimationState.Event -= OnAnimEventHandler;
		SkeletonGraphic.AnimationState.Event += OnAnimEventHandler;
		SkeletonGraphic.AnimationState.Complete -= OnAnimCompleteHandler;
		SkeletonGraphic.AnimationState.Complete += OnAnimCompleteHandler;

		DiceState = EDiceState.Idle;
	}

	private void OnChangedState()
	{
		UpdateAnimation();
	}

	#region Animation

	private const string ANIMATION_IDLE = "idle";
	private const string ANIMATION_DRAW = "active";

	protected virtual void UpdateAnimation()
	{
		switch (DiceState)
		{
			case EDiceState.Idle:
				PlayAnimation(0, ANIMATION_IDLE, true);
				break;
			case EDiceState.Draw:
				PlayAnimation(0, ANIMATION_DRAW, false);
				break;

			default:
				break;
		}
	}

	public void PlayAnimation(int trackIndex, string animName, bool loop)
	{
		if (SkeletonGraphic == null)
		{
			return;
		}

		SkeletonGraphic.AnimationState.SetAnimation(trackIndex, animName, loop);
	}

	public virtual void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e)
	{
		Managers.Game.HandleDrawEquipment();
	}

	private void OnAnimCompleteHandler(TrackEntry trackEntry)
	{
		if(DiceState == EDiceState.Draw)
		{
			DiceState = EDiceState.Idle;
		}
	}

	#endregion
}
