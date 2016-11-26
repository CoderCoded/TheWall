using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelClearedAlgorithm : MonoBehaviour {

    private int[] grid;
    private Vector2 gridSize = new Vector2(7, 4);
    private int gridWidth;
    private int gridHeight;
    public int ground = 0;
    public int wall = 1;
    private List<int> visited;
    private List<List<int>> spans;

    // Use this for initialization
    void Start () {
        gridWidth = (int)gridSize.x;
        gridHeight = (int)gridSize.y;
        grid = new int[] {
            0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,
            1,1,1,1,0,0,0
        };
        spans = getSpans(grid);
        List<List<int>> areas = new List<List<int>>();
        visited = new List<int>();
        for (int i = 0; i < spans.Count; i++) {
            if (!visited.Contains(i)) {
                areas.Add(recursiveMerge(i));
            }
        }
        Debug.Log(areas);
    }   
	
	// Update is called once per frame
	void Update () {
	
	}

    List<int> recursiveMerge(int index) {
        // create neighbour indexes for comparison
        List<int> area = new List<int>();
        if (visited.Contains(index)) {
            return null;
        }

        for (int i = 0; i < spans[index].Count; i++) {
            area.Add(spans[index][i]);
            visited.Add(index);
        }

        for (int i = 0; i < spans[index].Count; i++) {
            // add every span to area that has this span's neighbours
            int rightNeighbour = spans[index][i] + 1;
            int upNeighbour = spans[index][i] - gridWidth;
            int downNeighbour = spans[index][i] + gridWidth;

            for (int v = 0; v < spans.Count; v++) {
                if ((spans[v].Contains(rightNeighbour) && (rightNeighbour % gridWidth != 0)) || (spans[v].Contains(upNeighbour)) || (spans[v].Contains(downNeighbour) && downNeighbour < (gridWidth * gridHeight))) {
                    List<int> res = recursiveMerge(v);
                    if (res != null) {
                        for (int x = 0; x < res.Count; x++) {
                            area.Add(res[x]);
                        }
                    }
                }
            }
        }

        return area;
    }

    // Creates lists of connected areas per row
    // one list = one connected area in row
    List<List<int>> getSpans(int[] grid) {

        List<List<int>> spans = new List<List<int>>();
        List<int> span = new List<int>();
        bool inseq = false;

        for (int y = 0; y < gridHeight; y++) {
            // reset span for every row
            if (inseq) { spans.Add(span); }
            inseq = false;
            for (int x = 0; x < gridWidth; x++) {
                int index = x + y * gridWidth;

                if (grid[index] == ground) {
                    // create new span as new ground was discovered
                    if (!inseq) {
                        span = new List<int>();
                    }
                    // add ground's index to span
                    span.Add(index);
                    inseq = true;
                } else {
                    // wall or grave or something
                    if (inseq) {
                        // last piece was ground -> store the span
                        spans.Add(span);
                        inseq = false;
                    } else {
                        // just ignore it
                    }
                }
            }
        }
        return spans;
    }
}
