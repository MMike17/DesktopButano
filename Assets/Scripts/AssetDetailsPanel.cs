using System;
using System.IO;
using B83.Image.BMP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static ProjectExplorer;

/// <summary>Used to display details of assets</summary>
public class AssetDetailsPanel : MonoBehaviour
{
	[Header("References")]
	public TMP_Text detailsNameText;
	public TMP_Text detailsExtensionText;
	public TMP_Text detailsSizeText;
	public GameObject detailsWarning;
	public TMP_Text detailsWarningText;
	[Space]
	public GameObject imageDetails;
	public GameObject imagePreview;
	public TMP_Text imageDetailsDimentions;
	public RawImage imagePreviewTexture;
	public GameObject imagePreviewErrorText;
	[Space]
	public GameObject codeDetails;
	public GameObject codePreview;
	public TMP_Text codeDetailsLines;
	public TMP_Text codePreviewText;

	public FileInfo selected { get; private set; }

	private GeneralSettings settings;
	private BMPLoader bmpLoader;

	private void Awake()
	{
		settings = GeneralSettings.Get();
		bmpLoader = new BMPLoader();
	}

	public void ShowDetails(FileInfo asset, FileType selectedType, bool valid)
	{
		selected = asset;

		detailsNameText.text = asset.Name.Replace(asset.Extension, "");
		detailsExtensionText.text = asset.Extension;
		detailsSizeText.text = asset.Length + "o";

		imageDetails.SetActive(selectedType == FileType.image);
		imagePreview.SetActive(selectedType == FileType.image);

		codeDetails.SetActive(selectedType == FileType.code);
		codePreview.SetActive(selectedType == FileType.code);

		switch (selectedType)
		{
			case FileType.image:
				detailsWarning.SetActive(valid);
				detailsWarningText.text = settings.detailsImageWarning;
				Texture2D texture = null;

				try
				{
					texture = bmpLoader.LoadBMP(asset.FullName).ToTexture2D();
					texture.filterMode = FilterMode.Point;
				}
				catch (Exception ex)
				{
					Debug.Log(ex);
				}

				imagePreviewTexture.enabled = texture != null;
				imagePreviewErrorText.SetActive(texture == null);

				if (imagePreviewTexture.enabled)
					imagePreviewTexture.texture = texture;

				imageDetailsDimentions.text = texture.width + "x" + texture.height;
				break;

			case FileType.code:
				detailsWarning.SetActive(valid);
				detailsWarningText.text = settings.detailsCodeWarning;

				string content = string.Empty;
				string[] lines = File.ReadAllLines(asset.FullName);

				for (int i = 0; i < Mathf.Min(lines.Length, settings.detailsCodeMaxLines); i++)
					content += lines[i] + "\n";

				content.TrimEnd('\n');
				codePreviewText.text = content;
				codeDetailsLines.text = lines.Length.ToString();
				break;
		}
	}
}