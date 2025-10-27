using System;

/// <summary>Abstract class used to generate json files holding meta data for color counted able assets</summary>
[Serializable]
public abstract class ColorsCountMeta : ImageMeta
{
	public string colors_count; // [1...256]

	public void SetColors(int colors_count) => this.colors_count = colors_count.ToString();
}