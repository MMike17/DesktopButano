using System.IO;
using TMPro;
using UnityEngine;

/// <summary>Used to explore projects and operate on assets</summary>
public class ProjectExplorer : Panel
{
	[Header("References")]
	public TMP_Text projectTitle;

	public void Pop(DirectoryInfo project)
	{
		projectTitle.text = project.Name;

		// TODO : What do we need here ?

		base.Pop();
	}
}