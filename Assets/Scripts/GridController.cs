﻿using UnityEngine;
using System.Collections;

using DigitalRuby.Tween;

public enum WallDirection
{
    xAxis, zAxis
}

public class GridController : MonoBehaviour {

    public GameObject gridBlock;

    public GameObject testDude;

    private GameObject[] grid;

    public Vector2 gridSize = new Vector2(40, 40);

    public bool[] dudeSpawns;

    public float gridSpacing = 3.0f;

    public float spawnHeight = 60.0f;

    private int gridWidth;
    private int gridHeight;

    public float brickSpawnInterval = 1.0f;

    private Collider groundCollider;
    public GameObject markerObject;
    public GameObject ghostWall;

    private bool showingMarker;

    private int lastX;
    private int lastZ;

    private WallDirection selectedDirection = WallDirection.xAxis;

    // Use this for initialization
    void Start () {

        groundCollider = GameObject.Find("Ground").GetComponent<Collider>();

        markerObject.transform.parent = gameObject.transform;
        markerObject.SetActive(false);

        ghostWall.transform.parent = gameObject.transform;
        ghostWall.SetActive(false);

        gridWidth = (int)gridSize.x;
        gridHeight = (int)gridSize.y;

        gameObject.transform.position = new Vector3(-gridSize.x / 2 + 0.5f, 0.0f, -gridSize.y / 2 + 0.5f);

        grid = new GameObject[gridWidth * gridHeight];

        // For preventing spawing on top of each other
        dudeSpawns = new bool[gridWidth * gridHeight];
        

        if (testDude != null) SpawnSomeDudes(50);

        if (gridBlock == null) return;

        //StartCoroutine(SpawnSomeWalls(10, 2.0f));
        //SpawnAllRandom();

    }

