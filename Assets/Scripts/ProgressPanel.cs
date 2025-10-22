using System;
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

	private Func<float> GetProgress;
	private Action OnDone;
	private bool running;

	public void Pop(string title, Func<float> getProgress, Action onDone)
	{
		titleText.text = title;
		GetProgress = getProgress;
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
			float progress = GetProgress();
			Debug.Log(progress);
			progressSlider.value = progress;
			progressText.text = Mathf.FloorToInt(progress * 100) + "%";

			if (progress >= 1)
			{
				running = false;
				gameObject.SetActive(false);
				OnDone?.Invoke();
			}
		}
	}
}