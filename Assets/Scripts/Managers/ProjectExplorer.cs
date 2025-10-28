using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using B83.Win32;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Used to explore projects and operate on assets</summary>
public class ProjectExplorer : Panel
{
	[Header("References")]
	public TMP_Text projectTitle;
	public Button returnButton;
	public Button settingsButton;
	public Button playButton;
	[Space]
	public Toggle imageToggle;
	public Toggle codeToggle;
	public Button openFolderButton;
	[Space]
	public Transform explorerList;
	public AssetDetailsPanel assetDetailsPanel;
	[Space]
	public Panel settingsPanel;
	public Button settingsSaveButton;
	public TMP_InputField settingsRomNameInput;
	public TMP_InputField settingsRomCodeInput;
	// SETT : Add settings ui
	[Space]
	public Panel dragAndDropPanel;

	public enum FileType
	{
		ERROR,
		code,
		image
	}

	private FileType selectedType;
	private DirectoryInfo project;
	private GeneralSettings settings;
	private List<FileTicket> tickets;
	private List<string> makefileLines;
	private bool isWaiting;

	private void Awake()
	{
		tickets = new List<FileTicket>();
		settings = GeneralSettings.Get();

		selectedType = FileType.code;
	}

	private void OnEnable()
	{
		UnityDragAndDropHook.InstallHook();
		UnityDragAndDropHook.OnDroppedFiles += ImportAssets;
	}

	private void OnDisable()
	{
		UnityDragAndDropHook.UninstallHook();
	}

	private void Update()
	{
		if (GeneralManager.HasOverlay())
			return;

		Vector2 viewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		bool isOutOfWindow = viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1;

		if (!isWaiting && !Application.isFocused && isOutOfWindow)
			dragAndDropPanel.Pop();
		else
			dragAndDropPanel.gameObject.SetActive(false);

		if (assetDetailsPanel.selected != null)
		{
			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
				OpenAssetExternal(assetDetailsPanel.selected);

			if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
				DeleteAsset(assetDetailsPanel.selected);
		}
	}

	public void Pop(DirectoryInfo project, Action OnReturn)
	{
		this.project = project;
		projectTitle.text = project.Name;

		makefileLines = new List<string>(File.ReadAllLines(Path.Combine(project.FullName, "Makefile")));

		returnButton.onClick.RemoveAllListeners();
		returnButton.onClick.AddListener(() => OnReturn());

		settingsButton.onClick.RemoveAllListeners();
		settingsButton.onClick.AddListener(() =>
		{
			int lineIndex = GetMakefileLineIndex(new[] { settings.makefileRomNameFlag, settings.makefileSeparatorFlag });
			settingsRomNameInput.text = makefileLines[lineIndex].Split(settings.makefileSeparatorFlag)[1].Split('\n')[0];

			lineIndex = GetMakefileLineIndex(new[] { settings.makefileRomCodeFlag, settings.makefileSeparatorFlag });
			settingsRomCodeInput.text = makefileLines[lineIndex].Split(settings.makefileSeparatorFlag)[1].Split('\n')[0];
			// SETT : Set fields to saved settings

			settingsPanel.Pop();
		});

		playButton.onClick.RemoveAllListeners();
		playButton.onClick.AddListener(() => BuildProject());

		settingsSaveButton.onClick.RemoveAllListeners();
		settingsSaveButton.onClick.AddListener(() =>
		{
			int lineIndex = GetMakefileLineIndex(new[] { settings.makefileRomNameFlag, settings.makefileSeparatorFlag });
			makefileLines[lineIndex] = makefileLines[lineIndex].Split(settings.makefileSeparatorFlag)[0] +
				settings.makefileSeparatorFlag + settingsRomNameInput.text;

			lineIndex = GetMakefileLineIndex(new[] { settings.makefileRomCodeFlag, settings.makefileSeparatorFlag });
			makefileLines[lineIndex] = makefileLines[lineIndex].Split(settings.makefileSeparatorFlag)[0] +
				settings.makefileSeparatorFlag + settingsRomCodeInput.text;
			// SETT : Save settings

			File.WriteAllLines(Path.Combine(project.FullName, "Makefile"), makefileLines);
			settingsPanel.gameObject.SetActive(false);
		});

		imageToggle.onValueChanged.RemoveAllListeners();
		imageToggle.onValueChanged.AddListener(value =>
		{
			if (value)
			{
				selectedType = FileType.image;
				imageToggle.transform.SetAsLastSibling();
				RefreshExplorer();
			}
		});

		codeToggle.onValueChanged.RemoveAllListeners();
		codeToggle.onValueChanged.AddListener(value =>
		{
			if (value)
			{
				selectedType = FileType.code;
				codeToggle.transform.SetAsLastSibling();
				RefreshExplorer();
			}
		});

		openFolderButton.onClick.RemoveAllListeners();
		openFolderButton.onClick.AddListener(() =>
		{
			string targetFolder = selectedType switch
			{
				FileType.image => settings.explorerImageFolder,
				FileType.code => settings.explorerCodeFolder
			};

			Process.Start("explorer.exe", Path.Combine(project.FullName, targetFolder).Replace("/", "\\"));
		});

		// TODO : Asset type selectors
		// TODO : Rename project
		// TODO : Check assets before build

		base.Pop();
		RefreshExplorer();
	}

