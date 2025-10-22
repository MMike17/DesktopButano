using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>Scriptable objects that should have a static accessor in the editor</summary>
public class EditorScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
	static T instance;

	public static T Get()
	{
		if (instance == null)
		{
#if UNITY_EDITOR
			string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);

			if (guids.Length == 0)
				Debug.LogError("Couldn't find instance of " + typeof(T).Name + " in assets. Please create one.");
			else
				instance = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
#endif
		}

		return instance;
	}

	// Copy this to child classes
	// #if UNITY_EDITOR
	// 	[MenuItem("Tools/ Setttings")]
	// 	static void Select() => Selection.activeObject = Get();
	// #endif
}