    void SpawnSomeDudes(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnRandomDude();
        }
    }

    void SpawnRandomDude()
    {
        int randX = Random.Range(0, gridWidth);
        int randZ = Random.Range(0, gridHeight);

        SpawnDude(randX, randZ);
    }

    void SpawnRandomWall()
    {
        float rand = Random.Range(0.0f, 1.0f);
        WallDirection dir = rand < 0.5f ? WallDirection.xAxis : WallDirection.zAxis;

        int randX = Random.Range(0, gridWidth);
        int randZ = Random.Range(0, gridHeight);

        SpawnWall(randX, randZ, dir);
    }

    IEnumerator SpawnSomeWalls(int count, float waitTime)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnRandomWall();
            yield return new WaitForSeconds(waitTime);
        }
    }

    void SpawnAllRandom()
    {
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                SpawnBrick(j, i, 5.0f, true);
            }
        }
    }

    void SpawnDude (int x, int z)
    {
        GameObject Dude = (GameObject)Instantiate(testDude);
        Dude.transform.parent = gameObject.transform;
        // Unity is Z-forward
        float initialY = 1.4f;
        //float initialY = 40.0f;
        Dude.transform.localPosition = GetLocalPosFromGridPos(x, z, initialY);
        //Dude.transform.localPosition = new Vector3(gridSpacing * x, initialY, gridSpacing * z);
        Dude.SetActive(true);
    }

    void SpawnBrick (int x, int z, float delay, bool randomDelay)
    {
        int index = gridWidth * z + x;
        //Debug.Log(string.Format("X: {0}, Z: {1}, Index: {2}", x, z, index));
        grid[index] = (GameObject)Instantiate(gridBlock);
        grid[index].SetActive(true);
        grid[index].transform.parent = gameObject.transform;
        // Unity is Z-forward
        //float initialY = 40.0f - (z) / 3.0f;
        float initialY = spawnHeight;
        grid[index].transform.localPosition = GetLocalPosFromGridPos(x, z, initialY);
        grid[index].GetComponent<BlockController>().initialY = initialY;
        grid[index].GetComponent<BlockController>().Spawn(delay, randomDelay);
        //grid[index].SetActive(true);
    }

    void SpawnWall (int startX, int startZ, WallDirection wallDirection)
    {
        if (!IsOnGrid(startX, startZ)) return;
        if (wallDirection == WallDirection.xAxis)
        {
            // X forward
            for (int i = startX; i < gridWidth; i++)
            {
                int index = gridWidth * startZ + i;
                // Stop on block
                if (grid[index] != null) break;

                float delayFactor = i - startX;

                SpawnBrick(i, startZ, brickSpawnInterval * delayFactor, false);
            }
            if (startX - 1 >= 0)
            {
                // X backward
                for (int j = startX - 1; j >= 0; j--)
                {
                    int index = gridWidth * startZ + j;
                    // Stop on block
                    if (grid[index] != null) break;

                    float delayFactor = (startX - j) + 1.0f;
                    SpawnBrick(j, startZ, brickSpawnInterval * delayFactor, false);
                }
            }
        } else
        {
            // Z forward
            for (int k = startZ; k < gridWidth; k++)
            {
                int index = gridWidth * k + startX;
                // Stop on block
                if (grid[index] != null) break;

                float delayFactor = k - startX;

                SpawnBrick(startX, k, brickSpawnInterval * delayFactor, false);
            }
            if (startZ - 1 >= 0)
            {
                // Z backward
                for (int l = startZ - 1; l >= 0; l--)
                {
                    int index = gridWidth * l + startX;
                    // Stop on block
                    if (grid[index] != null) break;

                    float delayFactor = (startZ - l) + 1.0f;
                    SpawnBrick(startX, l, brickSpawnInterval * delayFactor, false);
                }
            }
        }
    }

    Vector3 GetGridPositionFromLocalPos(Vector3 localPos)
    {
        return new Vector3(Mathf.Round(localPos.x / gridSpacing), 0.0f, Mathf.Round(localPos.z / gridSpacing));
    }

    Vector3 GetGridPositionFromWorldPos(Vector3 worldPos)
    {
        Vector3 localPos = gameObject.transform.InverseTransformPoint(worldPos);
        return GetGridPositionFromLocalPos(localPos);
    }

    Vector3 GetLocalPosFromGridPos(int gridX, int gridZ, float y = 0.0f)
    {
        return new Vector3(gridSpacing * gridX, y, gridSpacing * gridZ);
    }

    Vector3 GetWorldPosFromGridPos(int gridX, int gridZ, float y = 0.0f)
    {
        Vector3 localPos = GetLocalPosFromGridPos(gridX, gridZ, y);
        return gameObject.transform.TransformPoint(localPos);
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (groundCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 gridPos = GetGridPositionFromWorldPos(hit.point);
                Vector3 markerPos = markerObject.transform.position;


                lastX = (int)gridPos.x;
                lastZ = (int)gridPos.z;

                ShowMarker(lastX, lastZ);

            }
        } else if (showingMarker)
        {
            HideMarker();
            SpawnWall(lastX, lastZ, selectedDirection);
            lastX = -1;
            lastZ = -1;
        }
    }

    bool IsOnGrid(int x, int z)
    {
        if (x < 0 || z < 0 || x > gridWidth - 1 || z > gridHeight - 1) return false;
        return true;
    }

    void HideGhostWall()
    {
        ghostWall.SetActive(false);
    }

    void ShowGhostWall(int x, int z, WallDirection dir)
    {
        if (dir == WallDirection.zAxis)
        {
            ghostWall.transform.localPosition = GetLocalPosFromGridPos(x, gridHeight / 2, 0.5f);
            ghostWall.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        }
        else
        {
            ghostWall.transform.localPosition = GetLocalPosFromGridPos(gridWidth / 2, z, 0.5f);
            ghostWall.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }

        ghostWall.SetActive(true);
    }

    void HideMarker()
    {
        markerObject.SetActive(false);
        showingMarker = false;
        HideGhostWall();
    }

    void ShowMarker(int x, int z)
    {
        if (IsOnGrid(x, z))
        {
            showingMarker = true;
            markerObject.SetActive(true);
            markerObject.transform.localPosition = GetLocalPosFromGridPos(x, z, 1.0f);
            ShowGhostWall(x, z, selectedDirection);
        }
        else
        {
            HideMarker();
        }
    }

    public void ToggleDirection ()
    {
        if (selectedDirection == WallDirection.xAxis)
        {
            selectedDirection = WallDirection.zAxis;
        }
        else
        {
            selectedDirection = WallDirection.xAxis;
        }
    }
}
