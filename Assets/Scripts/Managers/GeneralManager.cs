using System;
using UnityEngine;

/// <summary>Manages the general flow of windows</summary>
public class GeneralManager : MonoBehaviour
{
	private static GeneralManager instance;

	// TODO : Setup window selector

	[Header("References")]
	public ProjectManager projectsPanel;
	[Space]
	public ErrorPanel errorPanel;
	public ProgressPanel progressPanel;

	private GeneralSettings settings;

	private void Awake()
	{
		instance = this;
		settings = GeneralSettings.Get();

		foreach (Panel panel in FindObjectsByType<Panel>(FindObjectsSortMode.None))
			panel.gameObject.SetActive(false);

		projectsPanel.CheckPaths();

		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			PopError("Can't connect to the internet. This application will now close.", () => Application.Quit());
		}
	}

	public static void PopError(string message, Action onClick = null) => instance.errorPanel.Pop(message, onClick);

	public static void PopProgress(string title, Func<bool> checkDone, Action onDone)
	{
		instance.progressPanel.Pop(title, checkDone, onDone);
	}
}