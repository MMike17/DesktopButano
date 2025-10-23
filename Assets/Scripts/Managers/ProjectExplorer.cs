using System;
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

	public void Pop(DirectoryInfo project, Action OnReturn)
	{
		projectTitle.text = project.Name;

		returnButton.onClick.RemoveAllListeners();
		returnButton.onClick.AddListener(() => OnReturn());

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
}