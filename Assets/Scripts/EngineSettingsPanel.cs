using System;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Panel used to show and adjust engine and app settings</summary>
public class EngineSettingsPanel : Panel
{
	[Header(nameof(EngineSettingsPanel) + " References")]
	public Button projectPathButton;
	public TMP_Text projectPathText;
	public Button butanoPathButton;
	public TMP_Text butanoPathText;
	public TMP_InputField codeToolInput;
	public Button codeToolPickButton;
	public TMP_InputField imageToolInput;
	public Button imageToolPickButton;
	public TMP_InputField soundToolInput;
	public Button soundToolPickButton;
	public Button settingsCloseButton;
	// SETT : Add ui for settings

	private GeneralSettings settings;

	public void Pop(Action AskForRoot, Action AskForButano, Action OnClose)
	{
		settings = GeneralSettings.Get();

		// SETT : Set fields to saved settings
		projectPathText.text = PlayerPrefs.GetString(settings.projectRootKey);
		butanoPathText.text = PlayerPrefs.GetString(settings.projectButanoKey);
		codeToolInput.text = PlayerPrefs.GetString(settings.projectCodeKey);
		imageToolInput.text = PlayerPrefs.GetString(settings.projectImageKey);
		soundToolInput.text = PlayerPrefs.GetString(settings.projectSoundKey);

		projectPathButton.onClick.RemoveAllListeners();
		projectPathButton.onClick.AddListener(() => AskForRoot());

		butanoPathButton.onClick.RemoveAllListeners();
		butanoPathButton.onClick.AddListener(() => AskForButano());

		codeToolPickButton.onClick.AddListener(() =>
		{
			FileBrowser.ShowLoadDialog(
				paths => codeToolInput.SetTextWithoutNotify(paths[0]),
				null,
				FileBrowser.PickMode.Files,
				false,
				title: settings.projectCodeMessage
			);
		});

		imageToolPickButton.onClick.AddListener(() =>
		{
			FileBrowser.ShowLoadDialog(
				paths => imageToolInput.SetTextWithoutNotify(paths[0]),
				null,
				FileBrowser.PickMode.Files,
				false,
				title: settings.projectImageMessage
			);
		});

		soundToolPickButton.onClick.AddListener(() =>
		{
			FileBrowser.ShowLoadDialog(
				paths => soundToolInput.SetTextWithoutNotify(paths[0]),
				null,
				FileBrowser.PickMode.Files,
				false,
				title: settings.projectSoundMessage
			);
		});

		settingsCloseButton.onClick.AddListener(() =>
		{
			// SETT : Save settings
			PlayerPrefs.SetString(settings.projectCodeKey, codeToolInput.text);
			PlayerPrefs.SetString(settings.projectImageKey, imageToolInput.text);
			PlayerPrefs.SetString(settings.projectSoundKey, soundToolInput.text);
			OnClose?.Invoke();
		});

		base.Pop();
	}
}