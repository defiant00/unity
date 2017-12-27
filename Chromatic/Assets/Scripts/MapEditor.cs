using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
	void OnSceneGUI()
	{
		var cm = target as Map;
		const float size = 0.45f;
		var offset = new Vector3(size, size);
		var invOffset = new Vector3(size, -size);
		for (int x = 0; x < cm.X; x++)
		{
			for (int y = 0; y < cm.Y; y++)
			{
				var pos = new Vector3(x, y) + cm.transform.position + Vector3.one;
				if (cm.GetCollision(x, y))
				{
					Handles.DrawLine(pos - offset, pos + offset);
					Handles.DrawLine(pos - invOffset, pos + invOffset);
				}
				if (Handles.Button(pos, Quaternion.identity, size, size, Handles.RectangleHandleCap))
				{
					Undo.RecordObject(cm, "Toggle Collision Map");
					cm.SetCollision(x, y, !cm.GetCollision(x, y));
				}
			}
		}
	}
}
