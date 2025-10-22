using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>Settings for general use</summary>
[CreateAssetMenu(fileName = nameof(GeneralSettings), menuName = "Data/" + nameof(GeneralSettings))]
public class GeneralSettings : StaticScriptableObject<GeneralSettings>
{
#if UNITY_EDITOR
	[MenuItem("Tools/" + nameof(GeneralSettings))]
	static void Select() => Selection.activeObject = Get();
#endif

	[Header(nameof(ProjectManager))]
	public string projectRootKey;
	public string projectButanoKey;
	[Space]
	[TextArea]
	public string projectButanoURLVersion;
	[TextArea]
	public string projectButanoURLDownload;
}
