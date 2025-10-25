using UnityEngine;

using static Skinning;

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

	[Header(nameof(GeneralManager))]
	public SkinData skin;
	[Space]
	public float popupProgressSpeed;
	public float popupProgressSpeedSlow;
	[Range(0, 1)]
	public float popupProgressCheck;

	[Header(nameof(ProjectManager))]
	public string projectRootKey;
	public string projectRootMessage;
	public string projectButanoKey;
	public string projectButanoMessage;
	[Space]
	[TextArea]
	public string projectButanoURLVersion;
	[TextArea]
	public string projectButanoURLDownload;
	[Space]
	[TextArea]
	public string projectVersionFormat;
	[Space]
	public SkinTag projectVersionOkTag;
	public SkinTag projectVersionNoTag;
	[Space]
	public ProjectTicket projectTicketPrefab;
	[TextArea]
	public string projectDeleteErrorFormat;
	[TextArea]
	public string projectCreateErrorFormat;
	[Space]
	public string projectCustomDirName;
	public string projectRomNameFlag;

	[Header(nameof(ProjectExplorer))]
	public string makefileSeparatorFlag;
	public string makefileRomNameFlag;
	public string makefileRomCodeFlag;
	[Space]
	public FileTicket explorerFilePrefab;
	public string explorerImageFolder;
	[Space]
	[TextArea]
	public string detailsImageWarning;
}
