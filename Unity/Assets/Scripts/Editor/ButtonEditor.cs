using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Button))]
[CanEditMultipleObjects]
internal class ButtonEditor : Editor
{
	private void OnSceneGUI()
	{
		var button = target as Button;

		EditorGUI.BeginChangeCheck();

		var control = Handles.PositionHandle(button.control, Quaternion.identity);

		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Changed button");
			button.control = control;
		}

		Handles.DrawWireDisc(button.position, Vector3.forward, .2f);
		Handles.DrawLine(button.position, button.control);
	}
}
