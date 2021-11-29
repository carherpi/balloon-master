using UnityEngine;
public class CameraFollow : MonoBehaviour
{
	[SerializeField]
	private Transform player;
	[SerializeField]
	public float smoothSpeed;
	[SerializeField]
	public Vector3 offset;

	void FixedUpdate()
	{
		Vector3 desiredPosition = player.position + offset;
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
		transform.position = smoothedPosition;

		transform.LookAt(player);
	}

}