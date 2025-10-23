using UnityEngine;

/// <summary>Base class for full screen panels</summary>
public class Panel : MonoBehaviour
{
	private static Panel current;

	[Header(nameof(Panel) + " Settings")]
	public bool overrides;
	public bool fullScreen;

	private RectTransform rectTransform;
	private Vector2Int size;

	private void Init()
	{
		if (rectTransform != null)
			return;

		rectTransform = GetComponent<RectTransform>();

		if (overrides)
		{
			if (fullScreen)
			{
				Resolution res = Screen.resolutions[^1];
				size = new Vector2Int(res.width, res.height);
			}
			else
				size = new Vector2Int(Mathf.FloorToInt(rectTransform.rect.width), Mathf.FloorToInt(rectTransform.rect.height));
		}
	}

	public virtual void Pop()
	{
		Init();

		if (fullScreen && !overrides)
			overrides = true;

		if (overrides)
		{
			if (current != null)
				current.gameObject.SetActive(false);

			Screen.SetResolution(size.x, size.y, false);

			if (!fullScreen)
			{
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
				rectTransform.sizeDelta = Vector2.zero;
			}

			current = this;
		}

		gameObject.SetActive(true);
	}
}