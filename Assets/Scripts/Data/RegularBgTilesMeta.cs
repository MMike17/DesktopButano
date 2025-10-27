using System;

/// <summary>Class used to generate json files holding meta data for regular_bg_tiles assets</summary>
[Serializable]
public class RegularBgTilesMeta : AffineBgTilesMeta
{
	public string bpp_mode;
	// no auto or manual
	// bpp_4 expects palette to be valid

	public void SetBPPMode(BPPMode bpp_mode) => this.bpp_mode = bpp_mode.ToString();
}