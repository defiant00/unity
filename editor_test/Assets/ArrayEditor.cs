using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Array))]
public class ArrayEditor : Editor
{
	void OnSceneGUI()
	{
		Handles.color = Color.blue;

		var a = target as Array;
		const float size = 0.45f;
		var offset = new Vector3(size, size);
		var invOffset = new Vector3(size, -size);
		for (int x = 0; x < a.X; x++)
		{
			for (int y = 0; y < a.Y; y++)
			{
				var pos = new Vector3(x, y) + a.transform.position;
				if (a[x, y])
				{
					Handles.DrawLine(pos - offset, pos + offset);
					Handles.DrawLine(pos - invOffset, pos + invOffset);
				}
				if(Handles.Button(pos, Quaternion.identity, size, size, Handles.RectangleHandleCap))
				{
					Undo.RecordObject(a, "Toggle value");
					a[x, y] = !a[x, y];
				}
			}
		}
	}
}
