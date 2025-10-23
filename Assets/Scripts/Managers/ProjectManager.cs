using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
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
	public Button settingsButton;
	public TMP_Text versionText;
	public SkinGraphic versionSkin;
	public Transform projectList;
	public Button createProjectButton;
	[Space]
	public Panel createProjectPanel;
	public TMP_InputField createNameInput;
	public Button createButton;
	[Space]
	public Panel settingsPanel;
	public Button projectPathButton;
	public TMP_Text projectPathText;
	public Button butanoPathButton;
	public TMP_Text butanoPathText;
	public Button settingsCloseButton;

	[DllImport("shell32.dll", CharSet = CharSet.Auto)]
	private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOP);

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	private struct SHFILEOPSTRUCT
	{
		public IntPtr hwnd;
		public FileOperationType wFunc;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string pFrom;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string pTo;
		public FileOperationFlags fFlags;
		public bool fAnyOperationsAborted;
		public IntPtr hNameMappings;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string lpszProgressTitle;
	}

	[Flags]
	private enum FileOperationFlags : ushort
	{
		FOF_ALLOWUNDO = 0x0040, // move to recycle
		FOF_NOCONFIRMATION = 0x0010, // no confirmation dialogue
		FOF_SILENT = 0x0004, // no progression dialogue
		FOF_NOERRORUI = 0x0400 // no error UI
	}

	private enum FileOperationType : uint
	{
		FO_MOVE = 0x0001,
		FO_COPY = 0x0002,
		FO_DELETE = 0x0003,
		FO_RENAME = 0x0004,
	}

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

		settingsButton.onClick.AddListener(() =>
		{
			// TODO : Set fields to saved settings
			settingsPanel.Pop();
		});

		projectPathButton.onClick.AddListener(() => AskForRoot());
		butanoPathButton.onClick.AddListener(() => AskForButano());

		createProjectButton.onClick.AddListener(() =>
		{
			createNameInput.text = "";
			createProjectPanel.Pop();
			CheckPaths();
		});

		createNameInput.onValueChanged.AddListener(value => createButton.interactable = !string.IsNullOrWhiteSpace(value));
		createButton.onClick.AddListener(() => CreateProject(createNameInput.text));

		settingsCloseButton.onClick.AddListener(() =>
		{
			// TODO : Save settings here
			selectorPanel.Pop();
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

		if (!Directory.Exists(path) || !new DirectoryInfo(path).Name.Contains("butano-"))
			return false;

		foreach (DirectoryInfo dir in new DirectoryInfo(path).GetDirectories())
		{
			if (dir.Name == "butano")
				return true;
		}

		return false;
	}

	private void AskForButano()
	{
		if (PlayerPrefs.HasKey(settings.projectButanoKey))
			butanoPathInput.text = PlayerPrefs.GetString(settings.projectButanoKey);

		butanoPathPanel.Pop();
	}

	private void SaveButano()
	{
		if (!Directory.Exists(butanoPathInput.text))
		{
			GeneralManager.PopError("The provided path is invalid");
			butanoPathInput.text = "";
			return;
		}

		if (new DirectoryInfo(butanoPathInput.text).Name.Contains("butano-"))
			PlayerPrefs.SetString(settings.projectButanoKey, butanoPathInput.text);
		else
			DownloadButano();

		CheckPaths();
	}

	private void DownloadButano()
	{
		string tempPath = Path.Combine(Application.temporaryCachePath, "butanoInstall");

		GetButanoLatestVersion(version =>
		{
			string finalPath = PlayerPrefs.GetString(settings.projectButanoKey);

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
					ZipFile.ExtractToDirectory(tempPath, finalPath);
					PlayerPrefs.SetString(settings.projectButanoKey, Path.Combine(finalPath, "butano-" + version));
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

	private void DeleteProject(DirectoryInfo dir)
	{
		SHFILEOPSTRUCT shf = new SHFILEOPSTRUCT
		{
			wFunc = FileOperationType.FO_DELETE,
			pFrom = dir.FullName + '\0' + '\0', // double null is required
			fFlags = FileOperationFlags.FOF_ALLOWUNDO | FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_SILENT,
			hwnd = IntPtr.Zero,
			pTo = null,
			fAnyOperationsAborted = false,
			hNameMappings = IntPtr.Zero,
			lpszProgressTitle = null
		};

		int result = SHFileOperation(ref shf);

		if (result != 0)
			GeneralManager.PopError("Couldn't delete project " + dir.Name + "\nError : " + result);
	}

	private void CreateProject(string name)
	{
		createProjectPanel.gameObject.SetActive(false);
		string targetPath = Path.Combine(PlayerPrefs.GetString(settings.projectRootKey), name);

		SHFILEOPSTRUCT shf = new SHFILEOPSTRUCT
		{
			wFunc = FileOperationType.FO_COPY,
			pFrom = Path.Combine(PlayerPrefs.GetString(settings.projectButanoKey), "template") + '\0' + '\0',
			pTo = targetPath + '\0' + '\0',
			fFlags = FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_SILENT | FileOperationFlags.FOF_NOERRORUI,
			hwnd = IntPtr.Zero,
			fAnyOperationsAborted = false,
			hNameMappings = IntPtr.Zero,
			lpszProgressTitle = null,
		};

		int result = SHFileOperation(ref shf);

		if (result == 0)
		{
			List<FileInfo> files = new List<FileInfo>(new DirectoryInfo(targetPath).GetFiles());
			FileInfo makefile = files.Find(file => file.Name == "Makefile");
			File.WriteAllText(makefile.FullName, File.ReadAllText(makefile.FullName).Replace("ROM TITLE", name));

			CheckPaths();
		}
		else
			GeneralManager.PopError("Couldn't create project " + name + "\nError : " + result);
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
			AskForButano();
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
					CheckPaths();
				});
			}
		});

		selectorPanel.Pop();
		projectPathText.text = PlayerPrefs.GetString(settings.projectRootKey);
		butanoPathText.text = PlayerPrefs.GetString(settings.projectButanoKey);

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
					GeneralManager.OpenProject,
					() => GeneralManager.PopChoice(
						"Are you sure you want to delete the " + dir.Name + " project ?",
						null,
						() =>
						{
							DeleteProject(dir);
							CheckPaths();
						}
					)
				);
			}
		}
	}
}