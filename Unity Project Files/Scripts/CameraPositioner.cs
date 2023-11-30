using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositioner : MonoBehaviour
{

    public GameObject mazeManager;
    public Camera camera;
    public int dimension;

    public void Start()
    {
        float dimensionFloat = (float) mazeManager.GetComponent<MazeInitializer>().dimension;
        camera.transform.position = new Vector3(dimensionFloat/2, dimensionFloat/2 + dimensionFloat/10, -10);
        camera.orthographicSize = (dimensionFloat - dimensionFloat/3);
    }

    
}
