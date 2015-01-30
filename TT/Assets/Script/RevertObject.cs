using UnityEngine;
using UnityEditor;
using System.Collections;

public class RevertObject : EditorWindow
{
	[MenuItem("Jay/RevertPrefabs")]
	public static void ShowWindow ()
	{
		EditorWindow.GetWindow (typeof(RevertObject));
	}

	public void OnGUI ()
	{
		GameObject[] selections = Selection.gameObjects;
		if (GUILayout.Button ("Revert")) {
			Revert (selections);
		}
		if (GUILayout.Button ("CascadeRevert")) {
			CascadeRevert (selections);
		}
	}

	private static void CascadeRevert (GameObject[] selections)
	{
		foreach (GameObject selection in selections) {
			CascadeRevert (selection);
		}
	}

	private static void CascadeRevert (GameObject selection)
	{
		Revert (selection);
		foreach (Transform t in selection.transform) {
			CascadeRevert (t.gameObject);
		}
	}



	private static void Revert (GameObject[] selections)
	{
		foreach (GameObject selection in selections) {
			Revert (selection);
		}
	}

	private static void Revert (GameObject selection)
	{
		PrefabUtility.RevertPrefabInstance (selection);
	}

}
