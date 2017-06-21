using UnityEngine;

public class SmoothFollow : MonoBehaviour 
{
	[SerializeField]
	private Transform target;
	
	[SerializeField]
	private float smoothTime = 0.3f;
	
	private Vector2 velocity;
	
	private Vector3 offset;
	
	void Start()
	{
		offset = Vector3.zero;
	}
	
	void Update () 
	{
		float positionX = Mathf.SmoothDamp( transform.position.x, target.position.x + offset.x, ref velocity.x, smoothTime);
		float positionY = Mathf.SmoothDamp( transform.position.y, target.position.y + offset.y, ref velocity.y, smoothTime);
		//float positionY = UiModel.instance.zoomCamera;
		transform.position = new Vector3(positionX, positionY, transform.position.z);
	}
}
