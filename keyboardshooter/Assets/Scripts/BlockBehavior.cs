using UnityEngine;
using System.Collections;

public class BlockBehavior : MonoBehaviour {

	private float conceptionTime;
	private float despawnTime = 4f;

	// Use this for initialization
	void Start () {
		conceptionTime = Time.time;
	}


	//uncomment this to make the cubes sticky

	void OnCollisionEnter(Collision collision) {
		if (collision.collider.tag == "Block") {
//			GetComponent<Rigidbody> ().isKinematic = true;
//			GetComponent<Collider> ().tag = "Background";
//			FixedJoint joint = this.gameObject.AddComponent <FixedJoint> ();
//			joint.connectedBody = collision.rigidbody;
			GetComponent<Rigidbody> ().useGravity = true;
		}
	}

	// Update is called once per frame
	void Update () {
		//destroy when no longer visible
		if (!GetComponent <Renderer>().isVisible ){//|| Time.time - conceptionTime > despawnTime) {
			Destroy(gameObject);
		}
	}
}
