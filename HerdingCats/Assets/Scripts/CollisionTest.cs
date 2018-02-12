using UnityEngine;

public class CollisionTest : MonoBehaviour
{
	private void OnTriggerStay2D(Collider2D collision)
	{
		var c = collision.GetComponentInParent<Cat>();
		if (c.Target == c.transform.position)
		{
			c.Target = c.transform.position + new Vector3((Random.value - 0.5f) * 20, (Random.value - 0.5f) * 20);
		}
	}
}
