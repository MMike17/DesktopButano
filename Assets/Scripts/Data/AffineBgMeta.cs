using System;

/// <summary>Class used to generate json files holding meta data for affine_bg assets</summary>
[Serializable]
public class AffineBgMeta : ColorsCountMeta
{
	public string height;
	public string tiles_compression;
	public string palette_compression;
	public string palette_item;
	public string repeated_tiles_reduction; // true by default
	public string big; // forces set big to true
	public string map_compression;

	public void SetHeight(int height) => this.height = height.ToString();
	public void SetTilesCompression(Compression tiles_compression) => this.tiles_compression = tiles_compression.ToString();

	public void SetPaletteCompression(Compression palette_compression)
	{
		this.palette_compression = palette_compression.ToString();
	}

	public void SetRepeatedTilesReduction(bool repeated_tiles_reduction)
	{
		this.repeated_tiles_reduction = repeated_tiles_reduction.ToString();
	}

	public void SetBig(bool big) => this.big = big.ToString();
	public void SetMapCompression(Compression map_compression) => this.map_compression = map_compression.ToString();
}