using UnityEngine;

/// <summary>Base class for full screen panels</summary>
public class Panel : MonoBehaviour
{
	private static Panel current;

	[Header(nameof(Panel) + " Settings")]
	public bool overrides;
	public bool fullScreen;

	public virtual void Pop()
	{
		if (fullScreen && !overrides)
			overrides = true;

		if (overrides)
		{
			if (current != null)
				current.gameObject.SetActive(false);

			if (fullScreen)
			{
				Resolution res = Screen.resolutions[^1];
				Screen.SetResolution(res.width, res.height, true);
			}
			else
			{
				RectTransform rectTransform = GetComponent<RectTransform>();
				Screen.SetResolution(
					Mathf.FloorToInt(rectTransform.rect.width),
					Mathf.FloorToInt(rectTransform.rect.width),
					false
				);

				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
				rectTransform.sizeDelta = Vector2.zero;
			}

			current = this;
		}

		gameObject.SetActive(true);
	}
}