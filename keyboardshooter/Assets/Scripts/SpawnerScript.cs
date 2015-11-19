using UnityEngine;
using System.Collections;

public class SpawnerScript : MonoBehaviour {
	
	public Rigidbody charBlock;
	public Rigidbody spaceBlock;

	public GameObject topRow;
	public GameObject midRow;
	public GameObject botRow;

	private Vector3 topLeft;
	private float topSpan;

	private Vector3 midLeft;
	private float midSpan;

	private Vector3 botLeft;
	private float botSpan;

	private float lastShotTime;

	private float topCount = 9f;
	private float midCount = 8f;
	private float botCount = 6f;

	private float speed = 30f;	

	private AudioSource tone;
	private float octaves = 1f;

	private Vector3 lastPos;

	private float cBound0;
	private float cBound1;
	private float cBound2;
	private float cBound3;
	private float cBound4;
	private float cBound5;
	private float cBound6;

	private float cInterval;

	// Use this for initialization
	void Start () {
		topLeft = topRow.transform.position - topRow.transform.lossyScale.x * topRow.transform.right * 1f/2f;
		topSpan = Vector3.Distance (topLeft, topLeft + topRow.transform.lossyScale.x * topRow.transform.right);

		midLeft = midRow.transform.position - midRow.transform.lossyScale.x * midRow.transform.right * 1f/2f;
		midSpan = Vector3.Distance (midLeft, midLeft + midRow.transform.lossyScale.x * midRow.transform.right);

		botLeft = botRow.transform.position - botRow.transform.lossyScale.x * botRow.transform.right * 1f/2f;
		botSpan = Vector3.Distance (botLeft, botLeft + botRow.transform.lossyScale.x * botRow.transform.right);

		cInterval = topSpan / 6f;
		cBound0 = topLeft.x;
		cBound1 = topLeft.x + cInterval;
		cBound2 = topLeft.x + 2f * cInterval;
		cBound3 = topLeft.x + 3f * cInterval;
		cBound4 = topLeft.x + 4f * cInterval;
		cBound5 = topLeft.x + 5f * cInterval;
		cBound6 = topLeft.x + 6f * cInterval;

		tone = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown (KeyCode.Q)) {
			shootChar ("q", topLeft);
		}
		if (Input.GetKeyDown (KeyCode.W)) {
			shootChar ("w", topLeft + (topSpan * Vector3.right * 1f/topCount));
		}
		if (Input.GetKeyDown (KeyCode.E)) {
			shootChar ("e", topLeft + (topSpan * Vector3.right * 2f/topCount));
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			shootChar ("r", topLeft + (topSpan * Vector3.right * 3f/topCount));
		}
		if (Input.GetKeyDown (KeyCode.T)) {
			shootChar ("t", topLeft + (topSpan * Vector3.right * 4f/topCount));
		}
		if (Input.GetKeyDown (KeyCode.Y)) {
			shootChar ("y", topLeft + (topSpan * Vector3.right * 5f/topCount));
		}
		if (Input.GetKeyDown (KeyCode.U)) {
			shootChar ("u", topLeft + (topSpan * Vector3.right * 6f/topCount));
		}
		if (Input.GetKeyDown (KeyCode.I)) {
			shootChar ("i", topLeft + (topSpan * Vector3.right * 7f/topCount));
		}
		if (Input.GetKeyDown (KeyCode.O)) {
			shootChar ("o", topLeft + (topSpan * Vector3.right * 8f/topCount));
		}
		if (Input.GetKeyDown (KeyCode.P)) {
			shootChar ("p", topLeft + (topSpan * Vector3.right * 9f/topCount));
		}


		if (Input.GetKeyDown (KeyCode.A)) {
			shootChar ("a", midLeft);
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			shootChar ("s", midLeft + (midSpan * Vector3.right * 1f/midCount));
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			shootChar ("d", midLeft + (midSpan * Vector3.right * 2f/midCount));
		}
		if (Input.GetKeyDown (KeyCode.F)) {
			shootChar ("f", midLeft + (midSpan * Vector3.right * 3f/midCount));
		}
		if (Input.GetKeyDown (KeyCode.G)) {
			shootChar ("g", midLeft + (midSpan * Vector3.right * 4f/midCount));
		}
		if (Input.GetKeyDown (KeyCode.H)) {
			shootChar ("h", midLeft + (midSpan * Vector3.right * 5f/midCount));
		}
		if (Input.GetKeyDown (KeyCode.J)) {
			shootChar ("j", midLeft + (midSpan * Vector3.right * 6f/midCount));
		}
		if (Input.GetKeyDown (KeyCode.K)) {
			shootChar ("k", midLeft + (midSpan * Vector3.right * 7f/midCount));
		}
		if (Input.GetKeyDown (KeyCode.L)) {
			shootChar ("l", midLeft + (midSpan * Vector3.right * 8f/midCount));
		}




