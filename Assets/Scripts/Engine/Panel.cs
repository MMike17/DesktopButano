using UnityEngine;

/// <summary>Base class for full screen panels</summary>
public class Panel : MonoBehaviour
{
	private static Panel current;

	[Header(nameof(Panel) + " Settings")]
	public Vector2Int size;
	public bool overrides;
	public bool fullScreen;

	public void Pop()
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
				Screen.SetResolution(size.x, size.y, false);

			current = this;
		}

		gameObject.SetActive(true);
	}
}