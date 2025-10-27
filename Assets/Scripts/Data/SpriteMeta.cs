using System;

/// <summary>Class used to generate json files holding meta data for sprite assets</summary>
[Serializable]
public class SpriteMeta : SpriteTilesMeta
{
	// - bpp_mode
	// no auto or manual

	public string tiles_compression;
	public string palette_compression;

	public void SetTilesCompression(Compression tiles_compression) => this.tiles_compression = tiles_compression.ToString();

	public void SetPaletteCompression(Compression palette_compression)
	{
		this.palette_compression = palette_compression.ToString();
	}
}