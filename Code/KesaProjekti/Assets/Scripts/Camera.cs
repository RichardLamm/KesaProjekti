using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {


    private float xAxis;
    private float yAxis;

    //The speed at which the camera moves
    public float speed = 5;

	// Use this for initialization
	void Start () {
		
	}
	
	// The camera movement is done with the input manager (WASD and arrow keys by default)
	void Update () {
        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");
        transform.position += new Vector3(xAxis, yAxis, 0) * Time.deltaTime * speed;
    }
}
