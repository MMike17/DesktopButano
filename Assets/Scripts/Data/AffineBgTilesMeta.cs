using System;

/// <summary>Class used to generate json files holding meta data for affine_bg_tiles assets</summary>
[Serializable]
public class AffineBgTilesMeta : ImageMeta
{
	public string palette_compression;
	public string generate_palette; // false by default
	public string palette_colors_count;

	public void SetPaletteCompression(Compression palette_compression)
	{
		this.palette_compression = palette_compression.ToString();
	}

	public void SetGeneratePalette(bool generate_palette) => this.generate_palette = generate_palette.ToString();

	public void SetPaletteColorsCount(int palette_colors_count)
	{
		this.palette_colors_count = palette_colors_count.ToString();
	}
}