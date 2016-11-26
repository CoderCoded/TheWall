using UnityEngine;
using System.Collections;

public class DudeController : MonoBehaviour {

    public GameObject bloodSplatter;

    public Vector3 startPosition { get; set; }
    private Vector3 deathPosition;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /*
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "WallBlock")
        {
            Debug.Log("Splat");

            ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

            Debug.DrawRay(pos, contact.normal * 10.0f, Color.green, 2.0f);

            GameObject splatter = (GameObject) Instantiate(bloodSplatter, pos, rot);
            splatter.transform.parent = gameObject.transform;
            splatter.SetActive(true);
            float duration = splatter.GetComponent<ParticleSystem>().duration;
            Destroy(splatter, duration);
        }
    }
    */

    public void Kill ()
    {
        GameObject splatter = (GameObject)Instantiate(bloodSplatter, gameObject.transform.position, bloodSplatter.transform.rotation);
        //splatter.transform.parent = gameObject.transform;
        splatter.SetActive(true);

        ParticleSystem splatterPS = splatter.GetComponent<ParticleSystem>();

        float duration = splatterPS.duration;

        Destroy(splatter, duration * 3);
        gameObject.SetActive(false);
    }

    // The RESURRECTION
    public void Respawn()
    {
        gameObject.SetActive(true);
    }
}
