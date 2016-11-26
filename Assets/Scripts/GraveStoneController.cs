using UnityEngine;
using System.Collections;


using DigitalRuby.Tween;

public class GraveStoneController : MonoBehaviour {

    public float initialY;

    private Vector3 initialPos;

    public GameObject explosion;

    public GameObject dude;

    public int x;
    public int z;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Spawn()
    {
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
              //gridController.OnBlockLanded(gameObject);
          });
    }

    public void Explode()
    {
        GameObject exp = (GameObject)Instantiate(explosion, gameObject.transform.position, explosion.transform.rotation);
        //splatter.transform.parent = gameObject.transform;
        exp.SetActive(true);

        ParticleSystem expPS = exp.GetComponent<ParticleSystem>();

        float duration = expPS.duration;


        Destroy(exp, 5.0f);
        Destroy(gameObject, duration  / 2);
    }

}
