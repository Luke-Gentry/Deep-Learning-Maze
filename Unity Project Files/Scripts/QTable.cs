using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTable : MonoBehaviour
{

    public float[,,] qTable;
    public GameObject mazeManager;
    private int dimension;
    public Canvas canvas;

    public void Start () {
        dimension = mazeManager.GetComponent<MazeInitializer>().dimension;
        qTable = new float[dimension, dimension, 4];
        InitializeQTable();
    }

    public void InitializeQTable() {
        for (int x = 0; x < dimension; x++) {
            for (int y = 0; y < dimension; y++) {
                for (int i = 0; i < 4; i++) {
                    qTable[x,y,i] = 0f;
                }
            }
        }
    }

    public void SetArrows () {
        dimension = mazeManager.GetComponent<MazeInitializer>().dimension;
        for (int x = 0; x < dimension; x++) {
            for (int y = 0; y < dimension; y++) {
                canvas.GetComponent<CanvasPositioner>().SetArrows(x, y, new Vector4 (qTable[x,y,0], qTable[x,y,1], qTable[x,y,2], qTable[x,y,3]));
            }
        }
    }

    public void SetQTableValue() {
        for (int x = 0; x < dimension; x++) {
            for (int y = 0; y < dimension; y++) {
                for (int i = 0; i < 4; i++) {
                    canvas.GetComponent<CanvasPositioner>().SetQTableValue(x, y, i, qTable[x,y,i]);
                }
            }
        }
    }


    public void displayQTable() {
        canvas.GetComponent<CanvasPositioner>().DisplayQTable();
    }

    public void displayArrows() {
        canvas.GetComponent<CanvasPositioner>().DisplayArrows();
    }

    public void hideArrows() {
        canvas.GetComponent<CanvasPositioner>().HideArrows();
    }

    public void hideQTable() {
        canvas.GetComponent<CanvasPositioner>().HideNumbers();
    }


    public void resetQTable () {
        for (int x = 0; x < dimension; x++) {
            for (int y = 0; y < dimension; y++) {
                for (int i = 0; i < 4; i++) {
                    qTable[x,y,i] = 0;
                }
            }
        }
        Invoke("SetQTableValue", .1f);
        Invoke("SetArrows", .1f);
        Invoke("InitializeQTable", .1f);
        //Invoke("displayArrows", .1f);
    }
}
