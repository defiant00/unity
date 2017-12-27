using UnityEngine;


public class Map : MonoBehaviour
{
	public int X = 10, Y = 10;
	public Indicator Indicator;

	[HideInInspector]
	public GameState State = new GameState();

	[HideInInspector]
	public bool[] Values;

	Indicator[] Indicators;

	public bool GetCollision(int xv, int yv)
	{
		return Values[xv + yv * X];
	}

	public void SetCollision(int xv, int yv, bool value)
	{
		Values[xv + yv * X] = value;
	}

	void SetIndicator(int xv, int yv, Indicator value)
	{
		Indicators[xv + yv * X] = value;
	}

	public void SetIndicator(int xv, int yv, bool display, Color color)
	{
		var sr = Indicators[xv + yv * X].GetComponent<SpriteRenderer>();
		sr.enabled = display;
		sr.color = color;
	}

	public void ClearIndicators()
	{
		foreach (var i in Indicators) { i.GetComponent<SpriteRenderer>().enabled = false; }
	}

	void Start()
	{
		Indicators = new Indicator[X * Y];
		for (int x = 0; x < X; x++)
		{
			for (int y = 0; y < Y; y++)
			{
				var i = Instantiate(Indicator, new Vector3(x, y), Quaternion.identity);
				i.GetComponent<SpriteRenderer>().enabled = false;
				i.Map = this;
				SetIndicator(x, y, i);
			}
		}
	}

	void OnValidate()
	{
		if (Values == null || Values.Length != (X * Y))
		{
			Values = new bool[X * Y];
		}
	}
}
