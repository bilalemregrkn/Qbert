using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	private static readonly int MySpeed = Animator.StringToHash("MySpeed");

	private TileController _current;
	private bool _isMove;

	[Range(0, 1f)] [SerializeField] private float moveTime = .4f;
	[SerializeField] private AnimationCurve jumpCurve;
	[Range(0, 4f)] [SerializeField] private float jumpPower;
	[SerializeField] private Vector2Int startTile;
	[SerializeField] private Animator jumpAnimation;

	[Range(0, 2f)] [SerializeField] private float AITime = 1f;

	private IEnumerator Start()
	{
		_current = TowerManager.Instance.GetTile(startTile);
		while (true)
		{
			yield return new WaitForSeconds(AITime);
			var tile = _current.GetRandomNeighbour();
			Move(tile);
			// UpdateRotate(direction);
		}

		// ReSharper disable once IteratorNeverReturns
	}
	
	private void UpdateRotate(Direction direction)
	{
		float angle = direction switch
		{
			Direction.UpLeft => 90,
			Direction.UpRight => 180,
			Direction.DownLeft => 0,
			Direction.DownRight => 270,
			_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
		};

		var eulerAngles = transform.eulerAngles;
		eulerAngles.y = angle;
		transform.eulerAngles = eulerAngles;
	}

	private void Move(TileController tileController)
	{
		if (!tileController) return;
		if (_isMove) return;

		_isMove = true;

		var animationSpeed = 1f / moveTime;
		jumpAnimation.SetFloat(MySpeed, animationSpeed);

		IEnumerator _Move()
		{
			jumpAnimation.Play(0);
			yield return Animation(tileController.snapPoint);
			_current = tileController;
			_isMove = false;
		}

		StartCoroutine(_Move());
	}

	private IEnumerator Animation(Transform target)
	{
		var time = moveTime;
		var passed = 0f;
		var init = transform.position;
		while (passed < time)
		{
			passed += Time.deltaTime;
			var normalize = passed / time;
			var position = Vector3.Lerp(init, target.position, normalize);
			position.y += jumpCurve.Evaluate(normalize) * jumpPower;
			transform.position = position;
			yield return null;
		}

		transform.position = target.position;
	}
}