using UnityEngine;
using System.Collections;

public class BlockBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}


	//uncomment this to make the cubes sticky

//	void OnCollisionEnter(Collision collision) {
//		if (collision.collider.tag == "Background") {
//			GetComponent<Rigidbody> ().isKinematic = true;
//			GetComponent<Collider> ().tag = "Background";
//		}
//	}

	// Update is called once per frame
	void Update () {
		//destroy when no longer visible
		if (!GetComponent <Renderer>().isVisible) {
			Destroy(gameObject);
		}
	}
}
