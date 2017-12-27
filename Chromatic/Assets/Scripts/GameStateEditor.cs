using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameState))]
public class GameStateEditor : Editor
{
	void OnSceneGUI()
	{
		var st = target as GameState;
		const float size = 0.45f;
		var offset = new Vector3(size, size);
		var invOffset = new Vector3(size, -size);
		for (int x = 0; x < st.X; x++)
		{
			for (int y = 0; y < st.Y; y++)
			{
				var pos = new Vector3(x, y) + st.transform.position + Vector3.one;
				if (st.GetCollisionMap(x, y))
				{
					Handles.DrawLine(pos - offset, pos + offset);
					Handles.DrawLine(pos - invOffset, pos + invOffset);
				}
				if (Handles.Button(pos, Quaternion.identity, size, size, Handles.RectangleHandleCap))
				{
					Undo.RecordObject(st, "Toggle Collision Map");
					st.SetCollisionMap(x, y, !st.GetCollisionMap(x, y));
				}
			}
		}
	}
}
