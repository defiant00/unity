using UnityEngine;

public class Cat : MonoBehaviour {
	public Vector3 Target;

	const float MOVE_SPEED = 25;

	private void Start()
	{
		Target = transform.position;
	}

	private void Update()
	{
		transform.position = Vector3.MoveTowards(transform.position, Target, Time.deltaTime * MOVE_SPEED);
	}
}
