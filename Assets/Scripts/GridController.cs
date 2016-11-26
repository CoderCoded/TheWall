using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using DigitalRuby.Tween;

public enum WallDirection
{
    xAxis, zAxis
}

public class GridController : MonoBehaviour {

    public GameObject gridBlock;

    public GameObject testDude;

    public GameObject graveStone;

    private GameObject[] grid;

    private List<GameObject> dudes;

    public Vector2 gridSize = new Vector2(40, 40);

    public float gridSpacing = 3.0f;

    public float spawnHeight = 60.0f;

    private int gridWidth;
    private int gridHeight;

    public float BlockSpawnInterval = 1.0f;

    private Collider groundCollider;
    public GameObject markerObject;
    public GameObject ghostWall;

    private bool showingMarker;

    private int lastX;
    private int lastZ;

    private float minKillDistance = 1.0f;

    private bool explosionOnGoing = false;

    public float explosionRadius = 5.0f;

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

        dudes = new List<GameObject>();

        if (testDude != null) SpawnSomeDudes(50);

        if (gridBlock == null) return;

        //StartCoroutine(SpawnSomeWalls(10, 2.0f));
        //SpawnAllBlocksRandom();

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

    void DestroyBlocksOnRadius(int x, int z, float radius)
    {
        foreach (Transform t in gameObject.transform)
        {
            // all children
            if (t.gameObject.GetComponent<BlockController>())
            {
                if (Vector3.Distance(t.localPosition, GetLocalPosFromGridPos(x, z)) < radius)
                {
                    // Destroy the block
                    Destroy(t.gameObject);
                }
            }
        }
    }

    void SpawnAllBlocksRandom()
    {
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                SpawnBlock(j, i, 5.0f, true);
            }
        }
    }

    void SpawnDude (int x, int z)
    {
        GameObject dude = (GameObject)Instantiate(testDude);
        dude.transform.parent = gameObject.transform;
        // Unity is Z-forward
        float initialY = 1.4f;
        //float initialY = 40.0f;
        Vector3 startPos = GetLocalPosFromGridPos(x, z, initialY);

        foreach (GameObject oldDude in dudes)
        {
            float dist = Vector3.Distance(oldDude.GetComponent<DudeController>().startPosition, startPos);
            if (dist < 1.0f)
            {
                // Just destroy the duplicate for now
                Destroy(dude);
                return;
            }
        }

        dude.transform.localPosition = startPos;
        //Dude.transform.localPosition = new Vector3(gridSpacing * x, initialY, gridSpacing * z);
        dude.GetComponent<DudeController>().startPosition = startPos;
        dude.SetActive(true);
        dudes.Add(dude);
    }

    void SpawnBlock (int x, int z, float delay, bool randomDelay)
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
        BlockController blockController = grid[index].GetComponent<BlockController>();
        blockController.x = x;
        blockController.z = z;
        blockController.initialY = initialY;
        grid[index].GetComponent<BlockController>().Spawn(delay, randomDelay);
        //grid[index].SetActive(true);
    }

    void SpawnGraveStone (GameObject block, GameObject dude)
    {

        int x = block.GetComponent<BlockController>().x;
        int z = block.GetComponent<BlockController>().z;

        int index = gridWidth * z + x;

        if (grid[index] == null) return;

        Destroy(grid[index]);

        grid[index] = (GameObject)Instantiate(graveStone);
        grid[index].SetActive(true);
        grid[index].transform.parent = gameObject.transform;

        // From ground
        float initialY = -3.0f;
        grid[index].transform.localPosition = GetLocalPosFromGridPos(x, z, initialY);

        GraveStoneController graveStoneController = grid[index].GetComponent<GraveStoneController>();
        graveStoneController.x = x;
        graveStoneController.z = z;
        graveStoneController.initialY = initialY;
        graveStoneController.dude = dude; // For respawn, who died?
        graveStoneController.Spawn();
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

                SpawnBlock(i, startZ, BlockSpawnInterval * delayFactor, false);
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
                    SpawnBlock(j, startZ, BlockSpawnInterval * delayFactor, false);
                }
            }
        }
        else
        {
            // Z forward
            for (int k = startZ; k < gridWidth; k++)
            {
                int index = gridWidth * k + startX;
                // Stop on block
                if (grid[index] != null) break;

                float delayFactor = k - startX;

                SpawnBlock(startX, k, BlockSpawnInterval * delayFactor, false);
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
                    SpawnBlock(startX, l, BlockSpawnInterval * delayFactor, false);
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

        if (explosionOnGoing) return;

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

                if (Input.GetMouseButtonDown(0))
                {
                    GraveStoneController graveStoneController = GetGraveStoneController(lastX, lastZ);
                    if (graveStoneController != null)
                    {
                        HideMarker();
                        StartCoroutine(ExplodeGraveStoneAndRespawnDude(graveStoneController));
                    }
                }
                else
                {
                    ShowMarker(lastX, lastZ);
                }

            }
        } else if (showingMarker)
        {
            HideMarker();
            SpawnWall(lastX, lastZ, selectedDirection);
            lastX = -1;
            lastZ = -1;
        }
    }

    IEnumerator ExplodeGraveStoneAndRespawnDude(GraveStoneController graveStoneController)
    {
        explosionOnGoing = true;
        GameObject dude = graveStoneController.dude;
        graveStoneController.Explode();
        DestroyBlocksOnRadius(graveStoneController.x, graveStoneController.z, explosionRadius);
        yield return new WaitForSeconds(2.0f);
        dude.GetComponent<DudeController>().Respawn();
        yield return new WaitForSeconds(1.0f);
        explosionOnGoing = false;
    }

    GraveStoneController GetGraveStoneController(int x, int z)
    {
        int index = gridWidth * z + x;
        if (grid[index] == null) return null;
        return grid[index].GetComponent<GraveStoneController>();
    }

    bool IsOnGrid(int x, int z)
    {
        if (x < 0 || z < 0 || x > gridWidth - 1 || z > gridHeight - 1) return false;
        return true;
    }

    public void OnBlockLanded(GameObject block)
    {
        // Check if we hit some dude
        foreach (GameObject dude in dudes)
        {
            // skip dead
            if (!dude.activeSelf) continue;

            float dist = Vector3.Distance(dude.transform.position, block.transform.position);
            if (dist < minKillDistance)
            {
                dude.GetComponent<DudeController>().Kill();
                SpawnGraveStone(block, dude);
            }
        }
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
