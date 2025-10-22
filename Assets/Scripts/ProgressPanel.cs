using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Panel used to display the progress of a task</summary>
public class ProgressPanel : Panel
{
	[Header(nameof(ProgressPanel))]
	public TMP_Text titleText;
	public Slider progressSlider;
	public TMP_Text progressText;

	private GeneralSettings settings;
	private Func<bool> CheckDone;
	private Action OnDone;
	private float progress;
	private bool running;

	public void Pop(string title, Func<bool> checkDone, Action onDone)
	{
		settings = GeneralSettings.Get();

		titleText.text = title;
		CheckDone = checkDone;
		OnDone = onDone;

		progressSlider.value = 0;
		progressText.text = "0%";
		running = true;

		base.Pop();
	}

	void Update()
	{
		if (running)
		{
			float speed = progress >= settings.popupProgressCheck && !CheckDone() ?
				settings.popupProgressSpeedSlow : settings.popupProgressSpeed;
			progress += speed * Time.deltaTime;
			progressSlider.value = progress;
			progressText.text = Mathf.FloorToInt(progress * 100) + "%";

			if (progress >= 1 && CheckDone())
			{
				running = false;
				gameObject.SetActive(false);
				OnDone?.Invoke();
			}
		}
	}
}