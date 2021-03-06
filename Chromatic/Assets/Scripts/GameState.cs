﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum TurnState
{
	Ready,
	SelectMoveLocation,
	Moving,
	PickAction,
}

public class GameState : MonoBehaviour
{
	public int X = 10, Y = 10;
	public Indicator Indicator;
	public ActionButton ActionButton;
	public Color MoveColor = new Color(0.4f, 0.6f, 1);
	public Color AttackColor = new Color(1, 0.2f, 0.2f);

	[HideInInspector]
	public bool[] CollisionMap;

	[NonSerialized]
	public TurnState TurnState;
	[NonSerialized]
	public int[,] MovementMap;

	Indicator[,] Indicators;
	List<Unit> Units;
	Unit SelectedUnit;
	Camera Camera;
	Canvas Canvas;
	bool[,] TeamMap;

	public bool GetCollisionMap(int x, int y)
	{
		return CollisionMap[x + y * X];
	}

	public void SetCollisionMap(int x, int y, bool value)
	{
		CollisionMap[x + y * X] = value;
	}

	public bool ValidMovementStep(int x, int y, int weight)
	{
		return x > -1 && y > -1 && x < X && y < Y && MovementMap[x, y] > weight;
	}

	void Select(int x, int y)
	{
		switch (TurnState)
		{
			case TurnState.Ready:
				var un = Units.FirstOrDefault(u => u.X == x && u.Y == y && u.UnitState == UnitState.Ready && u.IsPlayerUnit);
				if (un != null)
				{
					SelectedUnit = un;
					CalcMovementMap(un.X, un.Y, un.Speed, un.Team);
					TurnState = TurnState.SelectMoveLocation;
				}
				break;
			case TurnState.SelectMoveLocation:
				if (MovementMap[x, y] > 0)
				{
					SelectedUnit.MoveTo(x, y);
					ClearIndicators();
				}
				break;
		}
	}

	void Cancel()
	{
		switch (TurnState)
		{
			case TurnState.SelectMoveLocation:
				TurnState = TurnState.Ready;
				ClearIndicators();
				break;
			case TurnState.PickAction:
				TurnState = TurnState.Ready;
				SelectedUnit.ResetMove();
				ClearActionUI();
				ClearIndicators();
				break;
		}
	}

	void Attack()
	{
		Debug.Log("attack");
	}

	void Fuse()
	{
		Debug.Log("fuse");
	}

	void Wait()
	{
		SelectedUnit.UnitState = UnitState.Done;
		FinishAction();
	}

	void FinishAction()
	{
		ClearActionUI();
		SelectedUnit = null;
		TurnState = TurnState.Ready;
	}

	public void PickAction()
	{
		TurnState = TurnState.PickAction;

		var names = new List<string>();
		var acts = new List<UnityAction>();

		var fusionUnit = GetFusionUnit(SelectedUnit);
		if (fusionUnit != null)
		{
			names.Add("Fuse");
			acts.Add(() => Fuse());
		}
		else
		{
			ShowAttackRange(SelectedUnit);

			var targets = GetAttackList(SelectedUnit);
			if (targets.Count > 0)
			{
				names.Add("Attack");
				acts.Add(() => Attack());
			}

			names.Add("Wait");
			acts.Add(() => Wait());
		}

		GenActionUI(names, acts);
	}

	void ShowAttackRange(Unit u)
	{
		for (int x = 0; x < X; x++)
		{
			for (int y = 0; y < Y; y++)
			{
				int d = Distance(u.X, u.Y, x, y);
				SetIndicator(x, y, d >= u.MinRange && d <= u.MaxRange, AttackColor);
			}
		}
	}

	List<Unit> GetAttackList(Unit unit)
	{
		return Units.Where(u =>
		{
			int d = UnitDistance(u, unit);
			return u.Team != unit.Team && d >= unit.MinRange && d <= unit.MaxRange;
		}).ToList();
	}

	Unit GetFusionUnit(Unit unit)
	{
		return Units.FirstOrDefault(u => u != unit && u.Player == unit.Player && u.X == unit.X && u.Y == unit.Y && (u.Weight + unit.Weight) <= 3);
	}

	int UnitDistance(Unit u1, Unit u2)
	{
		return Distance(u1.X, u1.Y, u2.X, u2.Y);
	}

