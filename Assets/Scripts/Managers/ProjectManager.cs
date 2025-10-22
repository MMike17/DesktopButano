using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Manages projects</summary>
public class ProjectManager : MonoBehaviour
{
	[Header("References")]
	public Panel rootPathPanel;
	public TMP_InputField rootPathInput;
	public Button pickPathButton;
	public Button confirmPathButton;
	[Space]
	public Panel selectorPanel;

	private GeneralSettings _settings;
	private GeneralSettings settings
	{
		get
		{
			if (_settings == null)
				_settings = GeneralSettings.Get();

			return _settings;
		}
	}

	private void Awake()
	{
		rootPathInput.onValueChanged.AddListener(text => confirmPathButton.enabled = !string.IsNullOrWhiteSpace(text));
		pickPathButton.onClick.AddListener(() =>
		{
			SimpleFileBrowser.FileBrowser.ShowLoadDialog(
				paths => rootPathInput.SetTextWithoutNotify(paths[0]),
				null,
				SimpleFileBrowser.FileBrowser.PickMode.Folders,
				false,
				title: "Select path to Butano projects folder"
			);
		});
		confirmPathButton.onClick.AddListener(() => SaveRoot());
	}

	private void SaveRoot()
	{
		PlayerPrefs.SetString(settings.projectRootKey, rootPathInput.text);
		DisplayProjects();
	}

	public bool CheckHasPath()
	{
		return PlayerPrefs.HasKey(settings.projectRootKey) && Directory.Exists(PlayerPrefs.GetString(settings.projectRootKey));
	}

	public void AskForRoot()
	{
		if (PlayerPrefs.HasKey(settings.projectRootKey))
			rootPathInput.text = PlayerPrefs.GetString(settings.projectRootKey);

		rootPathPanel.Pop();
	}

	public void DisplayProjects()
	{
		selectorPanel.Pop();
	}
}