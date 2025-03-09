using Spine;
using Spine.Unity;
using System.Collections;
using UnityEngine;
using static Define;

public class SkillCube : BaseObject
{
	private SkeletonAnimation _skeletonAnimation;
	private float _lifeTime;

	protected override void Init()
	{
		GameObjectType = EGameObjectType.SkillCube;

		_skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
		// Bind Event
		_skeletonAnimation.AnimationState.Event -= OnAnimEventHandler;
		_skeletonAnimation.AnimationState.Event += OnAnimEventHandler;
		_skeletonAnimation.AnimationState.Complete -= OnAnimCompleteHandler;
		_skeletonAnimation.AnimationState.Complete += OnAnimCompleteHandler;
	}

	#region Animation Event

	public virtual void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e)
	{
	}

	public virtual void OnAnimCompleteHandler(TrackEntry trackEntry)
	{
		Managers.Object.RemoveSkillCube(this);
	}

	#endregion
}
