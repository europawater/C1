using System.Collections;
using UnityEngine;
using static Define;

public class Projectile : BaseObject
{
	private Vector2 _startPosition;
	private Vector2 _endPosition;
	private float _lifeTime;
	private float _heightArc = 2.0f;

	protected override void Init()
	{
		GameObjectType = EGameObjectType.Projectile;
	}

	public void SetInfo(Vector2 startPosition, Vector2 endPosition, float lifeTime = 0.3f)
	{
		_startPosition = startPosition;
		_endPosition = endPosition;
		_lifeTime = lifeTime;

		StartCoroutine(CoProjectilMoveHandler());
	}

	private IEnumerator CoProjectilMoveHandler()
	{
		float startTime = Time.time;
		while (Time.time - startTime < _lifeTime)
		{
			float normalizedTime = (Time.time - startTime) / _lifeTime;

			// 포물선 모양으로 이동
			float x = Mathf.Lerp(_startPosition.x, _endPosition.x, normalizedTime);
			float baseY = Mathf.Lerp(_startPosition.y, _endPosition.y, normalizedTime);
			float arc = _heightArc * Mathf.Sin(normalizedTime * Mathf.PI);

			float y = baseY + arc;

			var nextPos = new Vector3(x, y);
			transform.position = nextPos;

			yield return null;
		}

		// 목표 지점에 도착하면 객체 제거
		transform.position = _endPosition;
		DestroyObject();
	}

	protected override void DestroyObject()
	{
		Managers.Object.RemoveProjectile(this);
	}
}
