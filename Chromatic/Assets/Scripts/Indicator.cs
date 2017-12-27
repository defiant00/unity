using UnityEngine;

public class Indicator : MonoBehaviour
{
	[HideInInspector]
	public Map Map;

	public bool Visible
	{
		get { return GetComponent<SpriteRenderer>().enabled; }
	}

	void OnMouseDown()
	{
		if (Visible)
		{
			Map.State.SelectedCharacter.MoveTo((int)transform.position.x, (int)transform.position.y);
		}
		Map.ClearIndicators();
	}
}