	int Distance(int x1, int y1, int x2, int y2)
	{
		return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
	}

	void GenActionUI(List<string> names, List<UnityAction> actions)
	{
		for (int i = 0; i < names.Count; i++)
		{
			var b = Instantiate(ActionButton, Vector3.zero, Quaternion.identity);
			b.Text = names[i];
			b.Click = actions[i];
			b.transform.SetParent(Canvas.transform, false);
			b.Position = new Vector2(-100, -40 - i * 40);
		}
	}

	void ClearActionUI()
	{
		foreach (var b in GetComponentsInChildren<ActionButton>()) { Destroy(b.gameObject); }
	}

	void ClearMovementMap()
	{
		for (int x = 0; x < X; x++)
		{
			for (int y = 0; y < Y; y++)
			{
				MovementMap[x, y] = 0;
			}
		}
	}

	void DisplayMovementMap()
	{
		for (int x = 0; x < X; x++)
		{
			for (int y = 0; y < Y; y++)
			{
				SetIndicator(x, y, MovementMap[x, y] > 0, MoveColor);
			}
		}
	}

	void CalcTeamMap(int team)
	{
		for (int x = 0; x < X; x++)
		{
			for (int y = 0; y < Y; y++)
			{
				TeamMap[x, y] = false;
			}
		}
		foreach (var un in Units.Where(u => u.Team != team))
		{
			TeamMap[un.X, un.Y] = true;
		}
	}

	void CalcMovementMap(int x, int y, int mov, int team)
	{
		var steps = new Queue<MovementStep>();
		ClearMovementMap();
		CalcTeamMap(team);
		steps.Enqueue(new MovementStep(x, y, mov + 1));   // Add 1 to mov since we're treating 0 as a point that can't be reached
		while (steps.Count > 0)
		{
			var ms = steps.Dequeue();
			if (ms.X > -1 && ms.Y > -1 && ms.X < X && ms.Y < Y && !GetCollisionMap(ms.X, ms.Y) && !TeamMap[ms.X, ms.Y] && MovementMap[ms.X, ms.Y] < ms.Weight)
			{
				MovementMap[ms.X, ms.Y] = ms.Weight;
				steps.Enqueue(new MovementStep(ms.X - 1, ms.Y, ms.Weight - 1));
				steps.Enqueue(new MovementStep(ms.X + 1, ms.Y, ms.Weight - 1));
				steps.Enqueue(new MovementStep(ms.X, ms.Y - 1, ms.Weight - 1));
				steps.Enqueue(new MovementStep(ms.X, ms.Y + 1, ms.Weight - 1));
			}
		}
		DisplayMovementMap();
	}

	void SetIndicator(int x, int y, bool display, Color color)
	{
		var sr = Indicators[x, y].SpriteRenderer;
		sr.enabled = display;
		sr.color = color;
	}

	void ClearIndicators()
	{
		foreach (var i in Indicators) { i.SpriteRenderer.enabled = false; }
	}

	void Start()
	{
		// Get the camera.
		Camera = GameObject.Find("Camera").GetComponent<Camera>();

		// Get the canvas.
		Canvas = GetComponentInChildren<Canvas>();

		// Setup arrays.
		MovementMap = new int[X, Y];
		TeamMap = new bool[X, Y];

		// Setup the indicators.
		var root = new GameObject("Indicators");
		Indicators = new Indicator[X, Y];
		for (int x = 0; x < X; x++)
		{
			for (int y = 0; y < Y; y++)
			{
				var i = Instantiate(Indicator, new Vector3(x, y), Quaternion.identity);
				Indicators[x, y] = i;
				i.transform.parent = root.transform;
			}
		}

		// Get all the characters.
		Units = FindObjectsOfType<Unit>().ToList();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(1))    // right
		{
			Cancel();
		}
		else if (Input.GetMouseButtonDown(0))   // left
		{
			var p = Camera.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0.5f, 0.5f);
			if (p.x >= 0 && p.y >= 0 && p.x < X && p.y < Y)
			{
				Select((int)p.x, (int)p.y);
			}
		}
	}

	void OnValidate()
	{
		if (CollisionMap == null || CollisionMap.Length != (X * Y))
		{
			CollisionMap = new bool[X * Y];
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
