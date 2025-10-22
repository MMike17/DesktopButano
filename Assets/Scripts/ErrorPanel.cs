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

		closeButton.onClick.AddListener(() => gameObject.SetActive(false));
	}

	public void Pop(string text)
	{
		messageText.text = text;
		base.Pop();
	}
}