using UnityEngine;
using static Skinning;

/// <summary>Base class to skin game elements</summary>
public abstract class SkinElement : MonoBehaviour
{
	public SkinTag skinTag;

	bool isSkinned;

	void Awake()
	{
		isSkinned = false;
	}

	void Update()
	{
		if (!isSkinned && Skinning.IsReady())
		{
			Skin();
			Skinning.Register(this);
		}
	}

	public virtual void Skin()
	{
		isSkinned = true;
	}

	void OnDestroy()
	{
		Skinning.Resign(this);
	}
}