using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using TMPro;

public class MazeInitializer : MonoBehaviour
{
    public GameObject tileHolder;
    public GameObject mazeTile;
    public int dimension = 4;
    public GameObject[,] tiles;
    public float time;
    public List<GameObject> orderAdded;
    public Color green;
    public Color red;
    public Color blue;
    public Color white;
    public Vector2 start;
    public Vector2 end;
    public List<GameObject> primsPath;
    public List<GameObject> primsNeighbors;
    public List<GameObject> borderTiles;

    public TMPro.TMP_Dropdown algoDrop;
    public Camera camera;
    public GameObject agentManager;
    public Canvas canvas;
    public GameObject qTable;
    
    private bool speedUp;

    public int[,] solveCode;
    
    public void Start()
    {
        speedUp = false;
        Debug.Log(dimension);
        solveCode = new int[dimension, dimension];
        start = new Vector2(0, 0);
        end = new Vector2(dimension - 1, dimension - 1);
        red = new Color (255, 0, 0);
        green = new Color (0, 255, 0);
        blue = new Color (0, 0, 255);
        white = new Color (255, 255, 255);
        tiles = new GameObject[dimension, dimension];
        orderAdded = new List<GameObject>();
        for (int x = 0; x < dimension; x++) {
            for (int y = 0; y < dimension; y++){
                GameObject currentTile = Instantiate(mazeTile, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                tiles[x, y] = currentTile;
                currentTile.name = "MazeTile" + "[" + x.ToString() + ", " + y.ToString() + "]";
                currentTile.transform.SetParent(tileHolder.transform);

                currentTile.GetComponent<TileData>().coordinate = new Vector2(x, y);
                currentTile.GetComponent<TileData>().CreateWalls(currentTile);
            }
        }

        for (int x = 0; x < dimension; x++) {
            for (int y = 0; y < dimension; y++){
                GameObject currentTile = tiles[x, y];
                SetNeighbors(currentTile, currentTile.GetComponent<TileData>().coordinate);
            }
        }
        
        
        
        
        
    }

    public void StartMaze() {
        if (algoDrop.value == 0) {
            StartCoroutine(CreateMazeDFS(start, end));
        }
        if (algoDrop.value == 1) {
            StartCoroutine(CreateMazePrims(start, end));
        }
        if (algoDrop.value == 2) {
            StartCoroutine(CreateMazeHuntKill(start, end));
        }
        if (algoDrop.value == 3) {
            StartCoroutine(CreateMazeBinary(start, end));
        }

        
    }


    public void ResetMaze() {
        primsPath.Clear();
        primsNeighbors.Clear();
        borderTiles.Clear();
        orderAdded.Clear();

        for (int i = 0; i < dimension; i++) {
            for (int j = 0; j < dimension; j++) {
                Destroy(tiles[i, j]);
            }
        }
    }

    public void ResetMazeAI() {
        for (int i = 0; i < dimension; i++) {
            for (int j = 0; j < dimension; j++) {
                solveCode[i,j] = 0;
            }
        }
    }

    public void ReadStringInput(string dimensionInput) {
        Debug.Log(dimensionInput);
        ResetMaze();
        canvas.GetComponent<CanvasPositioner>().ResetCanvas();
        agentManager.GetComponent<AgentManager>().ResetAI();
        dimension = int.Parse(dimensionInput);
        Start();
        camera.GetComponent<CameraPositioner>().Start();
        agentManager.GetComponent<AgentManager>().Start();
        qTable.GetComponent<QTable>().Start();
        canvas.GetComponent<CanvasPositioner>().Start();

    }

    public void SkipAnimation() {
        speedUp = true;
    }



///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    void SetNeighbors(GameObject currentTile, Vector2 coordinate){
        GameObject[] neighbors = new GameObject[4];
        int x = (int) coordinate.x;
        int y = (int) coordinate.y;
        if (y != dimension - 1) {
            neighbors[0] = tiles[x, y + 1];
        }
        if (coordinate.x != dimension - 1) {
            neighbors[1] = tiles[x + 1, y];
        }
        if (coordinate.y != 0) {
            neighbors[2] = tiles[x, y - 1];
        }
        if (coordinate.x != 0) {
            neighbors[3] = tiles[x - 1, y];
        }
        currentTile.GetComponent<TileData>().neighbors = neighbors;
    } 



    void DestroyWall (GameObject[] walls, int direction, GameObject destinationTile) {

        int oppositeDirection = 2 + direction;
        if (oppositeDirection > 3) {oppositeDirection -= 4;}//choose opposite direction

        GameObject carvedWall = walls[direction]; //get wall
        walls[direction] = null;//remove wall from list of current tile 
        destinationTile.GetComponent<TileData>().walls[oppositeDirection] = null;//remove wall from list of destination tile
        Destroy (carvedWall); //destroy wall
    }



