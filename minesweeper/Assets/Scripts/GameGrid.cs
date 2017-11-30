using System.Linq;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
	public GameTile TilePrefab;
	public int Width = 10, Height = 10, NumMines = 10;

	GameTile[,] Tiles;

	// Use this for initialization
	void Start()
	{
		CreateTiles();
		AssignMines();
	}

	// Update is called once per frame
	void Update()
	{

	}

	void CreateTiles()
	{
		Tiles = new GameTile[Width, Height];

		for (int w = 0; w < Width; w++)
		{
			for (int h = 0; h < Height; h++)
			{
				Tiles[w, h] = Instantiate(TilePrefab, new Vector3(transform.position.x + w, transform.position.y + h), transform.rotation);
				Tiles[w, h].Grid = this;
			}
		}
	}

	void AssignMines()
	{
		for (int i = 0; i < NumMines; i++)
		{
			int w = 0, h = 0;
			do
			{
				w = (int)(Random.value * (Width - 1) + 0.5);
				h = (int)(Random.value * (Height - 1) + 0.5);
			} while (Tiles[w, h].IsMine);
			Tiles[w, h].IsMine = true;
		}
	}

	public void TileClicked(int w, int h)
	{
		if (Tiles[w, h].IsMine)
		{
			foreach (var t in Tiles)
			{
				if (t.IsMine) { t.SetTexture(0); }
			}
		}
		else
		{
			FloodClear(w, h);
		}
	}

	void FloodClear(int w, int h)
	{
		if (w > -1 && h > -1 && w < Width && h < Height && Tiles[w, h].Covered)
		{
			int c = SurroundingCount(w, h);
			Tiles[w, h].SetTexture(c);
			Tiles[w, h].Covered = false;
			if (c == 0)
			{
				FloodClear(w - 1, h - 1);
				FloodClear(w, h - 1);
				FloodClear(w + 1, h - 1);
				FloodClear(w - 1, h);
				FloodClear(w + 1, h);
				FloodClear(w - 1, h + 1);
				FloodClear(w, h + 1);
				FloodClear(w + 1, h + 1);
			}
		}
	}

	int SurroundingCount(int w, int h)
	{
		int count = IsMine(w - 1, h - 1) ? 1 : 0;
		if (IsMine(w, h - 1)) count++;
		if (IsMine(w + 1, h - 1)) count++;
		if (IsMine(w - 1, h)) count++;
		if (IsMine(w + 1, h)) count++;
		if (IsMine(w - 1, h + 1)) count++;
		if (IsMine(w, h + 1)) count++;
		if (IsMine(w + 1, h + 1)) count++;
		return count;
	}

	bool IsMine(int w, int h)
	{
		return (w > -1 && h > -1 && w < Width && h < Height) ? Tiles[w, h].IsMine : false;
	}
}
