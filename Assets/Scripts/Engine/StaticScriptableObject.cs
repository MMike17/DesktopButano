using UnityEngine;

/// <summary>Scriptable objects that should have a static accessor</summary>
public class StaticScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
	static T instance;

	public static T Get()
	{
		if (instance == null)
			instance = Resources.Load<T>(typeof(T).Name);

		return instance;
	}

	// Copy this to child classes
	// #if UNITY_EDITOR
	// 	[MenuItem("Tools/ Setttings")]
	// 	static void Select() => Selection.activeObject = Get();
	// #endif
}