    List<int> GetDirections(GameObject currentTile) {
        GameObject[] neighborsCurr = currentTile.GetComponent<TileData>().neighbors;
        List<int> goodDirections = new List<int>();

        for (int i = 0; i < 4; i++) {
            if(neighborsCurr[i] != null && neighborsCurr[i].GetComponent<TileData>().visited == false) {
                goodDirections.Add(i);
            }    
        }
        return goodDirections;
    }



    int GetSourceDirection(GameObject sourceTile, GameObject currentTile) {
        int sourceToCurrDirection = 5;
        if (sourceTile.GetComponent<TileData>().coordinate.x < currentTile.GetComponent<TileData>().coordinate.x) {
            sourceToCurrDirection = 1;
        }
        if (sourceTile.GetComponent<TileData>().coordinate.x > currentTile.GetComponent<TileData>().coordinate.x) {
            sourceToCurrDirection = 3;
        }
        if (sourceTile.GetComponent<TileData>().coordinate.y < currentTile.GetComponent<TileData>().coordinate.y) {
            sourceToCurrDirection = 0;
        }
        if (sourceTile.GetComponent<TileData>().coordinate.y > currentTile.GetComponent<TileData>().coordinate.y) {
            sourceToCurrDirection = 2;
        }
        return sourceToCurrDirection;
    }



    void SetStartEndTiles (Vector2 start, Vector2 end) {
        GameObject startTile = tiles[(int) start.x, (int) start.y];
        GameObject endTile = tiles[(int) end.x, (int) end.y];
        startTile.GetComponent<SpriteRenderer>().color = new Color (0, 255, 255);
        endTile.GetComponent<SpriteRenderer>().color = new Color (255, 0, 255);
        speedUp = false;
        return; 
    }

    public void unvisitMaze () {
        for (int i = 0; i < dimension; i++) {
            for (int j = 0; j < dimension; j++) {
                tiles[i,j].GetComponent<TileData>().visited = false;
            }
        }
    }


    List<int> GetDirectionsWithWalls(GameObject currentTile) {
        GameObject[] neighborsCurr = currentTile.GetComponent<TileData>().neighbors;
        GameObject[] wallsCurr = currentTile.GetComponent<TileData>().walls;

        List<int> goodDirections = new List<int>();

        for (int i = 0; i < 4; i++) {
            if(neighborsCurr[i] != null && neighborsCurr[i].GetComponent<TileData>().visited == false && wallsCurr[i] == null) {
                goodDirections.Add(i);
            }    
        }
        return goodDirections;
    }


    private void GenerateSolveCode() {
        unvisitMaze();
        solveCode[dimension-1,dimension-1] = 0;
        GameObject currentTile = tiles[dimension - 1, dimension - 1];
        GenerateSolveCodeTile(currentTile);

    }

    private void GenerateSolveCodeTile (GameObject currentTile) {
        List<int> possibleDirections = GetDirectionsWithWalls(currentTile);
        int x = (int) currentTile.GetComponent<TileData>().coordinate.x;
        int y = (int) currentTile.GetComponent<TileData>().coordinate.y;
        currentTile.GetComponent<TileData>().visited = true;
        if (possibleDirections.Count == 0) {
            return;
        }
        for (int i = 0; i < possibleDirections.Count; i++) {
            if (possibleDirections[i] == 0) {
                solveCode[x,y+1] = 2;
                
                GenerateSolveCodeTile(tiles[x, y+1]);
            }
            if (possibleDirections[i] == 1) {
                solveCode[x+1,y] = 3;
                GenerateSolveCodeTile(tiles[x+1, y]);
            }
            if (possibleDirections[i] == 2) {
                solveCode[x,y-1] = 0;
                GenerateSolveCodeTile(tiles[x, y-1]);
            }
            if (possibleDirections[i] == 3) {
                solveCode[x-1,y] = 1;
                GenerateSolveCodeTile(tiles[x-1, y]);
            }
        }
    }



///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    IEnumerator CreateMazeDFS(Vector2 start, Vector2 end) {

        
        GameObject startTile = tiles[(int) start.x, (int) start.y];
        GameObject endTile = tiles[(int) end.x, (int) end.y];
        GameObject currentTile = startTile;
        bool atBeginning = true;
        orderAdded.Add(currentTile);
        currentTile.GetComponent<TileData>().visited = true;

        


