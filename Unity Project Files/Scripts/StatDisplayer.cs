using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StatDisplayer : MonoBehaviour
{
    public List<GameObject> statList;
    public GameObject mazeManager;
    public Canvas canvas;
    private int dimension;
    private float accuracy;
    private GameObject[,] tiles;
    public TMPro.TMP_Dropdown displayDrop;
    public List<Vector2> discrepencyTiles;
    public bool accurate;

    void Start () {
        
        discrepencyTiles = new List<Vector2>();
        accurate = false;
        dimension = mazeManager.GetComponent<MazeInitializer>().dimension;
        tiles = mazeManager.GetComponent<MazeInitializer>().tiles;
        statList = new List<GameObject>();
        statList.Add(gameObject.transform.GetChild(0).gameObject);
        statList.Add(gameObject.transform.GetChild(1).gameObject);
        statList.Add(gameObject.transform.GetChild(2).gameObject);
    }

    public void updateStats (int numberOfAgents, int generation) { 
        try {
            if (statList[2] == null) {return;};
        } catch (Exception e) {
            return;
        }
        statList[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Number of Agents: " + numberOfAgents.ToString();
        statList[1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Generation: " + (generation * numberOfAgents).ToString() + " - " + (numberOfAgents * (generation + 1)).ToString();
        calculateAccuracy();
        if (accurate == false) {
            statList[2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Accuracy: " + accuracy.ToString() + "%";
        }
        if (accuracy == 100 && accurate == false) {
            statList[2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color (0, 255, 0);
            int finalGen = generation * numberOfAgents;
            accurate = true;
            statList[2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Accuracy: " + accuracy.ToString() + "%" + " - Generation " + finalGen.ToString();
        }




    } 


    void calculateAccuracy () {

        int[,] solveCode = mazeManager.GetComponent<MazeInitializer>().solveCode;
        int[,] integerArray = canvas.GetComponent<CanvasPositioner>().integerArray;

        dimension = mazeManager.GetComponent<MazeInitializer>().dimension;

        int discrepencies = 0;
        discrepencyTiles.Clear();

        if (tiles[0,0] == null) {
            tiles = mazeManager.GetComponent<MazeInitializer>().tiles;
        }

        for (int i = 0; i < dimension; i++) {
            for (int j = 0; j < dimension; j++) {
                if (solveCode[i,j] != integerArray[i,j]) {
                    discrepencies++;
                    discrepencyTiles.Insert(0, new Vector2(i, j));
                    if ((displayDrop.value == 2 || displayDrop.value == 3) && ((i == dimension - 1 && j == dimension - 1) || (i == 0 && j == 0)) == false) {
                        tiles[i,j].GetComponent<SpriteRenderer>().color = new Color(255,0,0);
                    }
                } else {
                    if ((displayDrop.value == 2 || displayDrop.value == 3)) {
                        if (((i == dimension - 1 && j == dimension - 1) || (i == 0 && j == 0)) == false) {
                            tiles[i,j].GetComponent<SpriteRenderer>().color = new Color(255,255,255);
                        }
                    }
                }
            }
        }



        accuracy = 100 * (((dimension * dimension) - discrepencies)) / (float) (dimension * dimension);

        
    }

    public void resetStat() {
        accuracy = 0;
        accurate = false;
        for (int i = 0; i < dimension; i++) {
            for (int j = 0; j < dimension; j++) {
                if (((i == dimension - 1 && j == dimension - 1) || (i == 0 && j == 0)) == false) {
                    try {
                        tiles[i,j].GetComponent<SpriteRenderer>().color = new Color(255,255,255);
                    } catch (Exception e) {
                        return;
                    }
                }
            }
        }
        gameObject.SetActive(false);
        discrepencyTiles.Clear();
        
    }

    public void resetStatColor() {

        for (int i = 0; i < dimension; i++) {
            for (int j = 0; j < dimension; j++) {
                if (((i == dimension - 1 && j == dimension - 1) || (i == 0 && j == 0)) == false) {
                    try {
                        tiles[i,j].GetComponent<SpriteRenderer>().color = new Color(255,255,255);
                    } catch (Exception e) {
                        return;
                    }
                }
            }   
        }
    }
}
