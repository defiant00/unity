using System;
using UnityEngine;

public class Indicator : MonoBehaviour
{
	[NonSerialized]
	public SpriteRenderer SpriteRenderer;

	void Start()
	{
		SpriteRenderer = GetComponent<SpriteRenderer>();
	}
}