        while ((currentTile != startTile || atBeginning)) {

            Debug.Log(speedUp);
            atBeginning = false;
            List<int> possibleDirections = GetDirections(currentTile);

            currentTile.GetComponent<SpriteRenderer>().color = new Color32(232, 49, 81, 255);
            if (possibleDirections.Count != 0) {
                GameObject[] neighbors = currentTile.GetComponent<TileData>().neighbors; //get neighbors
                GameObject[] walls = currentTile.GetComponent<TileData>().walls; //get walls

                int directionDecider = UnityEngine.Random.Range(0, possibleDirections.Count);
                int direction = possibleDirections[directionDecider];//choose direction

                GameObject destinationTile = neighbors[direction]; //choose destination

                DestroyWall(walls, direction, destinationTile); //Destroy Wall

                
                if (speedUp == false) {
                    yield return new WaitForSeconds(.001f);
                }

                destinationTile.GetComponent<TileData>().visited = true;

                currentTile.GetComponent<SpriteRenderer>().color = new Color32(255,184,222,255);

                currentTile = destinationTile;
                orderAdded.Add(currentTile);
            
            } else {
                if (speedUp == false) {
                    yield return new WaitForSeconds(.001f);
                }
                orderAdded[orderAdded.Count - 1].GetComponent<SpriteRenderer>().color = white;
                orderAdded.RemoveAt(orderAdded.Count - 1);
                currentTile = orderAdded[orderAdded.Count - 1];
            }
        } 
        SetStartEndTiles(start, end);
        GenerateSolveCode();
        unvisitMaze();
    }




    



    IEnumerator CreateMazePrims(Vector2 start, Vector2 end) {
        
        GameObject currentTile = tiles[(int) (UnityEngine.Random.Range(0,dimension)), (int) (UnityEngine.Random.Range(0,dimension))];
        
        primsNeighbors = new List<GameObject>();
        primsPath = new List<GameObject>();
        primsNeighbors.Add(currentTile);
        int c = 0;

        while ((primsNeighbors.Count != 0)) {

            int r = UnityEngine.Random.Range(0,primsNeighbors.Count);
            currentTile = primsNeighbors[r]; //choose random neighbor
            primsPath.Add(currentTile);

            if (speedUp == false) {
                yield return new WaitForSeconds(.01f);
            }

            primsNeighbors.RemoveAt(r); //move from neighbors to path
            currentTile.GetComponent<SpriteRenderer>().color = red;
            currentTile.GetComponent<TileData>().visited = true;

            GameObject[] neighbors = currentTile.GetComponent<TileData>().neighbors; //get neighbors
            GameObject[] walls = currentTile.GetComponent<TileData>().walls; //get walls

            List<GameObject> sources = new List<GameObject>();
            for (int i = 0; i < neighbors.Length; i++) {
                if (primsPath.Contains(neighbors[i])) {
                    sources.Add(neighbors[i]);
                }
            }

            if (sources.Count != 0) { //find source direction
                GameObject sourceTile = sources[UnityEngine.Random.Range(0,sources.Count)];
                int sourceToCurrDirection = GetSourceDirection(sourceTile, currentTile);
                
                sources.Clear();

                DestroyWall(sourceTile.GetComponent<TileData>().walls, sourceToCurrDirection, currentTile);
            }
            


            List<int> possibleDirections = GetDirections(currentTile);
            for (int i = 0; i < possibleDirections.Count; i++) { //add neighbors of new tile
                if (primsNeighbors.Contains(neighbors[possibleDirections[i]]) == false) {
                    primsNeighbors.Add(neighbors[possibleDirections[i]]);
                    neighbors[possibleDirections[i]].GetComponent<SpriteRenderer>().color = new Color32(100,44,169,255);
                } 
            }
            c++;
            //yield return new WaitForSeconds(.0001f);
            currentTile.GetComponent<SpriteRenderer>().color = white;

            if (primsPath.Count == (dimension*dimension)) {
                primsNeighbors.Clear();
            }

        } 
        SetStartEndTiles(start, end);
        GenerateSolveCode();
        unvisitMaze();
    }







    IEnumerator CreateMazeHuntKill(Vector2 start, Vector2 end) {

        GameObject currentTile = tiles[(int) (UnityEngine.Random.Range(0,dimension)), (int) (UnityEngine.Random.Range(0,dimension))];
        borderTiles = new List<GameObject>();
        
        bool atBeginning = true;
        int k = 0;
        while((borderTiles.Count != 0 || atBeginning)) {
            atBeginning = false;
            bool unvisitedNeighbor = true;
            int c = 0;
            while(unvisitedNeighbor) {
                currentTile.GetComponent<TileData>().visited = true;
                currentTile.GetComponent<SpriteRenderer>().color = new Color32(171, 35, 70, 255);
                List<int> possibleDirections = GetDirections(currentTile);
                if (possibleDirections.Count == 0) {
                    unvisitedNeighbor = false;
                    currentTile.GetComponent<SpriteRenderer>().color = white;
                    continue;
                }
                GameObject[] neighbors = currentTile.GetComponent<TileData>().neighbors; //get neighbors
                GameObject[] walls = currentTile.GetComponent<TileData>().walls; //get walls

                int directionDecider = UnityEngine.Random.Range(0, possibleDirections.Count);
                int direction = possibleDirections[directionDecider];//choose direction

                for(int i = 0; i < possibleDirections.Count; i++) {
                    if (i != directionDecider && borderTiles.Contains(neighbors[possibleDirections[i]]) == false) {
                        borderTiles.Add(neighbors[possibleDirections[i]]);  
                        neighbors[possibleDirections[i]].GetComponent<SpriteRenderer>().color = new Color32(3, 187, 187, 255);
                        
                    }
                }

                GameObject destinationTile = neighbors[direction];

                DestroyWall(walls, direction, destinationTile);
                if (speedUp == false) {
                    yield return new WaitForSeconds(.001f);
                }

                currentTile.GetComponent<SpriteRenderer>().color = white;
                if (borderTiles.Contains(destinationTile)) {
                    borderTiles.Remove(destinationTile);
                }
                currentTile = destinationTile;
                c++;
            }
            
            if (borderTiles.Count == 0) {
                break;
            }
            if (borderTiles.Count == 1) {
                currentTile = borderTiles[0];
                currentTile.GetComponent<SpriteRenderer>().color = white;
                GameObject[] neighbors = currentTile.GetComponent<TileData>().neighbors; //get neighbors
                GameObject[] walls = currentTile.GetComponent<TileData>().walls; //get walls

                if (currentTile.GetComponent<TileData>().coordinate.x + 1 != dimension) {
                    DestroyWall(walls, 1, neighbors[1]);
                } else if (currentTile.GetComponent<TileData>().coordinate.y + 1 != dimension) {
                    DestroyWall(walls, 0, neighbors[0]);
                } else {
                    DestroyWall(walls, 2, neighbors[2]);
                }
                break;
            }
            int r = UnityEngine.Random.Range(0, borderTiles.Count);
            GameObject borderTile = borderTiles[r];
            GameObject[] borderNeighbors = borderTile.GetComponent<TileData>().neighbors; //get neighbors
            List<GameObject> visitedBorderNeighbors = new List<GameObject>();
            for (int i = 0; i < 4; i++) {
                if (borderNeighbors[i] != null && borderNeighbors[i].GetComponent<TileData>().visited == true) {
                    visitedBorderNeighbors.Add(borderNeighbors[i]);
                }
            }
            GameObject sourceTile = visitedBorderNeighbors[UnityEngine.Random.Range(0, visitedBorderNeighbors.Count)];
            GameObject[] sourceWalls = sourceTile.GetComponent<TileData>().walls;
            int sourceToCurrDirection = GetSourceDirection(sourceTile, borderTile);

            currentTile = borderTile;
            //currentTile.GetComponent<SpriteRenderer>().color = red;
            if (speedUp == false) {
                yield return new WaitForSeconds(.001f);
            }
            
            DestroyWall(sourceWalls, sourceToCurrDirection, currentTile);
            borderTiles.RemoveAt(r);
            k++;

        }
        SetStartEndTiles(start, end);
        GenerateSolveCode();
        unvisitMaze();
    }




    IEnumerator CreateMazeBinary(Vector2 start, Vector2 end) {

        for (int i = 0; i < dimension; i++) {
            for (int j = dimension - 1; j >= 0; j--) {
                GameObject currentTile = tiles[i,j];
                currentTile.GetComponent<SpriteRenderer>().color = red;
                GameObject[] neighbors = currentTile.GetComponent<TileData>().neighbors; //get neighbors
                GameObject[] walls = currentTile.GetComponent<TileData>().walls; //get walls

                int direction = 0;
                List<int> possibleDirections = GetDirections(currentTile);
                if (possibleDirections[0] != 0 && possibleDirections[0] != 1) {
                    currentTile.GetComponent<SpriteRenderer>().color = white;
                    continue;
                }
                else if (possibleDirections[0] == 1) {
                    direction = 1;
                } else if (possibleDirections.Count == 1 || possibleDirections[1] != 1) {
                    direction = 0;
                } else {
                    direction = UnityEngine.Random.Range(0, 2);
                }

                GameObject destinationTile = neighbors[direction];
                DestroyWall(walls, direction, destinationTile);
                if (speedUp == false) {
                    yield return new WaitForSeconds(.001f);
                }

                currentTile.GetComponent<SpriteRenderer>().color = white;
            }
        }

        SetStartEndTiles(start, end);
        GenerateSolveCode();
        unvisitMaze();
    }    

}



