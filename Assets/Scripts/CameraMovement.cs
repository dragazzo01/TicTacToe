using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float force;
    public float zoomFloat;
    public Transform cameraTran;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w"))
            cameraTran.position = cameraTran.position + new Vector3(0, force, 0);
        if (Input.GetKey("a"))
            cameraTran.position = cameraTran.position + new Vector3(-1*force, 0, 0);
        if (Input.GetKey("s"))
            cameraTran.position = cameraTran.position + new Vector3(0, -1 *force, 0);
        if (Input.GetKey("d"))
            cameraTran.position = cameraTran.position + new Vector3(force, 0, 0);
        if (Input.GetKey("q"))
            GetComponent<Camera>().orthographicSize += zoomFloat;
        if (Input.GetKey("e"))
            GetComponent<Camera>().orthographicSize -= zoomFloat;
    }
}
