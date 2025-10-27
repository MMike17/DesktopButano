using System;

/// <summary>Base class used to generate json files holding meta data for all image assets</summary>
[Serializable]
public abstract class ImageMeta
{
	// TODO : I don't get the difference between affine and regular bg (images are so fucking weird in butano)
	// TODO : Are all the fields of the json saved as text ? (check for enums, numbers and booleans)
	// TODO : Type control will be done on editor side

	public enum Type
	{
		sprite,
		sprite_tiles,
		sprite_palette,
		regular_bg,
		regular_bg_tiles,
		affine_bg,
		affine_bg_tiles,
		bg_palette
	}

	public enum Compression
	{
		none, // by default
		lz77,
		run_length,
		huffman,
		auto, // smallest data size
		auto_no_huffman // smallest exclude "huffman"
	}

	public enum BPPMode
	{
		bpp_4, // 16 colors
		bpp_8, // 256 colors
		bpp_4_auto,
		bpp_4_manual
	}

	public string type;
	public string compression;

	public void SetType(Type type) => this.type = type.ToString();

	public void SetCompression(Compression compression) => this.compression = compression.ToString();
}