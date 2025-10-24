using System;
using System.Collections.Generic;
using System.IO;
using B83.Image.BMP;
using TMPro;
using UnityEngine;
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
	public Panel settingsPanel;
	public Button settingsSaveButton;
	public TMP_InputField settingsRomNameInput;
	public TMP_InputField settingsRomCodeInput;
	// SETT : Add settings ui

	private enum FileType
	{
		image
	}

	private FileType selectedType;
	private DirectoryInfo project;
	private GeneralSettings settings;
	private BMPLoader bmpLoader;
	private List<FileTicket> tickets;
	private List<string> makefileLines;

	private void Awake()
	{
		tickets = new List<FileTicket>();
		settings = GeneralSettings.Get();
		bmpLoader = new BMPLoader();
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
		playButton.onClick.AddListener(() =>
		{
			// TODO : Show warning about invalid assets (what happens if I try to build with invalid pictures ?)
			// TODO : Add game build and start
		});

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

		imageToggle.onValueChanged.AddListener(value =>
		{
			if (value)
			{
				selectedType = FileType.image;
				RefreshExplorer();
			}
		});

		// TODO : Asset type selectors
		// TODO : Project settings
		// TODO : Preview Image
		// TODO : Preview Sound
		// TODO : Preview code
		// TODO : Open asset with external program
		// TODO : Rename project
		// TODO : Build
		// TODO : Check settings before build
		// TODO : Set ROM names from project name by default
		// TODO : Open category folder

		base.Pop();
		RefreshExplorer();
	}

	private void RefreshExplorer()
	{
		string dir = selectedType switch
		{
			FileType.image => "graphics"
		};

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

			ticket.Init(file, () =>
			{
				detailsNameText.text = file.Name;
				detailsExtensionText.text = file.Extension;
				detailsSizeText.text = file.Length + "o";
				detailsWarning.SetActive(file.Extension != ".bmp");

				imageDetails.SetActive(selectedType == FileType.image);

				switch (selectedType)
				{
					case FileType.image:
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
				}
			});
		}

		tickets.Find(item => item.gameObject.activeSelf).GetComponent<FileTicket>().selectButton.onClick.Invoke();
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
}