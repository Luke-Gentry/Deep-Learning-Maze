using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AgentManager : MonoBehaviour
{
    public TMPro.TMP_Dropdown displayDrop;
    public GameObject agentPrefab;
    public GameObject QTableManager;
    public GameObject mazeManager;
    public GameObject statManager;
    public int numberOfAgents;
    public List<GameObject> agents;

    public float rewardValue;
    public float rewardMove;
    public float rewardWallHit;
    public float rewardMoveToVisited;
    public float rewardMoveOutOfBounds;
    public float rewardReachEnd;
    public float rewardFloorConstant;
    public int maxTrials;
    public int trialNumber;
    public int maxSteps;
    public int agentSpeed;
    public int explorationConstant;
    public float discountConstant;

    public float agentsFinished;
    public Vector3 startingPosition;
    public int dimension;

    private int locIndex;
    private int generation;
    public TMPro.TMP_Dropdown dropAI;
    public TMPro.TMP_Dropdown speedDrop;

    public void Start () {
        locIndex = 0;
        generation = 0;
        dimension = mazeManager.GetComponent<MazeInitializer>().dimension;
        trialNumber = 0;
    }

    public void InitializeAgents() {
        for (int i = 0; i < numberOfAgents; i++) {
            GameObject a = Instantiate(agentPrefab, new Vector3(0.5f,0.5f, 0), Quaternion.identity);
            a.transform.SetParent(gameObject.transform);
            agents.Add(a);
        }
        for (int i = 0; i < agents.Count; i++) {
            agents[i].GetComponent<MazeAI>().InitializeAgent();
        }
        statManager.gameObject.SetActive(true);
    }

    public void StartSolve() {
        for (int i = 0; i < agents.Count; i++) {
            agents[i].GetComponent<MazeAI>().StartSolve();
        }
    }

    private int t = 0;
    void Update()
    {

        QTableManager.GetComponent<QTable>().SetArrows(); 
        QTableManager.GetComponent<QTable>().SetQTableValue();

        if (displayDrop.value == 0) {
            QTableManager.GetComponent<QTable>().hideArrows(); 
            QTableManager.GetComponent<QTable>().hideQTable();
        }
        if (displayDrop.value == 1) {
            QTableManager.GetComponent<QTable>().displayQTable(); 
            QTableManager.GetComponent<QTable>().hideArrows(); 
        }
        if (displayDrop.value == 2) {
            QTableManager.GetComponent<QTable>().displayArrows();
            QTableManager.GetComponent<QTable>().hideQTable(); 
        }
        if (displayDrop.value == 3) {
            QTableManager.GetComponent<QTable>().displayQTable(); 
            QTableManager.GetComponent<QTable>().displayArrows();
        }

        if (statManager.gameObject.activeInHierarchy && t > 100) {
            statManager.GetComponent<StatDisplayer>().updateStats(numberOfAgents, generation);
        }

        if (agentsFinished == numberOfAgents) {
            agentsFinished = 0;
            trialNumber += numberOfAgents;
            generation++;
            int r = 0;
            if (statManager.GetComponent<StatDisplayer>().discrepencyTiles.Count == 0) {
                startingPosition = new Vector3(Random.Range(0,dimension-1) + .5f, Random.Range(0,dimension-1) + .5f, 0);
            }
            else {
                if (dropAI.value == 2) {
                    r = Random.Range(0,statManager.GetComponent<StatDisplayer>().discrepencyTiles.Count);
                }
                else if (statManager.GetComponent<StatDisplayer>().discrepencyTiles.Count < 5) {
                    r = Random.Range(0,statManager.GetComponent<StatDisplayer>().discrepencyTiles.Count);
                } else {
                    r = Random.Range(0,5);
                }
                Vector2 startingTile = statManager.GetComponent<StatDisplayer>().discrepencyTiles[r];
                startingPosition = new Vector3(startingTile.x + .5f, startingTile.y + .5f, 0);
            }
        }
        t++;
    }

    public void ResetAI () {
        for (int i = 0; i < numberOfAgents; i++) {
            if (agents.Count == 0) {
                i = numberOfAgents;
                break;
            }
            Destroy(agents[i]);
        }
        agents.Clear();
        QTableManager.GetComponent<QTable>().resetQTable();
        statManager.gameObject.SetActive(false);
        if (statManager.gameObject.activeInHierarchy) {
            statManager.GetComponent<StatDisplayer>().statList[2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color (255, 255, 255);
        }
        generation = 0;
        trialNumber = 0;
        statManager.GetComponent<StatDisplayer>().discrepencyTiles.Clear();
        startingPosition = new Vector3(.5f, .5f, 0);


        
    }

    public void ReadAgentCount (string numberOfAgentsInput) {
        numberOfAgents = int.Parse(numberOfAgentsInput);

    }
}