		if (Input.GetKeyDown (KeyCode.Z)) {
			shootChar ("z", botLeft);
		}
		if (Input.GetKeyDown (KeyCode.X)) {
			shootChar ("x", botLeft + (botSpan * Vector3.right * 1f/botCount));
		}
		if (Input.GetKeyDown (KeyCode.C)) {
			shootChar ("c", botLeft + (botSpan * Vector3.right * 2f/botCount));
		}
		if (Input.GetKeyDown (KeyCode.V)) {
			shootChar ("v", botLeft + (botSpan * Vector3.right * 3f/botCount));
		}
		if (Input.GetKeyDown (KeyCode.B)) {
			shootChar ("b", botLeft + (botSpan * Vector3.right * 4f/botCount));
		}
		if (Input.GetKeyDown (KeyCode.N)) {
			shootChar ("n", botLeft + (botSpan * Vector3.right * 5f/botCount));
		}
		if (Input.GetKeyDown (KeyCode.M)) {
			shootChar ("m", botLeft + (botSpan * Vector3.right * 6f/botCount));
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			shootChar (" ", botLeft + (botSpan * Vector3.right * 1f/2f) + Vector3.back * 5f + Vector3.down * 5f);
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
	}

	void shootChar(string id, Vector3 pos) {
		if (Time.time - lastShotTime < 0.255f) {
			//return;
		}
		Rigidbody blockClone;

		if (id == " ") {
			blockClone = (Rigidbody)(Instantiate (spaceBlock, pos, topRow.transform.rotation));
		} else {
			blockClone = (Rigidbody)(Instantiate (charBlock, pos, topRow.transform.rotation));
			lastPos = pos;
			blockClone.rotation = Random.rotation;
		}

		//tone.pitch = (Mathf.Abs(topLeft.x - lastPos.x) / topSpan * 3f) + 1f;


		float r = Mathf.Abs (topLeft.x - lastPos.x) / topSpan;
		float aPitch = Mathf.Pow (2f, r * octaves + 1);
		tone.pitch = aPitch;
		//Debug.Log (Mathf.Abs (lastPos.x - topLeft.x));
		//if(!tone.isPlaying) {
		//	tone.Play();
		//}
		if (tone.isPlaying) {
			tone.Stop();
		}
		tone.Play ();


		//GameObject blockCloneGO = (GameObject)(blockClone);
		Color ech = computeColor (lastPos.x);



		blockClone.GetComponent<Renderer> ().materials [0].color = ech;

		blockClone.angularVelocity = new Vector3 (Random.value * 5f - 2.5f, Random.value * 5f - 2.5f, Random.value * 5f - 2.5f);
		blockClone.velocity = new Vector3 (0f, 0f, speed);//Vector3.forward * speed;

		//blockClone.velocity = new Vector3 (0f, speed, 0f);
		lastShotTime = Time.time;

	}

	Color computeColor (float xVal) {
		float r;
		float g;
		float b;

		if (cBound0 <= xVal && xVal < cBound1) {
			r = 1f;
			g = (xVal - cBound0) / (cBound1 - cBound0);
			b = 0f;
		}
		else if (cBound1 <= xVal && xVal < cBound2) {
			r = 1f - (xVal - cBound1) / (cBound2 - cBound1);
			g = 1f;
			b = 0f;
		}
		else if (cBound2 <= xVal && xVal < cBound3) {
			r = 0f;
			g = 1f;
			b = (xVal - cBound2) / (cBound3 - cBound2);
		}
		else if (cBound3 <= xVal && xVal < cBound4) {
			r = 0f;
			g = 1f - (xVal - cBound3) / (cBound4 - cBound3);
			b = 1f;
		}
		else if (cBound4 <= xVal && xVal < cBound5) {
			r = (xVal - cBound4) / (cBound5 - cBound4);
			g = 0f;
			b = 1f;
		}
		else if (cBound5 <= xVal && xVal < cBound6) {
			r = 1f;
			g = 0;
			b = 1f - (xVal - cBound5) / (cBound6 - cBound5);
		}
		else {
			r = 1f;
			g = 0f;
			b = 0f;
		}
		return new Color(r, g, b);
		
	}
}
