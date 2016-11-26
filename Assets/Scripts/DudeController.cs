using UnityEngine;
using System.Collections;

public class DudeController : MonoBehaviour {

    public GameObject bloodSplatter;

    public Vector3 startPosition { get; set; }
    private Vector3 deathPosition;

    public Material[] materials;

    public int dudeType;

	// Use this for initialization
	void Start () {
        //SetDudeType(0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetRandomType(int maxType)
    {
        //Debug.Log(string.Format("Got maxtype {0}", maxType));
        int randType = (int)Random.Range(0.0f, (float)(maxType));
        SetDudeType(randType);
    }

    public void SetDudeType(int type)
    {
        Debug.Log(string.Format("Setting dudetype: {0}", type));
        dudeType = type;
        GetComponent<Renderer>().material = materials[dudeType];
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