	private void RefreshExplorer()
	{
		tickets.ForEach(ticket => ticket.gameObject.SetActive(false));

		foreach (FileInfo file in new DirectoryInfo(Path.Combine(project.FullName, GetDirFromType(selectedType))).GetFiles())
		{
			if (file.Name == ".gitignore")
				continue;

			FileTicket ticket = tickets.Find(item => !item.gameObject.activeSelf);

			if (ticket == null)
			{
				ticket = Instantiate(settings.explorerFilePrefab, explorerList);
				tickets.Add(ticket);
			}

			ticket.Init(
				file,
				selectedType,
				valid => assetDetailsPanel.ShowDetails(file, selectedType, valid),
				() => OpenAssetExternal(file),
				() =>
				{
					List<string> choices = new List<string>(Enum.GetNames(typeof(FileType)));
					choices.Remove(selectedType.ToString());

					GeneralManager.PopEnum(
						string.Format(settings.assetMoveMessageFormat, file.Name),
						choices,
						index => MoveAsset(file, (FileType)Enum.Parse(typeof(FileType), choices[index]))
					);
				},
				DeleteAsset
			);
		}

		FileTicket firstTicket = tickets.Find(item => item.gameObject.activeSelf);

		if (firstTicket != null)
		{
			EventSystem.current.SetSelectedGameObject(firstTicket.selectButton.gameObject);
			firstTicket.selectButton.onClick.Invoke();
		}
	}

	private string GetDirFromType(FileType type)
	{
		return type switch
		{
			FileType.image => settings.explorerImageFolder,
			FileType.code => settings.explorerCodeFolder
		};
	}

	private int GetMakefileLineIndex(string[] tags)
	{
		string line = makefileLines.Find(line =>
		{
			foreach (string tag in tags)
			{
				if (!line.Contains(tag))
					return false;
			}

			return true;
		});

		return makefileLines.IndexOf(line);
	}

	private void ImportAssets(List<string> assetPaths, POINT point) => StartCoroutine(ImportRoutine(assetPaths));

