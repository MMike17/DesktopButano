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
	public float popupLoaderSpeed;

	[Header(nameof(ProjectManager))]
	public string projectRootKey;
	public string projectRootMessage;
	public string projectButanoKey;
	public string projectButanoMessage;
	public string projectCodeKey;
	public string projectCodeMessage;
	public string projectImageKey;
	public string projectImageMessage;
	public string projectSoundKey;
	public string projectSoundMessage;
	public string projectCoresKey;
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
	public string projectRomCodeFlag;
	[Space]
	[TextArea]
	public string projectBuildMessage;
	[TextArea]
	public string projectBuildError;
	[TextArea]
	public string projectPlayError;

	[Header(nameof(ProjectExplorer))]
	public string makefileSeparatorFlag;
	public string makefileRomNameFlag;
	public string makefileRomCodeFlag;
	[Space]
	public FileTicket explorerFilePrefab;
	public string explorerImageFolder;
	public string explorerCodeFolder;
	public string explorerSoundFolder;
	[Space]
	[TextArea]
	public string detailsImageWarning;
	[TextArea]
	public string detailsCodeWarning;
	[Space]
	public int detailsCodeMaxLines;
	[Space]
	[TextArea]
	public string assetImportMessage;
	[TextArea]
	public string assetImportPickMessage;
	[TextArea]
	public string assetImportErrorFormat;
	[TextArea]
	public string imageImportErrorFormat;
	[Space]
	[TextArea]
	public string noToolErrorFormat;
	[TextArea]
	public string imageToolErrorFormat;
	[Space]
	[TextArea]
	public string assetMoveMessageFormat;
	[Space]
	[TextArea]
	public string assetDeleteMessageFormat;
	[TextArea]
	public string assetDeleteErrorFormat;

	[Header(nameof(FileTicket))]
	public float fileClickDelay;
}
