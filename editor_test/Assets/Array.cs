using UnityEngine;

[ExecuteInEditMode]
public class Array : MonoBehaviour
{
	public int X = 10, Y = 10;

	[HideInInspector]
	public bool[] Values;

	public bool this[int xv, int yv]
	{
		get { return Values[xv + yv * X]; }
		set { Values[xv + yv * X] = value; }
	}

	void OnValidate()
	{
		if (Values == null || Values.Length != (X * Y))
		{
			Values = new bool[X * Y];
		}
	}
}
