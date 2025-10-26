using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using B83.Image.BMP;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Debug = UnityEngine.Debug;

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
	[Space]
	public TMP_Text detailsNameText;
	public TMP_Text detailsExtensionText;
	public TMP_Text detailsSizeText;
	public GameObject detailsWarning;
	public TMP_Text detailsWarningText;
	[Space]
	public GameObject imageDetails;
	public GameObject imagePreview;
	public TMP_Text imageDetailsDimentions;
	public RawImage imagePreviewTexture;
	[Space]
	public GameObject codeDetails;
	public GameObject codePreview;
	public TMP_Text codeDetailsLines;
	public TMP_Text codePreviewText;
	[Space]
	public Panel settingsPanel;
	public Button settingsSaveButton;
	public TMP_InputField settingsRomNameInput;
	public TMP_InputField settingsRomCodeInput;
	// SETT : Add settings ui

	public enum FileType
	{
		code,
		image
	}

	private FileType selectedType;
	private DirectoryInfo project;
	private GeneralSettings settings;
	private BMPLoader bmpLoader;
	private List<FileTicket> tickets;
	private List<string> makefileLines;
	private FileInfo selected;
	private bool isWaiting;

	private void Awake()
	{
		tickets = new List<FileTicket>();
		settings = GeneralSettings.Get();
		bmpLoader = new BMPLoader();
	}

	private void Update()
	{
		if (GeneralManager.HasOverlay())
			return;

		if (selected != null && Input.GetKeyDown(KeyCode.Delete))
			DeleteAsset(selected);
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
			Process.Start("explorer.exe", Path.Combine(project.FullName, settings.explorerImageFolder).Replace("/", "\\"));
		});

		// TODO : Asset type selectors
		// TODO : Preview Sound
		// TODO : Open asset with external program
		// TODO : Rename project
		// TODO : Check settings before build

		base.Pop();
		RefreshExplorer();
	}

	private void RefreshExplorer()
	{
		string dir = selectedType switch
		{
			FileType.image => settings.explorerImageFolder,
			FileType.code => settings.explorerCodeFolder
		};

		tickets.ForEach(ticket => ticket.gameObject.SetActive(false));

		foreach (FileInfo file in new DirectoryInfo(Path.Combine(project.FullName, dir)).GetFiles())
		{
			if (file.Name == ".gitignore")
				continue;

			FileTicket ticket = tickets.Find(item => !item.gameObject.activeSelf);

			if (ticket == null)
			{
				ticket = Instantiate(settings.explorerFilePrefab, explorerList);
				tickets.Add(ticket);
			}

			ticket.Init(file, selectedType, valid =>
			{
				selected = file;

				detailsNameText.text = file.Name.Replace(file.Extension, "");
				detailsExtensionText.text = file.Extension;
				detailsSizeText.text = file.Length + "o";

				imageDetails.SetActive(selectedType == FileType.image);
				imagePreview.SetActive(selectedType == FileType.image);

				codeDetails.SetActive(selectedType == FileType.code);
				codePreview.SetActive(selectedType == FileType.code);

				switch (selectedType)
				{
					case FileType.image:
						detailsWarning.SetActive(valid);
						detailsWarningText.text = settings.detailsImageWarning;
						Texture2D texture = null;

						try
						{
							texture = bmpLoader.LoadBMP(File.ReadAllBytes(file.FullName)).ToTexture2D();
						}
						catch (Exception ex)
						{
							Debug.Log(ex);
						}

						imagePreviewTexture.enabled = texture != null;

						if (imagePreviewTexture.enabled)
							imagePreviewTexture.texture = texture;

						imageDetailsDimentions.text = texture.width + "x" + texture.height;
						break;

					case FileType.code:
						detailsWarning.SetActive(valid);
						detailsWarningText.text = settings.detailsCodeWarning;

						string content = string.Empty;
						string[] lines = File.ReadAllLines(file.FullName);

						for (int i = 0; i < Mathf.Min(lines.Length, settings.detailsCodeMaxLines); i++)
							content += lines[i] + "\n";

						content.TrimEnd('\n');
						codePreviewText.text = content;
						codeDetailsLines.text = lines.Length.ToString();
						break;
				}
			}, DeleteAsset);
		}

		FileTicket firstTicket = tickets.Find(item => item.gameObject.activeSelf);

		if (firstTicket != null)
		{
			EventSystem.current.SetSelectedGameObject(firstTicket.selectButton.gameObject);
			firstTicket.selectButton.onClick.Invoke();
		}
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
		Process buildProcess = new Process()
		{
			StartInfo = new ProcessStartInfo()
			{
				FileName = "cmd.exe",
				Arguments = "/c make -j" + PlayerPrefs.GetInt(settings.projectCoresKey, SystemInfo.processorCount),
				UseShellExecute = false,
				RedirectStandardError = true,
				CreateNoWindow = true,
				ErrorDialog = true,
				WorkingDirectory = project.FullName
			},
			EnableRaisingEvents = true
		};

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
		Process playProcess = new Process()
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "C:\\Program Files\\mGBA\\mGBA.exe",
				Arguments = Path.Combine(project.FullName, project.Name + ".gba"),
				UseShellExecute = false,
				RedirectStandardError = true,
				CreateNoWindow = true,
				ErrorDialog = true
			},
			EnableRaisingEvents = true
		};

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
}