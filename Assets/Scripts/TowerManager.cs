using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
	[SerializeField] private TileController tilePrefab;
	[SerializeField] private int amount;

	[SerializeField] private List<TileController> listTile = new List<TileController>();


	[SerializeField] private List<Color> listColor;

	[SerializeField] private Color activeColor;

	[SerializeField] private Color inactiveColor;

	public List<Color> ListColor => listColor;
	public Color InactiveColor => inactiveColor;
	public Color ActiveColor => activeColor;

	public static TowerManager Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		Prepare();
	}

	public TileController GetTile(Vector2Int coordinate)
	{
		return listTile.Find(x => x.coordinate == coordinate);
	}

	[ContextMenu(nameof(Create))]
	public void Create()
	{
		Create(amount);
	}

	private void Prepare()
	{
		foreach (var tile in listTile)
		{
			tile.SpriteRenderer.enabled = true;
			tile.SetActive(false);
		}
	}

	private void Create(int firstStepCubeAmount)
	{
		listTile = new List<TileController>();
		var parent = new GameObject()
		{
			transform = { name = "Parent" }
		};

		Vector3 position = Vector3.zero;
		Vector2Int coordinate = Vector2Int.zero;
		for (int step = firstStepCubeAmount, height = 0; step > 0; step--, height++)
		{
			for (int count = 0; count < step; count++)
			{
				var tile = Instantiate(tilePrefab, position, Quaternion.identity, parent.transform);
				tile.coordinate = coordinate;
				tile.transform.name = $"Tile [{coordinate.x},{coordinate.y}]";
				listTile.Add(tile);

				coordinate.x += 2;


				position.x++;
				position.z++;
			}


			coordinate.y++;
			coordinate.x = (height + 1);

			position.y++;

			position.z = (height + 1);
			position.x = 0;
		}
	}

	public bool HasUnmark()
	{
		foreach (var tile in listTile)
		{
			if (!tile.Active)
				return true;
		}

		return false;
	}

	public void GameCheck()
	{
		var finish = !HasUnmark();
		if (finish)
		{
			Debug.Log("Game End");
			foreach (var tile in listTile)
			{
				tile.PlayWinAnimation();
			}
		}
	}

	[ContextMenu(nameof(TEST_WIN))]
	public void TEST_WIN()
	{
		foreach (var tile in listTile)
		{
			tile.PlayWinAnimation();
		}
	}
}