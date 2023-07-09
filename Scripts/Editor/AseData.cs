using UnityEngine;
using System;

[System.Serializable]
public struct AseData{
	public AseFrameData[] frames;
	public AseMetaData meta;
}

[System.Serializable]
public struct AseFrameData{
	public string filename;
	public AseFrameSizeData frame;
	public bool rotated;
	public bool trimmed;
	public AseFrameSizeData spriteSourceSize;
	public AseFrameSizeData sourceSize;
	public int duration;
}

[System.Serializable]
public struct AseFrameSizeData{
	public int x, y, w, h;
}
	
[System.Serializable]
public struct AseMetaData{
	public string app;
	public string version;
	public string image;
	public string format;
	public AseFrameSizeData size;
	public int scale;
	public AseFrameTag[] frameTags;
	public AseLayer[] layers;
}

[System.Serializable]
public struct AseFrameTag{
	public string name;
	public int from;
	public int to;
	public string direction;
}

[System.Serializable]
public struct AseLayer{
	public string name;
	public int opacity;
	public string blendMode;
	public AseCel[] cels;
	public string data;
}

[System.Serializable]
public struct AseCel{
	public int frame;
	public string color;
	public string data;
}