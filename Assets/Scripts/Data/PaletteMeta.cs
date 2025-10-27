using System;

/// <summary>Class used to generate json files holding meta data for palette assets</summary>
[Serializable]
public class PaletteMeta : ColorsCountMeta
{
	public string bpp_mode;
	// no auto or manual

	public void SetBPPMode(BPPMode bpp_mode) => this.bpp_mode = bpp_mode.ToString();
}