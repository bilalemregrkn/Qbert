using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private static readonly int MySpeed = Animator.StringToHash("MySpeed");
	private bool IsPressLeft => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
	private bool IsPressRight => Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
	private bool IsPressUp => Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

	private TileController _current;
	private bool _isMove;

	[Range(0, 1f)] [SerializeField] private float moveTime = .4f;
	[SerializeField] private AnimationCurve jumpCurve;
	[Range(0, 4f)] [SerializeField] private float jumpPower;
	[SerializeField] private Vector2Int startTile;
	[SerializeField] private Animator jumpAnimation;


	private void Start()
	{
		var tile = TowerManager.Instance.GetTile(startTile);
		Move(tile);
	}

	private void Update()
	{
		if (IsPressLeft || IsPressRight)
		{
			var up = IsPressLeft ? Direction.UpLeft : Direction.UpRight;
			var down = IsPressLeft ? Direction.DownLeft : Direction.DownRight;

			var direction = IsPressUp ? up : down;
			var tile = _current.GetNeighbour(direction);
			Move(tile);
			UpdateRotate(direction);
		}
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
			tileController.SetActive(true);
			_current = tileController;

			_isMove = false;

			TowerManager.Instance.GameCheck();
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

public class CanvasController : MonoBehaviour
{
}

public class GameManager : MonoBehaviour
{
}