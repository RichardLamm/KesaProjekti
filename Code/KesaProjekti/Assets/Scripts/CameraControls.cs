using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour {


    private float xAxis;
    private float yAxis;
    public float zAxis;

    //The speed at which the camera moves
    public float moveSpeed = 5;
    public float scrollSpeed = 2;

	// Use this for initialization
	void Start () {
        
    }
	
	// The camera movement is done with the input manager (WASD and arrow keys by default)
	void LateUpdate () {
        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");
        zAxis = Input.GetAxis("Mouse ScrollWheel");
        transform.position += new Vector3(xAxis, yAxis, 0) * Time.deltaTime * moveSpeed;        
        Camera.main.orthographicSize -= zAxis * scrollSpeed;
    }
}
