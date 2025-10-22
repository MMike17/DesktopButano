using UnityEngine;
using UnityEngine.UI;

/// <summary>Manages the general flow of windows</summary>
public class GeneralManager : MonoBehaviour
{
	// TODO : Setup window selector

	[Header("References")]
	public ProjectManager projectsPanel;

	private GeneralSettings settings;

	private void Awake()
	{
		settings = GeneralSettings.Get();

		if (projectsPanel.CheckHasPath())
			projectsPanel.DisplayProjects();
		else
			projectsPanel.AskForRoot();
	}
}