using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Panel used to display custom error messages</summary>
public class ErrorPanel : Panel
{
	[Header(nameof(ErrorPanel) + " References")]
	public TMP_Text messageText;
	public Button closeButton;

	private void Awake()
	{
		overrides = false;
		fullScreen = false;
	}

	public void Pop(string text, Action OnClick)
	{
		messageText.text = text;

		closeButton.onClick.RemoveAllListeners();
		closeButton.onClick.AddListener(() =>
		{
			OnClick?.Invoke();
			gameObject.SetActive(false);
		});

		base.Pop();
	}
}