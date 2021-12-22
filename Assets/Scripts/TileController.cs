using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Direction
{
	UpLeft,
	UpRight,
	DownLeft,
	DownRight
}

public class TileController : MonoBehaviour
{
	public Vector2Int coordinate;
	public Transform snapPoint;

	[SerializeField] private SpriteRenderer spriteRenderer;

	public SpriteRenderer SpriteRenderer => spriteRenderer;

	public TileController GetNeighbour(Direction direction)
	{
		var targetCoordinate = coordinate;

		switch (direction)
		{
			case Direction.UpLeft:
				targetCoordinate.x--;
				targetCoordinate.y++;
				break;
			case Direction.UpRight:
				targetCoordinate.x++;
				targetCoordinate.y++;
				break;
			case Direction.DownLeft:
				targetCoordinate.x--;
				targetCoordinate.y--;
				break;
			case Direction.DownRight:
				targetCoordinate.x++;
				targetCoordinate.y--;
				break;
		}

		return TowerManager.Instance.GetTile(targetCoordinate);
	}

	public List<TileController> GetAllNeighbours()
	{
		var result = new List<TileController>();

		var lenght = Enum.GetNames(typeof(Direction)).Length;
		for (int i = 0; i < lenght; i++)
		{
			var tile = GetNeighbour((Direction)i);
			if (tile) result.Add(tile);
		}

		return result;
	}

	public TileController GetRandomNeighbour()
	{
		var tiles = GetAllNeighbours();
		return tiles[Random.Range(0, tiles.Count)];
	}

	public bool Active { get; private set; }

	public void SetActive(bool active)
	{
		var manager = TowerManager.Instance;
		spriteRenderer.color = active ? manager.ActiveColor : manager.InactiveColor;
		Active = active;
	}

	public void PlayWinAnimation()
	{
		IEnumerator Do()
		{
			var manager = TowerManager.Instance;
			for (int i = 0; i < 2; i++)
				foreach (var color in manager.ListColor)
				{
					yield return new WaitForSeconds(.15f);
					spriteRenderer.color = color;
				}
		}

		StartCoroutine(Do());
	}
}