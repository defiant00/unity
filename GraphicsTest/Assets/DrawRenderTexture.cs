using UnityEngine;

public class DrawRenderTexture : MonoBehaviour
{
	public RenderTexture RTex;
	public Material Mat;

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination);
		Graphics.Blit(RTex, destination, Mat);
	}
}
