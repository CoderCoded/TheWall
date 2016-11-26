using UnityEngine;
using System.Collections;

using DigitalRuby.Tween;

public class BlockController : MonoBehaviour {

    private Vector3 initialPos;

    public float initialY;

	// Use this for initialization
	void Start () {

    }

    // Called from GridController
    public void Spawn(float delay, bool randomDelay)
    {
        if (randomDelay) StartCoroutine(WaitAndSpawn(Random.Range(0.0f, delay)));
        else StartCoroutine(WaitAndSpawn(delay));
    }

    // Update is called once per frame
    void Update () {
	
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
              //Debug.Log("Finished move.");
          });
    }

}
