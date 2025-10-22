using UnityEngine;
using UnityEngine.UI;

/// <summary>Manages the general flow of windows</summary>
public class GeneralManager : MonoBehaviour
{
	private static GeneralManager instance;

	// TODO : Setup window selector

	[Header("References")]
	public ProjectManager projectsPanel;
	public ErrorPanel errorPanel;

	private GeneralSettings settings;

	private void Awake()
	{
		instance = this;
		settings = GeneralSettings.Get();

		if (projectsPanel.CheckHasRootPath())
			projectsPanel.DisplayProjects();
		else
			projectsPanel.AskForRoot();
	}

	public static void PopError(string message) => instance.errorPanel.Pop(message);
}