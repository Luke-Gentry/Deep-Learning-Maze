using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasPositioner : MonoBehaviour
{
    public Canvas canvas;
    public GameObject mazeManager;

    public GameObject textPrefab;
    public GameObject arrowPrefab;
    public GameObject statManager;

    private GameObject[,,] textArray;
    private GameObject[,] imageArray;
    public int[,] integerArray;

    public void Start()
    {
        canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(mazeManager.GetComponent<MazeInitializer>().dimension, mazeManager.GetComponent<MazeInitializer>().dimension);
        textArray = new GameObject[mazeManager.GetComponent<MazeInitializer>().dimension, mazeManager.GetComponent<MazeInitializer>().dimension, 4];
        imageArray = new GameObject[mazeManager.GetComponent<MazeInitializer>().dimension, mazeManager.GetComponent<MazeInitializer>().dimension];
        integerArray = new int[mazeManager.GetComponent<MazeInitializer>().dimension, mazeManager.GetComponent<MazeInitializer>().dimension];
    }


    public void SetQTableValue (int x, int y, int i, float val) {
        canvas.transform.GetChild(0).gameObject.SetActive(true);
        float f = val;
        f = Mathf.Round(f * 100.0f) * 0.01f;

        if (textArray[x,y,i] == null) {
            Vector2 textPosition;
            int textRotation;
            if (i == 0) {
                textPosition = new Vector2(x + .5f, y + .85f);
                textRotation = 0;
            }
            else if (i == 1) {
                textPosition = new Vector2(x + .85f, y + .5f);
                textRotation = 90;
            }
            else if (i == 2) {
                textPosition = new Vector2(x + .5f, y + .15f);
                textRotation = 0;
            }
            else if (i == 3) {
                textPosition = new Vector2(x + .15f, y + .5f);
                textRotation = 270;
            } else {
                textPosition = new Vector2(x + .5f, y + .85f);
                textRotation = 0;
            }
            GameObject tempTextBoxParent = Instantiate(textPrefab, textPosition, Quaternion.Euler(0, 0, textRotation));
            textArray[x,y,i] = tempTextBoxParent;
            tempTextBoxParent.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = f.ToString();
            tempTextBoxParent.transform.SetParent(canvas.transform.GetChild(0).transform, false);
            return;
        }

        
        textArray[x,y,i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = f.ToString();

        
    }

    public void SetArrows (int x, int y, Vector4 vals) {
        canvas.transform.GetChild(1).gameObject.SetActive(true);
        float max = vals[0];
        int maxIndex = 0;
        for (int i = 1; i < 4; i++) {
            if (vals[i] > max) {max = vals[i]; maxIndex = i;}
        }
        

        if (imageArray[x,y] == null) {
            GameObject imageParent = Instantiate(arrowPrefab, new Vector3(x, y, 0), Quaternion.Euler(0, 0, 0));
            imageParent.transform.GetChild(0).GetComponent<Image>().transform.rotation = Quaternion.Euler(0, 0, -90 * maxIndex);
            imageArray[x,y] = imageParent;
            imageParent.transform.SetParent(canvas.transform.GetChild(1).transform, false);
            integerArray[x,y] = 0;

        } 
        else 
        {
            GameObject imageParent = imageArray[x,y];
            imageParent.transform.GetChild(0).GetComponent<Image>().transform.rotation = Quaternion.Euler(0, 0, -90 * maxIndex);
            integerArray[x,y] = maxIndex;
        }
    }

    public void HideArrows() {
        canvas.transform.GetChild(1).gameObject.SetActive(false);
        statManager.GetComponent<StatDisplayer>().resetStatColor();
    }

    public void HideNumbers() {
        canvas.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void DisplayArrows() {
        canvas.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void DisplayQTable() {
        canvas.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void ResetCanvas() {
        Debug.Log(mazeManager.GetComponent<MazeInitializer>().dimension);
        for (int i = 0; i < mazeManager.GetComponent<MazeInitializer>().dimension; i++) {
            for (int j = 0; j < mazeManager.GetComponent<MazeInitializer>().dimension; j++) {
                integerArray[i,j] = 0;
                Destroy(imageArray[i,j]);
                for (int k = 0; k < 4; k++) {
                    Destroy(textArray[i,j,k]);
                }
            }
        }
    }
}
