using UnityEngine;

public class GameTile : MonoBehaviour
{
	public GameGrid Grid;

	public bool IsMine;
	public bool Covered = true;

	public Sprite[] EmptyTextures;
	public Sprite MineTexture;

	public void SetTexture(int count)
	{
		GetComponent<SpriteRenderer>().sprite = IsMine ? MineTexture : EmptyTextures[count];
	}

	void OnMouseUpAsButton()
	{
		Grid.TileClicked((int)transform.position.x, (int)transform.position.y);
	}
}
