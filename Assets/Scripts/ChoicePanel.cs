using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Panel used to display a binary choice</summary>
public class ChoicePanel : Panel
{
	[Header("References")]
	public TMP_Text messageText;
	public Button noButton;
	public Button yesButton;

	public void Pop(string message, Action OnNo, Action OnYes)
	{
		GeneralSettings settings = GeneralSettings.Get();
		messageText.text = message;

		yesButton.onClick.RemoveAllListeners();
		yesButton.onClick.AddListener(() =>
		{
			gameObject.SetActive(false);
			OnYes?.Invoke();
		});

		noButton.onClick.RemoveAllListeners();
		noButton.onClick.AddListener(() =>
		{
			gameObject.SetActive(false);
			OnNo?.Invoke();
		});

		base.Pop();
	}
}