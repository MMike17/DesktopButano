using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static ProjectExplorer;

/// <summary>Used to display informations about files</summary>
public class FileTicket : MonoBehaviour
{
	[Header("References")]
	public TMP_Text nameText;
	public TMP_Text extensionText;
	public TMP_Text flagText;
	public Button selectButton;
	public Button deleteButton;

	public void Init(FileInfo file, FileType type, Action<bool> OnSelect, Action<FileInfo> OnDelete)
	{
		gameObject.SetActive(true);

		nameText.text = file.Name.Replace(file.Extension, "");
		extensionText.text = file.Extension;

		flagText.enabled = type switch
		{
			FileType.image => file.Extension != ".bmp",
			FileType.code => file.Extension != ".cpp"
		};

		selectButton.onClick.RemoveAllListeners();
		selectButton.onClick.AddListener(() => OnSelect?.Invoke(flagText.enabled));

		deleteButton.onClick.RemoveAllListeners();
		deleteButton.onClick.AddListener(() => OnDelete?.Invoke(file));
	}
}