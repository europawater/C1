using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static Define;

public abstract class BaseAIObject : BaseObject
{
	public int TemplateID { get; protected set; }
	public SkeletonAnimation SkeletonAnimation { get; private set; }
	public BaseStatus Status { get; private set; }

	public virtual Skill DefaultSkill { get; protected set; }
	public virtual List<Skill> SkillList { get; protected set; } = new List<Skill>();

	protected EAIObjectState _aiObjectState = EAIObjectState.None;
	public virtual EAIObjectState AIObjectState
	{
		get { return _aiObjectState; }
		set
		{
			_aiObjectState = value;

			OnChangedState();
		}
	}

	protected virtual void OnChangedState()
	{
		UpdateAnimation();
	}

	public bool IsAlive
	{
		get { return _aiObjectState != EAIObjectState.Dead; }
	}

	protected override void Init()
	{
		SkeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

		// Component
		if (GameObjectType == EGameObjectType.Buddy)
		{
			Status = Managers.Object.Hero.Status;
		}
		else
		{
			Status = GetComponent<BaseStatus>();
		}
	}

	public virtual void SetInfo(int templateID)
	{
		TemplateID = templateID;

		// Bind Event
		SkeletonAnimation.AnimationState.Event -= OnAnimEventHandler;
		SkeletonAnimation.AnimationState.Event += OnAnimEventHandler;
		SkeletonAnimation.AnimationState.Complete -= OnAnimCompleteHandler;
		SkeletonAnimation.AnimationState.Complete += OnAnimCompleteHandler;

		AIObjectState = EAIObjectState.Move;
	}

	#region Battle

	public Skill UseSkill { get; set; }

	public virtual void OnDamage(BaseAIObject attacker, int damage, bool isCritical, string hitEffectKey = "")
	{
		Status.TakeDamage(damage);

		Managers.Object.SpawnSkillEffect(transform.position + Vector3.up, hitEffectKey, 1.0f);

		// TODO: DamageText 생성
		if (isCritical)
		{
			UI_DamageText damageText = Managers.UI.MakeSubItem<UI_DamageText>(transform, "UI_DamageText");
			damageText.SetInfo(damage);
		}
		else
		{
			UI_DamageText damageText = Managers.UI.MakeSubItem<UI_DamageText>(transform, "UI_DamageText");
			damageText.SetInfo(damage);
		}
	}

	public virtual void OnDead()
	{
		AIObjectState = EAIObjectState.Dead;
	}

	public void ReboneObject()
	{
		Status.ResetHp();
		AIObjectState = EAIObjectState.Move;
	}

	public void StackAllSkillTurnCount()
	{
		foreach (Skill skill in SkillList)
		{
			skill.StackedTurnCount++;
		}
	}

	public void FullStackAllSkillTurnCount()
	{
		foreach (Skill skill in SkillList)
		{
			skill.StackedTurnCount = skill.NeedTurnCount;
		}
	}

	public List<Skill> GetCanUseSkillList()
	{
		List<Skill> canUseSkillList = new List<Skill>();
		foreach (Skill skill in SkillList)
		{
			if (skill.IsReadyToUse)
			{
				canUseSkillList.Add(skill);
			}
		}

		canUseSkillList.Add(DefaultSkill);

		return canUseSkillList;
	}

	#endregion

	#region Animation

	private const int DEFAULT_ORDER_IN_LAYER = 300;
	private const int DEAD_ORDER_IN_LAYER = 0;

	private const string ANIMATION_IDLE = "idle";
	private const string ANIMATION_MOVE = "move";
	private const string ANIMATION_HIT = "damage";
	private const string ANIMATION_DEAD = "dead";

	public string AttackAnimationName { get; set; }
	public bool IsAttackComplete { get; set; }

	protected virtual void UpdateAnimation()
	{
		switch (AIObjectState)
		{
			case EAIObjectState.Idle:
				SkeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = DEFAULT_ORDER_IN_LAYER;
				PlayAnimation(0, ANIMATION_IDLE, true);
				break;
			case EAIObjectState.Move:
				SkeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = DEFAULT_ORDER_IN_LAYER;
				PlayAnimation(0, ANIMATION_MOVE, true);
				break;
			case EAIObjectState.Attack:
				SkeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = DEFAULT_ORDER_IN_LAYER;
				PlayAnimation(0, AttackAnimationName, false);
				break;
			case EAIObjectState.Hit:
				SkeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = DEFAULT_ORDER_IN_LAYER;
				PlayAnimation(0, ANIMATION_HIT, false);
				break;
			case EAIObjectState.Dead:
				SkeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = DEAD_ORDER_IN_LAYER;
				PlayAnimation(0, ANIMATION_DEAD, false);
				break;

			default:
				break;
		}
	}

	public void PlayAnimation(int trackIndex, string animName, bool loop)
	{
		if (SkeletonAnimation == null)
		{
			return;
		}

		SkeletonAnimation.AnimationState.SetAnimation(trackIndex, animName, loop);
	}

	private bool _lookLeft = true;
	public bool LookLeft
	{
		get { return _lookLeft; }
		set
		{
			_lookLeft = value;
			Flip(!value);
		}
	}

	private void Flip(bool flag)
	{
		if (SkeletonAnimation == null)
		{
			return;
		}

		SkeletonAnimation.Skeleton.ScaleX = flag ? -1 : 1;
	}

	public void SetMeleeAttackMove()
	{
		BaseAIObject target = UseSkill.SkillTargetList.FirstOrDefault();
		if (target != null)
		{
			Vector3 direction = (target.transform.position - transform.position).normalized;
			Vector3 targetPosition = target.transform.position - direction * 3.0f;
			StartCoroutine(CoMeleeAttackMove(targetPosition, 0.2f));
		}
	}

	private IEnumerator CoMeleeAttackMove(Vector3 targetPosition, float moveDuration)
	{
		PlayAnimation(0, ANIMATION_MOVE, true);

		Vector3 startPosition = transform.position;
		float elapsedTime = 0f;

		// 이동
		while (elapsedTime < moveDuration)
		{
			transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		transform.position = targetPosition;

		AttackAnimationName = "attack";
		AIObjectState = EAIObjectState.Attack;

		// 공격 대기 시간
		yield return new WaitUntil(() => IsAttackComplete);

		// 복귀
		elapsedTime = 0f;
		while (elapsedTime < moveDuration)
		{
			transform.position = Vector3.Lerp(targetPosition, startPosition, elapsedTime / moveDuration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		transform.localPosition = Vector3.zero;
	}

	public virtual void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e)
	{
		UseSkill.UseSkill();
	}

	public virtual void OnAnimCompleteHandler(TrackEntry trackEntry)
	{
		if (AIObjectState == EAIObjectState.Attack || AIObjectState == EAIObjectState.Hit)
		{
			IsAttackComplete = AIObjectState == EAIObjectState.Attack ? true : false;
			PlayAnimation(0, ANIMATION_IDLE, true);
		}
	}

	#endregion
}
