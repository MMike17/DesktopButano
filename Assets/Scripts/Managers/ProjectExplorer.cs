using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Used to explore projects and operate on assets</summary>
public class ProjectExplorer : Panel
{
	[Header("References")]
	public TMP_Text projectTitle;
	public Button returnButton;
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

	private enum FileType
	{
		image
	}

	private FileType selectedType;
	private DirectoryInfo project;
	private GeneralSettings settings;
	private List<FileTicket> tickets;

	private void Awake()
	{
		tickets = new List<FileTicket>();
		settings = GeneralSettings.Get();
	}

	public void Pop(DirectoryInfo project, Action OnReturn)
	{
		this.project = project;
		projectTitle.text = project.Name;

		returnButton.onClick.RemoveAllListeners();
		returnButton.onClick.AddListener(() => OnReturn());

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
		// TODO : Delete project
		// TODO : Build
		// TODO : Check settings before build
		// TODO : Set ROM code and name
		// TODO : Set ROM names from project name by default

		base.Pop();
	}

	private void RefreshExplorer()
	{
		string dir = selectedType switch
		{
			FileType.image => "graphics"
		};

		foreach (FileInfo file in new DirectoryInfo(Path.Combine(project.FullName, dir)).GetFiles())
		{
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
						Texture2D texture = new Texture2D(2, 2);
						imagePreviewTexture.enabled = texture.LoadImage(File.ReadAllBytes(file.FullName));

						if (imagePreviewTexture.enabled)
							imagePreviewTexture.texture = texture;

						imageDetailsDimentions.text = texture.width + "x" + texture.height;
						break;
				}
			});
		}
	}
}