	private IEnumerator ImportRoutine(List<string> assetPaths)
	{
		if (isWaiting)
			yield break;

		PopProgress();

		foreach (string assetPath in assetPaths)
		{
			FileInfo file = new FileInfo(assetPath);

			if (file.Exists)
			{
				FileType detectedType = GetFileType(file);
				string targetFolder = "";

				if (detectedType == FileType.ERROR)
				{
					bool blocked = true;
					List<string> choices = new List<string>(Enum.GetNames(typeof(FileType)));
					isWaiting = false;

					GeneralManager.PopEnum(
						settings.assetImportPickMessage,
						choices,
						index =>
						{
							targetFolder = GetDirFromType((FileType)Enum.Parse(typeof(FileType), choices[index]));
							blocked = false;
						}
					);

					yield return new WaitUntil(() => !blocked);
					PopProgress();
				}
				else
				{
					switch (detectedType)
					{
						case FileType.image:
							// check if image is 16 or 256 colors mode
							using (FileStream stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
							{
								stream.Seek(28, SeekOrigin.Begin); // 0x1C offset
								byte[] data = new byte[2];
								stream.Read(data, 0, data.Length);
								int bitsPerPixel = data[0] | (data[1] << 8); // bit shift to number

								if (bitsPerPixel != 4 && bitsPerPixel != 8)
								{
									isWaiting = false;
									bool blocked = true;
									GeneralManager.PopError(
										string.Format(settings.imageImportErrorFormat, file.Name, bitsPerPixel),
										() => blocked = false
									);
									yield return new WaitUntil(() => !blocked);
									PopProgress();
									continue;
								}
							}
							break;
					}

					targetFolder = GetDirFromType(detectedType);
				}

				File.Move(file.FullName, Path.Combine(project.FullName, targetFolder, file.Name));
				// TODO : Generate the proper json files for that asset
			}
			else
			{
				bool blocked = true;
				isWaiting = false;
				GeneralManager.PopError(string.Format(settings.assetImportErrorFormat, file.Name), () => blocked = false);
				yield return new WaitUntil(() => !blocked);
				PopProgress();
			}
		}

		isWaiting = false;
		RefreshExplorer();

		void PopProgress()
		{
			isWaiting = true;
			GeneralManager.PopProgress(settings.assetImportMessage, () => isWaiting = false, null);
		}
	}

	private FileType GetFileType(FileInfo file)
	{
		return file.Extension switch
		{
			".bmp" => FileType.image,
			".cpp" => FileType.code,
			_ => FileType.ERROR
			// TODO : Add support for sound files here
		};
	}

	private void MoveAsset(FileInfo file, FileType target)
	{
		File.Move(file.FullName, Path.Combine(project.FullName, GetDirFromType(target), file.Name));
		// TODO : Add json asset def change as if it was a new import
		RefreshExplorer();
	}

	private void OpenAssetExternal(FileInfo file)
	{
		string toolPath = selectedType switch
		{
			FileType.image => PlayerPrefs.GetString(settings.projectImageKey, ""),
			FileType.code => PlayerPrefs.GetString(settings.projectCodeKey, ""),
			// TODO : Add support for sound assets
		};

		if (string.IsNullOrWhiteSpace(toolPath))
		{
			GeneralManager.PopError(string.Format(settings.noToolErrorFormat, selectedType));
			return;
		}

		Process toolProcess = CreateProcess(toolPath, file.FullName, false);

		if (!toolProcess.Start())
		{
			string errors = toolProcess.StandardError.ReadToEnd();

			if (string.IsNullOrWhiteSpace(errors))
				GeneralManager.PopError(string.Format(settings.imageToolErrorFormat, file.Name, errors.Split('\n')[0]));
		}
	}

	private void DeleteAsset(FileInfo file)
	{
		GeneralManager.PopChoice(
			string.Format(settings.assetDeleteMessageFormat, file.Name),
			null,
			() =>
			{
				IOHelper.DeleteFile(file, settings.assetDeleteErrorFormat);
				RefreshExplorer();
			}
		);
	}

	private void BuildProject()
	{
		Process buildProcess = CreateProcess(
			"cmd.exe",
			"/c make -j" + PlayerPrefs.GetInt(settings.projectCoresKey, SystemInfo.processorCount),
			true
		);

		isWaiting = true;
		buildProcess.Exited += (s, e) => isWaiting = false;

		int lineIndex = GetMakefileLineIndex(new[] { settings.makefileRomNameFlag, settings.makefileSeparatorFlag });
		string projectName = makefileLines[lineIndex].Split(settings.makefileSeparatorFlag)[1].Split('\n')[0];
		GeneralManager.PopProgress(
			string.Format(settings.projectBuildMessage, projectName),
			() => !isWaiting,
			() => OnBuildDone(buildProcess)
		);

		buildProcess.Start();
	}

	private void OnBuildDone(Process buildProcess)
	{
		string errors = buildProcess.StandardError.ReadToEnd();

		if (string.IsNullOrWhiteSpace(errors))
			StartGame();
		else
			GeneralManager.PopError(settings.projectBuildError + "\n" + errors.Split('\n')[0]);
	}

	private void StartGame()
	{
		Process playProcess = CreateProcess(
			"C:\\Program Files\\mGBA\\mGBA.exe",
			Path.Combine(project.FullName, project.Name + ".gba"),
			true
		);

		playProcess.Exited += (s, e) => isWaiting = false;
		StartCoroutine(WaitForError(playProcess));
	}

	private IEnumerator WaitForError(Process playProcess)
	{
		isWaiting = true;
		playProcess.Start();

		yield return new WaitUntil(() => !isWaiting);
		string errors = playProcess.StandardError.ReadToEnd();

		if (!string.IsNullOrEmpty(errors))
			GeneralManager.PopError(settings.projectPlayError + "\n" + errors.Split('\n')[0]);
	}

	private Process CreateProcess(string application, string arguments, bool enableEvents)
	{
		return new Process()
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = application,
				Arguments = arguments,
				UseShellExecute = false,
				RedirectStandardError = true,
				CreateNoWindow = true,
				ErrorDialog = true
			},
			EnableRaisingEvents = enableEvents
		};
	}
}