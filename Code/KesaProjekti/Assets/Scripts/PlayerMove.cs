using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

    private enum playerState
    {
        Idle,
        Moving,
        Map
    };
    public int speed = 10;
    private playerState state = playerState.Idle;
    private float xAxis;
    private float yAxis;
    private Vector3 cameraOffset;

    // Use this for initialization
    void Start () {
        cameraOffset = Camera.main.transform.position - transform.position;
    }

	
	// Update is called once per frame
	void Update () {
        PlayerStateMachine();       
    }


    public void PlayerStateMachine()
    {
        switch (state)
        {
            case playerState.Idle:
                if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
                {
                    state = playerState.Moving;
                }
                else if (Input.GetButtonDown("Map"))
                {
                    state = playerState.Map;
                    Camera.main.orthographicSize = 30;
                }
                break;

            case playerState.Moving: 
                xAxis = Input.GetAxis("Horizontal");
                yAxis = Input.GetAxis("Vertical");
                transform.position += new Vector3(xAxis, yAxis, 0) * Time.deltaTime * speed;
                Camera.main.transform.position = transform.position + cameraOffset;

                //TODO: Testaile eri statenvaihtotapoja, nyt ei voi tehdä täyskäännöstä suoraan.
                if ( xAxis == 0 && yAxis == 0)
                {
                    state = playerState.Idle;
                }               
                break;

            case playerState.Map:
                xAxis = Input.GetAxis("Horizontal");
                yAxis = Input.GetAxis("Vertical");
                Camera.main.transform.position += new Vector3(xAxis, yAxis, 0) * Time.deltaTime * 2 * speed;
                if (Input.GetButtonDown("Map"))
                {
                    state = playerState.Idle;
                    Camera.main.transform.position = transform.position + cameraOffset;
                    Camera.main.orthographicSize = 5;
                }
                break;
        }
    }
}
