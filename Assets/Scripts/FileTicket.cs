using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Used to display informations about files</summary>
public class FileTicket : MonoBehaviour
{
	[Header("References")]
	public TMP_Text nameText;
	public TMP_Text extensionText;
	public TMP_Text flagText;
	public Button selectButton;
	public Button deleteButton;

	public void Init(FileInfo file, Action OnSelect, Action<FileInfo> OnDelete)
	{
		gameObject.SetActive(true);

		nameText.text = file.Name;
		extensionText.text = file.Extension;
		flagText.enabled = file.Extension != ".bmp";

		selectButton.onClick.RemoveAllListeners();
		selectButton.onClick.AddListener(() => OnSelect?.Invoke());

		deleteButton.onClick.RemoveAllListeners();
		deleteButton.onClick.AddListener(() => OnDelete?.Invoke(file));
	}
}