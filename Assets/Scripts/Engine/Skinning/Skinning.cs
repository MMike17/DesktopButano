using System.Collections.Generic;
using UnityEngine;

public static class Skinning
{
	public enum SkinTag
	{
		BACKGROUND,

		PRIMARY_WINDOW,
		PRIMARY_ELEMENT,

		SECONDARY_WINDOW,
		SECONDARY_ELEMENT,

		CONTRAST,

		VALIDATE,
		DELETE,

		PICTO
	}

	static SkinData actual_skin;

	static List<SkinElement> components;

	public static void Init(SkinData data)
	{
		actual_skin = data;
	}

	public static Color GetSkin(SkinTag tag)
	{
		if (!IsReady())
		{
			Debug.LogError("<b>[Skinning]</b> : skinning is not ready (skin data has not been assigned)");
			return Color.black;
		}

		return actual_skin.GetSkin(tag);
	}

	public static bool IsReady()
	{
		return actual_skin != null;
	}

	public static void Register(SkinElement graphic)
	{
		if (components == null)
			components = new List<SkinElement>();

		components.Add(graphic);
	}

	public static void Resign(SkinElement graphic)
	{
		if (components == null)
		{
			components = new List<SkinElement>();
			return;
		}

		components.Remove(graphic);
	}

	public static void ResetSkin(SkinData data)
	{
		actual_skin = data;
		components.ForEach((item) => { item.Skin(); });
	}
}