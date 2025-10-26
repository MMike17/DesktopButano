using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Panel used to choose from multiple options</summary>
public class EnumPanel : Panel
{
	[Header(nameof(EnumPanel) + " References")]
	public TMP_Text messageText;
	public TMP_Dropdown choiceDropdown;
	public Button saveButton;
	public Button cancelButton;

	private void Awake()
	{
		overrides = false;
		fullScreen = false;
	}

	public void Pop(string text, List<string> choices, Action<int> OnSelected)
	{
		messageText.text = text;

		choiceDropdown.ClearOptions();
		choiceDropdown.AddOptions(choices);
		choiceDropdown.value = 0;
		choiceDropdown.RefreshShownValue();

		cancelButton.onClick.RemoveAllListeners();
		cancelButton.onClick.AddListener(() => gameObject.SetActive(false));

		saveButton.onClick.RemoveAllListeners();
		saveButton.onClick.AddListener(() =>
		{
			gameObject.SetActive(false);
			OnSelected(choiceDropdown.value);
		});

		base.Pop();
	}
}