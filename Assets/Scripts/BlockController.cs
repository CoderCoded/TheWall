using UnityEngine;
using System.Collections;

using DigitalRuby.Tween;

public class BlockController : MonoBehaviour {

    private Vector3 initialPos;

    private float spawnDelay;

    public float initialY;

    public int x;
    public int z;

    private GridController gridController;

	// Use this for initialization
	void Start () {
        gridController = GameObject.Find("Grid").GetComponent<GridController>();
    }

    void Awake ()
    {
        
    }

    // Called from GridController
    public void Spawn(float delay, bool randomDelay)
    {
        if (randomDelay) spawnDelay = Random.Range(0.0f, delay);
        else spawnDelay = delay;
        StartCoroutine(WaitAndSpawn(spawnDelay));
    }

    // Update is called once per frame
    void Update () {
        transform.position = transform.position;
    }

    private IEnumerator WaitAndSpawn(float waitTime)
    {
        GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(waitTime);
        GetComponent<MeshRenderer>().enabled = true;
        TweenSpawn();
    }

    private void TweenSpawn()
    {
        initialPos = transform.position;
        gameObject.Tween("Move" + GetInstanceID(),
          initialY,
          0.5f,
          0.3f,
          TweenScaleFunctions.Linear, (t) =>
          {
              float yPos = t.CurrentValue;
              transform.position = new Vector3(initialPos.x, yPos, initialPos.z);
          }, (t) =>
          {
              gridController.OnBlockLanded(gameObject);
          });
    }

}
