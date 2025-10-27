using System;

/// <summary>Class used to generate json files holding meta data for sprite_tiles assets</summary>
[Serializable]
public class SpriteTilesMeta : PaletteMeta
{
	// - bpp_mode
	// no auto or manual

	public string height;
	public string width;

	public void SetHeight(int height) => this.height = height.ToString();
	public void SetWidth(int width) => this.width = width.ToString();
}