using System.Collections.Generic;
using UnityEngine;

public enum CharacterState
{
	Ready,
	Done,
}

public class Character : MonoBehaviour
{
	public int PosX, PosY, Movement = 5;

	CharacterState CState;
	Map Map;

	void Start()
	{
		transform.position = new Vector3(PosX, PosY);
		Map = GameObject.Find("Map").GetComponent<Map>();
	}

	void Update()
	{

	}

	public void MoveTo(int x, int y)
	{
		PosX = x;
		PosY = y;
		transform.position = new Vector3(PosX, PosY);
	}

	void OnMouseDown()
	{
		Map.State.SelectedCharacter = this;
		CalcMovementMap();
	}

	void CalcMovementMap()
	{
		var steps = new Queue<MovementStep>();
		var weights = new int[Map.X, Map.Y];
		steps.Enqueue(new MovementStep(PosX, PosY, Movement));
		while (steps.Count > 0)
		{
			var ms = steps.Dequeue();
			if (ms.X > -1 && ms.Y > -1 && ms.X < Map.X && ms.Y < Map.Y && !Map.GetCollision(ms.X, ms.Y) && weights[ms.X, ms.Y] < ms.Weight)
			{
				weights[ms.X, ms.Y] = ms.Weight;
				steps.Enqueue(new MovementStep(ms.X - 1, ms.Y, ms.Weight - 1));
				steps.Enqueue(new MovementStep(ms.X + 1, ms.Y, ms.Weight - 1));
				steps.Enqueue(new MovementStep(ms.X, ms.Y - 1, ms.Weight - 1));
				steps.Enqueue(new MovementStep(ms.X, ms.Y + 1, ms.Weight - 1));
			}
		}

		for (int x = 0; x < Map.X; x++)
		{
			for (int y = 0; y < Map.Y; y++)
			{
				Map.SetIndicator(x, y, weights[x, y] > 0, Color.white);
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
}
