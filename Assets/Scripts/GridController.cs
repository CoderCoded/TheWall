using UnityEngine;
using System.Collections;

using DigitalRuby.Tween;

public enum WallDirection
{
    xAxis, zAxis
}

public class GridController : MonoBehaviour {

    public GameObject gridBlock;

    public GameObject testCapsule;

    private GameObject[] grid;

    public Vector2 gridSize = new Vector2(40, 40);

    public float gridSpacing = 3.0f;

    private int gridWidth;
    private int gridHeight;

    public float brickSpawnInterval = 1.0f;

    // Use this for initialization
    void Start () {
        gridWidth = (int)gridSize.x;
        gridHeight = (int)gridSize.y;

        gameObject.transform.position = new Vector3(-gridSize.x / 2 + 0.5f, 0.0f, -gridSize.y / 2 + 0.5f);

        grid = new GameObject[gridWidth * gridHeight];

        if (testCapsule != null) SpawnSomeCapsules(50);

        if (gridBlock == null) return;

        StartCoroutine(SpawnSomeWalls(10, 2.0f));
        //SpawnAllRandom();

    }

    void SpawnSomeCapsules(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnRandomCapsule();
        }
    }

    void SpawnRandomCapsule()
    {
        int randX = Random.Range(0, gridWidth);
        int randZ = Random.Range(0, gridHeight);

        SpawnCapsule(randX, randZ);
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

    void SpawnCapsule (int x, int z)
    {
        GameObject capsule = (GameObject)Instantiate(testCapsule);
        capsule.transform.parent = gameObject.transform;
        // Unity is Z-forward
        float initialY = 1.4f;
        //float initialY = 40.0f;
        capsule.transform.localPosition = new Vector3(gridSpacing * x, initialY, gridSpacing * z);
        capsule.SetActive(true);
    }

    void SpawnBrick (int x, int z, float delay, bool randomDelay)
    {
        int index = gridWidth * z + x;
        //Debug.Log(string.Format("X: {0}, Z: {1}, Index: {2}", x, z, index));
        grid[index] = (GameObject)Instantiate(gridBlock);
        grid[index].transform.parent = gameObject.transform;
        // Unity is Z-forward
        float initialY = 40.0f - (x + z) / 3.0f;
        //float initialY = 40.0f;
        grid[index].transform.localPosition = new Vector3(gridSpacing * x, initialY, gridSpacing * z);
        grid[index].GetComponent<BlockController>().initialY = initialY;
        grid[index].SetActive(true);
        grid[index].GetComponent<BlockController>().Spawn(delay, randomDelay);
    }

    void SpawnWall (int startX, int startZ, WallDirection wallDirection)
    {
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

    // Update is called once per frame
    void Update () {
	
	}
}
