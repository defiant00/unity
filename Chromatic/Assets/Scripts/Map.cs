using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
	public int X = 10, Y = 10;
	public Indicator Indicator;

	[NonSerialized]
	public GameState State = new GameState();

	[HideInInspector]
	public bool[] Values;

	Indicator[] Indicators;
	List<Character> Characters;

	public bool GetCollision(int xv, int yv)
	{
		return Values[xv + yv * X];
	}

	public void SetCollision(int xv, int yv, bool value)
	{
		Values[xv + yv * X] = value;
	}

	Indicator GetIndicator(int xv, int yv)
	{
		return Indicators[xv + yv * X];
	}

	void SetIndicator(int xv, int yv, Indicator value)
	{
		Indicators[xv + yv * X] = value;
	}

	public bool ValidMovementStep(int x, int y, int weight)
	{
		return x > -1 && y > -1 && x < X && y < Y && State.MovementMap[x, y] > weight;
	}

	public void Clicked(int x, int y, bool left)
	{
		if (left)
		{
			switch (State.TurnState)
			{
				case TurnState.Idle:
					var ch = Characters.FirstOrDefault(c => c.PosX == x && c.PosY == y);
					if (ch != null)
					{
						State.SelectedCharacter = ch;
						CalcMovementMap(ch.PosX, ch.PosY, ch.Movement);
						State.TurnState = TurnState.SelectMoveLocation;
					}
					break;
				case TurnState.SelectMoveLocation:
					if (GetIndicator(x, y).Visible)
					{
						State.SelectedCharacter.MoveTo(x, y);
						ClearIndicators();
					}
					break;
			}
		}
		else
		{
			switch (State.TurnState)
			{
				case TurnState.SelectMoveLocation:
					State.TurnState = TurnState.Idle;
					ClearIndicators();
					break;
			}
		}
	}

	void CalcMovementMap(int px, int py, int mov)
	{
		var steps = new Queue<MovementStep>();
		State.MovementMap = new int[X, Y];
		steps.Enqueue(new MovementStep(px, py, mov + 1));   // Add 1 to mov since we're treating 0 as a point that can't be reached
		while (steps.Count > 0)
		{
			var ms = steps.Dequeue();
			if (ms.X > -1 && ms.Y > -1 && ms.X < X && ms.Y < Y && !GetCollision(ms.X, ms.Y) && State.MovementMap[ms.X, ms.Y] < ms.Weight)
			{
				State.MovementMap[ms.X, ms.Y] = ms.Weight;
				steps.Enqueue(new MovementStep(ms.X - 1, ms.Y, ms.Weight - 1));
				steps.Enqueue(new MovementStep(ms.X + 1, ms.Y, ms.Weight - 1));
				steps.Enqueue(new MovementStep(ms.X, ms.Y - 1, ms.Weight - 1));
				steps.Enqueue(new MovementStep(ms.X, ms.Y + 1, ms.Weight - 1));
			}
		}

		for (int x = 0; x < X; x++)
		{
			for (int y = 0; y < Y; y++)
			{
				SetIndicator(x, y, State.MovementMap[x, y] > 0, Color.white);
			}
		}
	}

	class MovementStep
	{
		public int X, Y, Weight;

		public MovementStep(int x, int y, int weight)
		{
			X = x;
			Y = y;
			Weight = weight;
		}
	}

	void SetIndicator(int xv, int yv, bool display, Color color)
	{
		var sr = Indicators[xv + yv * X].SpriteRenderer;
		sr.enabled = display;
		sr.color = color;
	}

	void ClearIndicators()
	{
		foreach (var i in Indicators) { i.SpriteRenderer.enabled = false; }
	}

	void Start()
	{
		// Setup the indicators.
		Indicators = new Indicator[X * Y];
		for (int x = 0; x < X; x++)
		{
			for (int y = 0; y < Y; y++)
			{
				var i = Instantiate(Indicator, new Vector3(x, y), Quaternion.identity);
				i.Map = this;
				SetIndicator(x, y, i);
			}
		}

		// Get all the characters.
		Characters = FindObjectsOfType<Character>().ToList();
	}

	void OnValidate()
	{
		if (Values == null || Values.Length != (X * Y))
		{
			Values = new bool[X * Y];
		}
	}
}
