using UnityEngine;

public class Controls : MonoBehaviour
{
	const float SPEED = 30;

	void Update()
	{
		transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * SPEED;
	}
}
