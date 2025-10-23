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
	public GameObject detailsWarningMessage;

	// TODO : What are the details ?
	// name
	// preview
	// extension
	// size
	// warning panel

	// 	image
	// dimensions

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

		// TODO : What do we need here ?

		// TODO : Asset type selectors
		// TODO : Project settings
		// TODO : Preview
		// TODO : Open asset with external program
		// TODO : Rename project
		// TODO : Delete project
		// TODO : Build
		// TODO : Check settings before build
		// TODO : Select how many cores to build
		// TODO : Set ROM code and name
		// TODO : Set ROM names from project name by default

		base.Pop();
	}

	private void RefreshExplorer()
	{
		// TODO : Display files depending on selected type

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
				detailsWarningMessage.SetActive(file.Extension != ".bmp");
			});
		}
	}
}