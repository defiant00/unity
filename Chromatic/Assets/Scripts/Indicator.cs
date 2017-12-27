using System;
using UnityEngine;

public class Indicator : MonoBehaviour
{
	[NonSerialized]
	public Map Map;
	[NonSerialized]
	public SpriteRenderer SpriteRenderer;

	void Start()
	{
		SpriteRenderer = GetComponent<SpriteRenderer>();
	}

	public bool Visible
	{
		get { return SpriteRenderer.enabled; }
	}
}
