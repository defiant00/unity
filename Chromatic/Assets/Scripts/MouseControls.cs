using UnityEngine;

public class MouseControls : MonoBehaviour
{
	Camera Camera;
	Map Map;

	void Start()
	{
		Camera = GameObject.Find("Camera").GetComponent<Camera>();
		Map = GetComponent<Map>();
	}

	void Update()
	{
		bool left = Input.GetMouseButtonDown(0);
		bool right = Input.GetMouseButtonDown(1);

		if (left || right)
		{
			var p = Camera.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0.5f, 0.5f);
			if (p.x >= 0 && p.y >= 0 && p.x < Map.X && p.y < Map.Y)
			{
				Map.Clicked((int)p.x, (int)p.y, !right);
			}
		}
	}
}
