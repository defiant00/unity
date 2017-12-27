using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	const int MOVE_SPEED = 12;

	public int PosX, PosY, Movement = 5;

	GameState State;

	Stack<Vector3> MovementPoints = new Stack<Vector3>();

	void Start()
	{
		transform.position = new Vector3(PosX, PosY);
		State = FindObjectOfType<GameState>();
	}

	void Update()
	{
		if (State.TurnState == TurnState.Moving && MovementPoints.Count > 0)
		{
			transform.position = Vector3.MoveTowards(transform.position, MovementPoints.Peek(), Time.deltaTime * MOVE_SPEED);

			if (transform.position == MovementPoints.Peek())
			{
				MovementPoints.Pop();
				if (MovementPoints.Count == 0) { State.TurnState = TurnState.Idle; }
			}
		}
	}

	public void MoveTo(int x, int y)
	{
		if (x != PosX || y != PosY)
		{
			MovementPoints.Clear();
			MovementPoints.Push(new Vector3(x, y));
			int cx = x, cy = y;
			while (cx != PosX || cy != PosY)
			{
				int cw = State.MovementMap[cx, cy];
				if (State.ValidMovementStep(cx - 1, cy, cw)) { cx--; }
				else if (State.ValidMovementStep(cx + 1, cy, cw)) { cx++; }
				else if (State.ValidMovementStep(cx, cy - 1, cw)) { cy--; }
				else if (State.ValidMovementStep(cx, cy + 1, cw)) { cy++; }
				else { throw new Exception("Error, unable to find a path from " + PosX + ", " + PosY + " to " + x + ", " + y); }
				MovementPoints.Push(new Vector3(cx, cy));
			}

			PosX = x;
			PosY = y;

			State.TurnState = TurnState.Moving;
		}
		else
		{
			State.TurnState = TurnState.Idle;
		}
	}
}
