using UnityEngine;
using UnityEngine.UI;

/// <summary>Class used to skin UI elements inheriting from UnityEngine.UI.Graphics</summary>
[DisallowMultipleComponent]
public class SkinGraphic : SkinElement
{
	Graphic internalGraphic;
	Graphic graphic
	{
		get
		{
			if(internalGraphic == null)
				internalGraphic = GetComponent<Graphic>();

			return internalGraphic;
		}
	}

	public override void Skin()
	{
		base.Skin();

		graphic.color = Skinning.GetSkin(skinTag);
	}
}