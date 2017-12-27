using System;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
	Ready,
	Done,
}

public class Unit : MonoBehaviour
{
	const int MOVE_SPEED = 12;

	public int X, Y;
	public int Player, Team;
	public int Speed = 8, Range = 1;

	[NonSerialized]
	public UnitState UnitState;

	GameState State;

	Stack<Vector3> MovementPoints = new Stack<Vector3>();
	int PriorX, PriorY;

	public bool IsPlayerUnit
	{
		get { return Player == 0 && Team == 0; }
	}

	void Start()
	{
		transform.position = new Vector3(X, Y);
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
				if (MovementPoints.Count == 0) { PickAction(); }
			}
		}
	}

	void OnValidate()
	{
		transform.position = new Vector3(X, Y);
	}

	public void MoveTo(int x, int y)
	{
		PriorX = X;
		PriorY = Y;

		if (x != X || y != Y)
		{
			MovementPoints.Clear();
			MovementPoints.Push(new Vector3(x, y));
			int cx = x, cy = y;
			while (cx != X || cy != Y)
			{
				int cw = State.MovementMap[cx, cy];
				if (State.ValidMovementStep(cx - 1, cy, cw)) { cx--; }
				else if (State.ValidMovementStep(cx + 1, cy, cw)) { cx++; }
				else if (State.ValidMovementStep(cx, cy - 1, cw)) { cy--; }
				else if (State.ValidMovementStep(cx, cy + 1, cw)) { cy++; }
				else { throw new Exception("Error, unable to find a path from " + X + ", " + Y + " to " + x + ", " + y); }
				MovementPoints.Push(new Vector3(cx, cy));
			}

			X = x;
			Y = y;

			State.TurnState = TurnState.Moving;
		}
		else { PickAction(); }
	}

	public void ResetMove()
	{
		X = PriorX;
		Y = PriorY;
		transform.position = new Vector3(X, Y);
	}

	void PickAction()
	{
		State.TurnState = TurnState.PickAction;
	}
}
