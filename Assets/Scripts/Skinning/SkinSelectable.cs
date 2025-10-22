using UnityEngine;
using UnityEngine.UI;
using static Skinning;

/// <summary>Class used to skin UI elements inheriting from UnityEngine.UI.Selectable</summary>
[DisallowMultipleComponent]
public class SkinSelectable : SkinElement
{
	[Header("Settings")]
	public SkinTag highlightedColorTag;
	public SkinTag pressedColorTag;

	Selectable internalSelectable;
	Selectable selectable
	{
		get
		{
			if(internalSelectable == null)
				internalSelectable = GetComponent<Selectable>();

			return internalSelectable;
		}
	}

	public override void Skin()
	{
		base.Skin();

		ColorBlock selectableColors = new ColorBlock()
		{
			normalColor = Skinning.GetSkin(skinTag),
				highlightedColor = Skinning.GetSkin(highlightedColorTag),
				pressedColor = Skinning.GetSkin(pressedColorTag),
				fadeDuration = 0.1f,
				colorMultiplier = 1
		};

		selectable.colors = selectableColors;
	}
}