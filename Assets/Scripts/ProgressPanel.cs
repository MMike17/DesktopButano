using System;
using TMPro;
using UnityEngine;

/// <summary>Panel used to display the progress of a task</summary>
public class ProgressPanel : Panel
{
	[Header(nameof(ProgressPanel))]
	public TMP_Text titleText;
	public Transform spinner;

	private GeneralSettings settings;
	private Func<bool> CheckDone;
	private Action OnDone;
	private bool running;

	public void Pop(string title, Func<bool> checkDone, Action onDone)
	{
		settings = GeneralSettings.Get();

		titleText.text = title;
		CheckDone = checkDone;
		OnDone = onDone;
		running = true;

		base.Pop();
	}

	void Update()
	{
		if (running)
		{
			spinner.Rotate(0, 0, -settings.popupLoaderSpeed * Time.deltaTime);

			if (CheckDone())
			{
				running = false;
				gameObject.SetActive(false);
				OnDone?.Invoke();
			}
		}
	}
}