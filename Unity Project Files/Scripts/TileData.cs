using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour
{
    public GameObject mazeWall;
    public bool visited = false;
    public Vector2 coordinate;
    public int numberOfWalls = 0;
    public GameObject[] walls;
    
    public GameObject[] neighbors;
    public GameObject[,] tiles;
    
    public void CreateWalls(GameObject MazeTile)
    {
        
        walls = new GameObject[4];
        tiles = GameObject.Find("MazeManager").GetComponent<MazeInitializer>().tiles;

        walls[0] = Instantiate(mazeWall, new Vector3(coordinate.x + .5f, coordinate.y + 1f, 0), Quaternion.Euler(0, 0, 90));
        walls[0].name = "MazeTile" + "[" + coordinate.x.ToString() + ", " + coordinate.y.ToString() + "]" + " N";
        walls[0].transform.SetParent(MazeTile.transform);

        walls[1] = Instantiate(mazeWall, new Vector3(coordinate.x + 1f, coordinate.y + .5f, 0), Quaternion.Euler(0, 0, 0));
        walls[1].name = "MazeTile" + "[" + coordinate.x.ToString() + ", " + coordinate.y.ToString() + "]" + " E";
        walls[1].transform.SetParent(MazeTile.transform);

        if (coordinate.y == 0) {
            walls[2] = Instantiate(mazeWall, new Vector3(coordinate.x + .5f, coordinate.y, 0), Quaternion.Euler(0, 0, 90));
            walls[2].name = "MazeTile" + "[" + coordinate.x.ToString() + ", " + coordinate.y.ToString() + "]" + " S";
            walls[2].transform.SetParent(MazeTile.transform);
        } else {
            walls[2] = tiles[(int) coordinate.x, (int) coordinate.y - 1].GetComponent<TileData>().walls[0];
        }
        
        if (coordinate.x == 0) {
            walls[3] = Instantiate(mazeWall, new Vector3(coordinate.x, coordinate.y + .5f, 0), Quaternion.Euler(0, 0, 0));
            walls[3].name = "MazeTile" + "[" + coordinate.x.ToString() + ", " + coordinate.y.ToString() + "]" + " W";
            walls[3].transform.SetParent(MazeTile.transform);
        } else {
            walls[3] = tiles[(int) coordinate.x - 1, (int) coordinate.y].GetComponent<TileData>().walls[1];
        }
        
    }

}
