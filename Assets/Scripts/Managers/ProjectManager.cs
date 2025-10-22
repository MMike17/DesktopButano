using System.IO;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Manages projects</summary>
public class ProjectManager : MonoBehaviour
{
	[Header("References")]
	public Panel rootPathPanel;
	public TMP_InputField rootPathInput;
	public Button rootPickButton;
	public Button rootConfirmButton;
	[Space]
	public Panel butanoPathPanel;
	public TMP_InputField butanoPathInput;
	public Button butanoPickButton;
	public Button butanoConfirmButton;
	[Space]
	public Panel selectorPanel;
	public TMP_Text versionText;

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
		rootPickButton.onClick.AddListener(() =>
		{
			FileBrowser.ShowLoadDialog(
				paths => rootPathInput.SetTextWithoutNotify(paths[0]),
				null,
				FileBrowser.PickMode.Folders,
				false,
				title: "Select path to Butano projects folder"
			);
		});
		rootConfirmButton.onClick.AddListener(() => SaveRoot());

		butanoPickButton.onClick.AddListener(() =>
		{
			FileBrowser.ShowLoadDialog(
				paths => butanoPathInput.SetTextWithoutNotify(paths[0]),
				null,
				FileBrowser.PickMode.Folders,
				false,
				title: "Select path to/for Butano installation"
			);
		});
		butanoConfirmButton.onClick.AddListener(() => SaveButano());
	}

	private void AskForRoot()
	{
		if (PlayerPrefs.HasKey(settings.projectRootKey))
			rootPathInput.text = PlayerPrefs.GetString(settings.projectRootKey);

		rootPathPanel.Pop();
	}

	private void SaveRoot()
	{
		if (!Directory.Exists(rootPathInput.text))
		{
			GeneralManager.PopError("The provided path is invalid");
			rootPathInput.text = "";
			return;
		}

		PlayerPrefs.SetString(settings.projectRootKey, rootPathInput.text);
		CheckPaths();
	}

	private bool CheckHasButanoPath()
	{
		if (!PlayerPrefs.HasKey(settings.projectButanoKey))
			return false;

		string path = PlayerPrefs.GetString(settings.projectButanoKey);

		if (!Directory.Exists(path))
			return false;

		return new DirectoryInfo(path).GetDirectories("butano").Length > 0;
	}

	private void SaveButano()
	{
		if (!Directory.Exists(butanoPathInput.text))
		{
			GeneralManager.PopError("The provided path is invalid");
			butanoPathInput.text = "";
			return;
		}

		if (new DirectoryInfo(butanoPathInput.text).GetDirectories("butano").Length == 0)
		{
			// install
			// TODO : Download butano from github and place it here
		}

		PlayerPrefs.SetString(settings.projectRootKey, rootPathInput.text);
		CheckPaths();
	}

	public void CheckPaths()
	{
		if (!PlayerPrefs.HasKey(settings.projectRootKey) || !Directory.Exists(PlayerPrefs.GetString(settings.projectRootKey)))
		{
			AskForRoot();
			return;
		}

		if (!CheckHasButanoPath())
		{
			butanoPathPanel.Pop();
			return;
		}

		selectorPanel.Pop();
	}
}