using System;

/// <summary>Class used to generate json files holding meta data for regular_bg assets</summary>
[Serializable]
public class RegularBgMeta : AffineBgMeta
{
	public string bpp_mode;
	// bpp_4_auto : doesn't work if palette is used
	// bpp_4_manual : expects valid palette
	public string flipped_tiles_reduction; // true by default
	public string palette_reduction; // true by default

	public void SetBPPMode(BPPMode bpp_mode) => this.bpp_mode = bpp_mode.ToString();

	public void SetFlippedTilesReduction(bool flipped_tiles_reduction)
	{
		this.flipped_tiles_reduction = flipped_tiles_reduction.ToString();
	}

	public void SetPaletteReduction(bool palette_reduction) => this.palette_reduction = palette_reduction.ToString();
}