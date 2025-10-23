using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
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
	public SkinGraphic versionSkin;
	public Button projectPathButton;
	public TMP_Text projectPathText;
	public Transform projectList;

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
				title: settings.projectRootMessage
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
				title: settings.projectButanoMessage
			);
		});
		butanoConfirmButton.onClick.AddListener(() => SaveButano());

		projectPathButton.onClick.AddListener(() =>
		{
			rootPathInput.text = PlayerPrefs.GetString(settings.projectRootKey);
			AskForRoot();
		});
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

		foreach (DirectoryInfo dir in new DirectoryInfo(path).GetDirectories())
		{
			if (dir.Name.Contains("butano"))
				return true;
		}

		return false;
	}

	private void SaveButano()
	{
		if (!Directory.Exists(butanoPathInput.text))
		{
			GeneralManager.PopError("The provided path is invalid");
			butanoPathInput.text = "";
			return;
		}

		PlayerPrefs.SetString(settings.projectButanoKey, butanoPathInput.text);
		bool hasInstall = false;

		foreach (DirectoryInfo dir in new DirectoryInfo(butanoPathInput.text).GetDirectories())
		{
			if (dir.Name.Contains("butano"))
				hasInstall = true;
		}

		if (!hasInstall)
			DownloadButano();

		CheckPaths();
	}

	private void DownloadButano()
	{
		string tempPath = Path.Combine(Application.temporaryCachePath, "butanoInstall");

		GetButanoLatestVersion(version =>
		{
			UnityWebRequest request = UnityWebRequest.Get(settings.projectButanoURLDownload + version + ".zip");
			request.downloadHandler = new DownloadHandlerFile(tempPath);
			GeneralManager.PopProgress(
				"Installing Butano " + version,
				() => (request.uploadProgress + request.downloadProgress) / 2 >= 1,
				CheckPaths
			);

			request.SendWebRequest().completed += op =>
			{
				if (request.result != UnityWebRequest.Result.Success)
				{
					GeneralManager.PopError(
						"Couldn't retrieve butano version\n" + request.error + "\n(" + request.url + ")"
					);
				}
				else
				{
					ZipFile.ExtractToDirectory(tempPath, PlayerPrefs.GetString(settings.projectButanoKey));
					CheckPaths();
				}
			};
		});
	}

	private string GetButanoLocalVersion()
	{
		return new DirectoryInfo(PlayerPrefs.GetString(settings.projectButanoKey)).Name.Replace("butano-", "");
	}

	private void GetButanoLatestVersion(Action<string> OnDone)
	{
		UnityWebRequest request = UnityWebRequest.Get(settings.projectButanoURLVersion);
		request.SendWebRequest().completed += op => OnDone?.Invoke(request.url.Split('/')[^1]);
	}

	private bool IsDirButanoProject(DirectoryInfo dir)
	{
		return new List<FileInfo>(dir.GetFiles()).Find(item => item.Name == "Makefile") != null &&
			dir.GetDirectories("src").Length > 0;
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

		GetButanoLatestVersion(latest =>
		{
			versionText.text = latest;
			string local = GetButanoLocalVersion();

			versionSkin.skinTag = local == latest ? settings.projectVersionOkTag : settings.projectVersionNoTag;
			versionSkin.Skin();

			if (local != latest)
			{
				GeneralManager.PopChoice(string.Format(settings.projectVersionFormat, local, latest), null, () =>
				{
					string butanoPath = PlayerPrefs.GetString(settings.projectButanoKey);

					foreach (DirectoryInfo dir in new DirectoryInfo(butanoPath).GetDirectories())
					{
						if (dir.Name.Contains("butano"))
							Directory.Delete(dir.FullName, true);
					}

					DownloadButano();
				});
			}
		});

		selectorPanel.Pop();
		projectPathText.text = PlayerPrefs.GetString(settings.projectRootKey);

		foreach (Transform ticket in projectList)
			ticket.gameObject.SetActive(false);

		foreach (DirectoryInfo dir in new DirectoryInfo(projectPathText.text).GetDirectories())
		{
			if (IsDirButanoProject(dir))
			{
				ProjectTicket selected = null;

				foreach (Transform ticket in projectList)
				{
					if (!ticket.gameObject.activeSelf)
					{
						selected = ticket.GetComponent<ProjectTicket>();
						break;
					}
				}

				if (selected == null)
					selected = Instantiate(settings.projectTicketPrefab, projectList);

				selected.Init(
					dir,
					null, // TODO : Add method to open project
					() => GeneralManager.PopChoice(
						"Are you sure you want to delete the " + dir.Name + " project ?",
						null,
						() =>
						{
							Type shellType = Type.GetTypeFromProgID("Shell.Application", true);
							dynamic shellApp = Activator.CreateInstance(shellType);
							var recycleBin = shellApp.Namespace(0xa);
							recycleBin.MoveHere(dir.FullName);
						}
					)
				);
			}
		}
	}
}