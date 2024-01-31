using UnityEngine;

[System.Serializable]
public class ShadowSettings {

	[Min(0f)]
	public float maxDistance = 100f;
	
	[System.Serializable]
	public struct Directional {

		public TextureSize atlasSize;
	}

	public Directional directional = new Directional {
		atlasSize = TextureSize._1024
	};
}