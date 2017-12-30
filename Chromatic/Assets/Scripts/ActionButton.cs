using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
	public string Text;
	[NonSerialized]
	public UnityAction Click;

	public Vector2 Position
	{
		set
		{
			GetComponent<RectTransform>().anchoredPosition = value;
		}
	}

	void Start()
	{
		SetText();
		GetComponent<Button>().onClick.AddListener(Click);
	}

	void OnValidate() { SetText(); }

	void SetText()
	{
		GetComponentInChildren<Text>().text = Text;
	}
}
