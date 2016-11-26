using UnityEngine;
using System.Collections;

public class CapsuleController : MonoBehaviour {

    public GameObject bloodSplatter;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

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
}
