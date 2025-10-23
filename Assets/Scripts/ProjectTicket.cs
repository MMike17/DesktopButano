using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Used to display informations about Butano projects</summary>
public class ProjectTicket : MonoBehaviour
{
	[Header("References")]
	public TMP_Text nameText;
	public Button openButton;
	public Button deleteButton;

	public void Init(DirectoryInfo dir, Action<DirectoryInfo> OnOpen, Action OnDelete)
	{
		nameText.text = dir.Name;

		openButton.onClick.RemoveAllListeners();
		openButton.onClick.AddListener(() => OnOpen?.Invoke(dir));

		deleteButton.onClick.RemoveAllListeners();
		deleteButton.onClick.AddListener(() => GeneralManager.PopChoice(
			"Are you sure you want to delete the " + dir.Name + " project ?",
			null,
			() => OnDelete?.Invoke()
		));

		gameObject.SetActive(true);
	}
}