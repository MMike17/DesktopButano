using UnityEditor;
using UnityEngine;

public class TestEditor : EditorWindow
{
	string test = "";

	[MenuItem("DesktopButano/Test")]
	private static void ShowWindow()
	{
		var window = GetWindow<TestEditor>();
		window.titleContent = new GUIContent("Test");
		window.Show();
	}

	private void OnGUI()
	{
		PaletteMeta data = new PaletteMeta();

		if (GUILayout.Button("Test"))
		{
			test = JsonUtility.ToJson(data, true);
		}

		EditorGUILayout.TextArea(test);
	}
}