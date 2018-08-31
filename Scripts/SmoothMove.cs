using UnityEngine;
using System.Collections;

public class SmoothMove : MonoBehaviour {
	public Vector3 destination;
	public float speed = 10f;

	void Start () {
		destination = transform.position;
	}

	void Update () {
		transform.position = Vector3.Lerp(transform.position, destination, speed * Time.deltaTime);
	}

	public void setVector(Vector3 vec){
		destination = vec;
	}
	public void setSpeed(float inSpeed){
		speed = inSpeed;
	}
}