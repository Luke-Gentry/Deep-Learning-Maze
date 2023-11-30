using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeAI : MonoBehaviour
{
    public GameObject agent;
    public GameObject qTableManager;
    public float rewardValue;
    public float[,,] qTable;
    public float rewardMove;
    public float rewardWallHit;
    public float rewardMoveToVisited;
    public float rewardMoveOutOfBounds;
    public float rewardReachEnd;
    public float rewardFloorConstant;
    public int maxTrials;
    public int maxSteps;
    private int dimension;
    public int agentSpeed;
    public GameObject mazeManager;
    public TMPro.TMP_Dropdown dropAI;
    public TMPro.TMP_Dropdown speedDrop;

    private GameObject[,] tiles;
    public int agentX;
    public int agentY;
    public Vector2 start;
    public Vector2 end;
    public Vector3 destinationVector;
    public List<Vector3> stateActionPairs;

    public int explorationConstant;
    public int explorationConstantLimit;
    public float discountConstant;
    public float numberOfAgents;
    public float epsilonGreedConstant;


    void Start () {
        destinationVector = new Vector3(.5f, .5f, 0);
    }
    public void InitializeAgent () {
        mazeManager = gameObject.transform.parent.GetComponent<AgentManager>().mazeManager;
        qTableManager = gameObject.transform.parent.GetComponent<AgentManager>().QTableManager;
        rewardValue = gameObject.transform.parent.GetComponent<AgentManager>().rewardValue;
        rewardMove = gameObject.transform.parent.GetComponent<AgentManager>().rewardMove;
        rewardMoveOutOfBounds = gameObject.transform.parent.GetComponent<AgentManager>().rewardMoveOutOfBounds;
        rewardMoveToVisited = gameObject.transform.parent.GetComponent<AgentManager>().rewardMoveToVisited;
        rewardReachEnd = gameObject.transform.parent.GetComponent<AgentManager>().rewardReachEnd;
        rewardWallHit = gameObject.transform.parent.GetComponent<AgentManager>().rewardWallHit;
        agentSpeed = gameObject.transform.parent.GetComponent<AgentManager>().agentSpeed;
        explorationConstantLimit = gameObject.transform.parent.GetComponent<AgentManager>().explorationConstant;
        discountConstant = gameObject.transform.parent.GetComponent<AgentManager>().discountConstant;
        numberOfAgents = gameObject.transform.parent.GetComponent<AgentManager>().numberOfAgents;
        rewardFloorConstant = gameObject.transform.parent.GetComponent<AgentManager>().rewardFloorConstant;
        maxTrials = gameObject.transform.parent.GetComponent<AgentManager>().maxTrials;
        maxSteps = gameObject.transform.parent.GetComponent<AgentManager>().maxSteps;
        dropAI = gameObject.transform.parent.GetComponent<AgentManager>().dropAI;
        speedDrop = gameObject.transform.parent.GetComponent<AgentManager>().speedDrop;
        explorationConstant = 100 - explorationConstantLimit;




        start = mazeManager.GetComponent<MazeInitializer>().start;
        end = mazeManager.GetComponent<MazeInitializer>().end;
        destinationVector = new Vector3(start.x + .5f, start.y + .5f, 0);
        agentX = (int) (agent.transform.position.x - 0.5);
        agentY = (int) (agent.transform.position.y - 0.5);
        tiles = mazeManager.GetComponent<MazeInitializer>().tiles;
        rewardValue = 0;
        dimension = mazeManager.GetComponent<MazeInitializer>().dimension;
        qTable = qTableManager.GetComponent<QTable>().qTable;
    }

    public void StartSolve() {
        if (dropAI.value == 2) {
            StartCoroutine(StartSolveMonteCarlo(start, end));
        }
        if (dropAI.value == 1) {
            StartCoroutine(StartSolveSARSA(start, end));
        }
        if (dropAI.value == 0) {
            StartCoroutine(StartSolveTD(start, end));
        }
    }



    

    private bool CheckActionHitWall(int action, GameObject currentTile, GameObject[] walls) {
        if (walls[action] != null) {
            return true;
        }
        return false;
    }

    private bool CheckActionOutOfBounds(int action, GameObject currentTile) {
        if (action == 0 && currentTile.GetComponent<TileData>().coordinate.y == dimension - 1) {
            return true;
        }
        if (action == 1 && currentTile.GetComponent<TileData>().coordinate.x == dimension - 1) {
            return true;
        }
        if (action == 2 && currentTile.GetComponent<TileData>().coordinate.y == 0) {
            return true;
        }
        if (action == 3 && currentTile.GetComponent<TileData>().coordinate.x == 0) {
            return true;
        }
        return false;
    }

    private void moveAgent(int action) {
        Vector3 t = agent.transform.position;
        if (action == 0) {agent.transform.position = new Vector3(t.x, t.y + 1, t.z);}
        if (action == 1) {agent.transform.position = new Vector3(t.x + 1, t.y, t.z);}
        if (action == 2) {agent.transform.position = new Vector3(t.x, t.y - 1, t.z);}
        if (action == 3) {agent.transform.position = new Vector3(t.x - 1, t.y, t.z);}
        
    }

    void FixedUpdate() {
        //agent.transform.position = Vector3.Lerp(agent.transform.position, destinationVector, Time.deltaTime * agentSpeed);
    }
    


    IEnumerator StartSolveMonteCarlo(Vector2 start, Vector2 end) {
        while (gameObject.transform.parent.GetComponent<AgentManager>().trialNumber < maxTrials) {
            gameObject.transform.position = gameObject.transform.parent.GetComponent<AgentManager>().startingPosition;
            destinationVector = gameObject.transform.parent.GetComponent<AgentManager>().startingPosition;
            discountConstant = gameObject.transform.parent.GetComponent<AgentManager>().discountConstant;
            int k = 0;
            stateActionPairs = new List<Vector3>();

            while ((agentX != end.x || agentY != end.y) && k < maxSteps && rewardValue > rewardFloorConstant * dimension * dimension) {

                agentX = (int) (Mathf.Floor(agent.transform.position.x));
                agentY = (int) (Mathf.Floor(agent.transform.position.y));

                //int action = Random.Range(0, 4);
                int action = chooseAction(agentX, agentY);

                GameObject currentTile = tiles[agentX, agentY];
                GameObject[] neighbors = currentTile.GetComponent<TileData>().neighbors; //get neighbors
                GameObject[] walls = currentTile.GetComponent<TileData>().walls; //get walls
                GameObject destinationTile = neighbors[action];

                if (CheckActionHitWall(action, currentTile, walls)) {
                    rewardValue += rewardWallHit;
                } 
                
                else if (CheckActionOutOfBounds(action, currentTile)) {
                    rewardValue += rewardMoveOutOfBounds;
                }

                else {
                    moveAgent(action);
                    //yield return new WaitForSeconds(.01f);
                    if (destinationTile.GetComponent<TileData>().visited) {
                        rewardValue += rewardMoveToVisited;
                    } else if (destinationTile.GetComponent<TileData>().coordinate.x == end.x && destinationTile.GetComponent<TileData>().coordinate.y == end.y) {
                        rewardValue += rewardReachEnd;
                        stateActionPairs.Add(new Vector3(agentX, agentY, action));
                        break;
                    }
                    else {
                        rewardValue += rewardMove;
                        destinationTile.GetComponent<TileData>().visited = true;
                    }
                }

                if (speedDrop.value == 0) {
                    yield return new WaitForSeconds(.1f);
                } else if (speedDrop.value == 1) {
                    yield return new WaitForSeconds(.01f);
                }
                
                stateActionPairs.Add(new Vector3(agentX, agentY, action));

                k++; 
            }
            discountConstant = 1;
            for (int i = 0; i < stateActionPairs.Count; i++) {
                Vector3 saPair = stateActionPairs[i];
                float currEntry = qTable[(int) saPair.x, (int) saPair.y, (int) saPair.z];
                qTable[(int) saPair.x, (int) saPair.y, (int) saPair.z] = currEntry + (1 * (1/((float) stateActionPairs.Count)) * (rewardValue - currEntry));
            }




            if (speedDrop.value == 0) {
                yield return new WaitForSeconds(.1f);
            }
            else if (speedDrop.value == 1) {
                yield return new WaitForSeconds(.01f);
            }
            else if (speedDrop.value == 2) {
                yield return new WaitForSeconds(.01f);
            }

            gameObject.transform.parent.GetComponent<AgentManager>().agentsFinished += 1;
            while (gameObject.transform.parent.GetComponent<AgentManager>().agentsFinished != 0) {
                yield return new WaitForSeconds(.02f);
            }
            

            mazeManager.GetComponent<MazeInitializer>().unvisitMaze();
            agentX = 0;
            agentY = 0;
            rewardValue = 0;
            gameObject.transform.parent.GetComponent<AgentManager>().discountConstant -= (1/(((float) maxTrials) * (numberOfAgents)));
            if (explorationConstant > explorationConstantLimit && Random.Range(0,10) == 0) {explorationConstant -= 1;}

            gameObject.transform.parent.GetComponent<AgentManager>().trialNumber++;
        }
    }




    IEnumerator StartSolveTD(Vector2 start, Vector2 end) {

        while (gameObject.transform.parent.GetComponent<AgentManager>().trialNumber < maxTrials) {
            gameObject.transform.position = gameObject.transform.parent.GetComponent<AgentManager>().startingPosition;
            destinationVector = gameObject.transform.parent.GetComponent<AgentManager>().startingPosition;
            discountConstant = gameObject.transform.parent.GetComponent<AgentManager>().discountConstant;
            int k = 0;
            stateActionPairs = new List<Vector3>();
            if (speedDrop.value == 0) {
                yield return new WaitForSeconds(.1f);
            }
            else if (speedDrop.value == 1) {
                yield return new WaitForSeconds(.01f);
            }
            else if (speedDrop.value == 2) {
                yield return new WaitForSeconds(.01f);
            }

            while ((agentX != end.x || agentY != end.y) && k < maxSteps && rewardValue > rewardFloorConstant * dimension * dimension) {
                

                agentX = (int) (Mathf.Floor(agent.transform.position.x));
                agentY = (int) (Mathf.Floor(agent.transform.position.y));

                if (speedDrop.value == 0) {
                    yield return new WaitForSeconds(.1f);
                } else if (speedDrop.value == 1) {
                    yield return new WaitForSeconds(.01f);
                }

                //int action = Random.Range(0, 4);
                int action = chooseAction(agentX, agentY);

                GameObject currentTile = tiles[agentX, agentY];
                GameObject[] neighbors = currentTile.GetComponent<TileData>().neighbors; //get neighbors
                GameObject[] walls = currentTile.GetComponent<TileData>().walls; //get walls
                GameObject destinationTile = neighbors[action];

                float tempReward = 0;

                if (CheckActionHitWall(action, currentTile, walls)) {
                    rewardValue += rewardWallHit;
                    tempReward = rewardWallHit;
                } 
                
                else if (CheckActionOutOfBounds(action, currentTile)) {
                    rewardValue += rewardMoveOutOfBounds;
                    tempReward = rewardMoveOutOfBounds;
                }

                else {
                    moveAgent(action);
                    //yield return new WaitForSeconds(.001f);
                    if (destinationTile.GetComponent<TileData>().visited) {
                        rewardValue += rewardMoveToVisited;
                        tempReward = rewardMoveToVisited;

                    } else if (destinationTile.GetComponent<TileData>().coordinate.x == end.x && destinationTile.GetComponent<TileData>().coordinate.y == end.y) {
                        rewardValue += rewardReachEnd;
                        tempReward = rewardReachEnd;
                    }
                    else {
                        rewardValue += rewardMove;
                        tempReward = rewardMove;
                        destinationTile.GetComponent<TileData>().visited = true;
                    }
                }
                

                float currEntry = qTable[(int) agentX, (int) agentY, (int) action];

                float oneStepQValue = 0;
                if (tempReward == rewardMoveOutOfBounds || tempReward == rewardWallHit) {
                    oneStepQValue = qTable[(int) agentX, (int) agentY, 0];
                    for (int i = 1; i < 4; i++) {
                        if (qTable[(int) agentX, (int) agentY, i] > oneStepQValue) {
                            oneStepQValue = qTable[(int) agentX, (int) agentY, i];
                        }
                    }
                    qTable[(int) agentX, (int) agentY, (int) action] = currEntry + (tempReward + (discountConstant * oneStepQValue) - currEntry);
                    
                }
                else if (tempReward == rewardReachEnd) {
                    oneStepQValue = qTable[(int) agentX, (int) agentY, 0];
                    for (int i = 1; i < 4; i++) {
                        if (qTable[(int) agentX, (int) agentY, i] > oneStepQValue) {
                            oneStepQValue = qTable[(int) agentX, (int) agentY, i];
                        }
                    }
                    qTable[(int) agentX, (int) agentY, (int) action] = currEntry + (tempReward + (discountConstant * oneStepQValue) - currEntry);
                    break;
                } else {
                    if (action == 0) {
                        oneStepQValue = qTable[(int) agentX, (int) agentY + 1, 0];
                        for (int i = 1; i < 4; i++) {
                            if (qTable[(int) agentX, (int) agentY + 1, i] > oneStepQValue) {
                                oneStepQValue = qTable[(int) agentX, (int) agentY + 1, i];
                            }
                        }
                    }
                    else if (action == 1) {
                        oneStepQValue = qTable[(int) agentX + 1, (int) agentY, 0];
                        for (int i = 1; i < 4; i++) {
                            if (qTable[(int) agentX + 1, (int) agentY, i] > oneStepQValue) {
                                oneStepQValue = qTable[(int) agentX + 1, (int) agentY, i];
                            }
                        }
                    }
                    else if (action == 2) {
                        oneStepQValue = qTable[(int) agentX, (int) agentY - 1, 0];
                        for (int i = 1; i < 4; i++) {
                            if (qTable[(int) agentX, (int) agentY - 1, i] > oneStepQValue) {
                                oneStepQValue = qTable[(int) agentX, (int) agentY - 1, i];
                            }
                        }
                    }
                    else if (action == 3) {
                        oneStepQValue = qTable[(int) agentX - 1, (int) agentY, 0];
                        for (int i = 1; i < 4; i++) {
                            if (qTable[(int) agentX - 1, (int) agentY, i] > oneStepQValue) {
                                oneStepQValue = qTable[(int) agentX - 1, (int) agentY, i];
                            }
                        }
                    }

                    qTable[(int) agentX, (int) agentY, (int) action] = currEntry + (tempReward + (discountConstant * oneStepQValue) - currEntry);
                    
                }
                

            }




            //yield return new WaitForSeconds(.001f);


            gameObject.transform.parent.GetComponent<AgentManager>().agentsFinished += 1;
            while (gameObject.transform.parent.GetComponent<AgentManager>().agentsFinished != 0) {
                yield return new WaitForSeconds(.01f);
            }
            

            mazeManager.GetComponent<MazeInitializer>().unvisitMaze();
            agentX = 0;
            agentY = 0;
            rewardValue = 0;
            if (explorationConstant > explorationConstantLimit) {explorationConstant -= 10;}

            gameObject.transform.parent.GetComponent<AgentManager>().trialNumber++;
        }
    }


    IEnumerator StartSolveSARSA(Vector2 start, Vector2 end) {

        explorationConstant = 0;

        while (gameObject.transform.parent.GetComponent<AgentManager>().trialNumber < maxTrials) {
            gameObject.transform.position = gameObject.transform.parent.GetComponent<AgentManager>().startingPosition;
            destinationVector = gameObject.transform.parent.GetComponent<AgentManager>().startingPosition;
            discountConstant = gameObject.transform.parent.GetComponent<AgentManager>().discountConstant;
            int k = 0;
            stateActionPairs = new List<Vector3>();
            if (speedDrop.value == 0) {
                yield return new WaitForSeconds(.1f);
            }
            else if (speedDrop.value == 1) {
                yield return new WaitForSeconds(.01f);
            }
            else if (speedDrop.value == 2) {
                yield return new WaitForSeconds(.01f);
            }

            while ((agentX != end.x || agentY != end.y) && k < maxSteps && rewardValue > rewardFloorConstant * dimension * dimension) {
                

                agentX = (int) (Mathf.Floor(agent.transform.position.x));
                agentY = (int) (Mathf.Floor(agent.transform.position.y));

                if (speedDrop.value == 0) {
                    yield return new WaitForSeconds(.1f);
                } else if (speedDrop.value == 1) {
                    yield return new WaitForSeconds(.01f);
                }

                int action = chooseAction(agentX, agentY);

                GameObject currentTile = tiles[agentX, agentY];
                GameObject[] neighbors = currentTile.GetComponent<TileData>().neighbors; //get neighbors
                GameObject[] walls = currentTile.GetComponent<TileData>().walls; //get walls
                GameObject destinationTile = neighbors[action];

                float tempReward = 0;

                if (CheckActionHitWall(action, currentTile, walls)) {
                    rewardValue += rewardWallHit;
                    tempReward = rewardWallHit;
                } 
                
                else if (CheckActionOutOfBounds(action, currentTile)) {
                    rewardValue += rewardMoveOutOfBounds;
                    tempReward = rewardMoveOutOfBounds;
                }

                else {
                    moveAgent(action);
                    //yield return new WaitForSeconds(.001f);
                    if (destinationTile.GetComponent<TileData>().visited) {
                        rewardValue += rewardMoveToVisited;
                        tempReward = rewardMoveToVisited;

                    } else if (destinationTile.GetComponent<TileData>().coordinate.x == end.x && destinationTile.GetComponent<TileData>().coordinate.y == end.y) {
                        rewardValue += rewardReachEnd;
                        tempReward = rewardReachEnd;
                    }
                    else {
                        rewardValue += rewardMove;
                        tempReward = rewardMove;
                        destinationTile.GetComponent<TileData>().visited = true;
                    }
                }
                

                float currEntry = qTable[(int) agentX, (int) agentY, (int) action];

                float oneStepQValue = 0;
                if (tempReward == rewardMoveOutOfBounds || tempReward == rewardWallHit) {
                    oneStepQValue = qTable[(int) agentX, (int) agentY, 0];
                    for (int i = 1; i < 4; i++) {
                        if (qTable[(int) agentX, (int) agentY, i] > oneStepQValue) {
                            oneStepQValue = qTable[(int) agentX, (int) agentY, i];
                        }
                    }
                    qTable[(int) agentX, (int) agentY, (int) action] = currEntry + (tempReward + (discountConstant * oneStepQValue) - currEntry);
                    
                }
                else if (tempReward == rewardReachEnd) {
                    oneStepQValue = qTable[(int) agentX, (int) agentY, 0];
                    for (int i = 1; i < 4; i++) {
                        if (qTable[(int) agentX, (int) agentY, i] > oneStepQValue) {
                            oneStepQValue = qTable[(int) agentX, (int) agentY, i];
                        }
                    }
                    qTable[(int) agentX, (int) agentY, (int) action] = currEntry + (tempReward + (discountConstant * oneStepQValue) - currEntry);
                    break;
                } else {
                    if (action == 0) {
                        oneStepQValue = qTable[(int) agentX, (int) agentY + 1, 0];
                        for (int i = 1; i < 4; i++) {
                            if (qTable[(int) agentX, (int) agentY + 1, i] > oneStepQValue) {
                                oneStepQValue = qTable[(int) agentX, (int) agentY + 1, i];
                            }
                        }
                    }
                    else if (action == 1) {
                        oneStepQValue = qTable[(int) agentX + 1, (int) agentY, 0];
                        for (int i = 1; i < 4; i++) {
                            if (qTable[(int) agentX + 1, (int) agentY, i] > oneStepQValue) {
                                oneStepQValue = qTable[(int) agentX + 1, (int) agentY, i];
                            }
                        }
                    }
                    else if (action == 2) {
                        oneStepQValue = qTable[(int) agentX, (int) agentY - 1, 0];
                        for (int i = 1; i < 4; i++) {
                            if (qTable[(int) agentX, (int) agentY - 1, i] > oneStepQValue) {
                                oneStepQValue = qTable[(int) agentX, (int) agentY - 1, i];
                            }
                        }
                    }
                    else if (action == 3) {
                        oneStepQValue = qTable[(int) agentX - 1, (int) agentY, 0];
                        for (int i = 1; i < 4; i++) {
                            if (qTable[(int) agentX - 1, (int) agentY, i] > oneStepQValue) {
                                oneStepQValue = qTable[(int) agentX - 1, (int) agentY, i];
                            }
                        }
                    }

                    qTable[(int) agentX, (int) agentY, (int) action] = currEntry + (tempReward + (discountConstant * oneStepQValue) - currEntry);
                    
                }
                

            }




            //yield return new WaitForSeconds(.001f);


            gameObject.transform.parent.GetComponent<AgentManager>().agentsFinished += 1;
            while (gameObject.transform.parent.GetComponent<AgentManager>().agentsFinished != 0) {
                yield return new WaitForSeconds(.01f);
            }
            

            mazeManager.GetComponent<MazeInitializer>().unvisitMaze();
            agentX = 0;
            agentY = 0;
            rewardValue = 0;
            if (explorationConstant > explorationConstantLimit) {explorationConstant -= 10;}

            gameObject.transform.parent.GetComponent<AgentManager>().trialNumber++;
        }
    }

    private int chooseAction(int x, int y) {
        int r = Random.Range(0, 100);
        if (r < explorationConstant) {
            return Random.Range(0, 4);
        }

        float qTableValueMax = qTable[x,y,0];
        int action = 0;
        for (int i = 1; i < 4; i++) {
            if (qTableValueMax < qTable[x,y,i]) {
                qTableValueMax = qTable[x,y,i];
                action = i;
            } else if (qTableValueMax == qTable[x,y,i]){
                return Random.Range(0, 4);
            }
        }
        return action;    
    }
    
    

    

}
