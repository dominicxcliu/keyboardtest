using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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

	private float speed = 50f;	

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

	public Texture[] textures;

	public GameObject myNetworkThingy;
	private Performance myPerformance;

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
		myPerformance = myNetworkThingy.GetComponent<Performance> ();

	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown (KeyCode.Q)) {
			myPerformance.sendKeyPress (0, topLeft);
		}
		if (Input.GetKeyDown (KeyCode.W)) {
			myPerformance.sendKeyPress (1, topLeft + (topSpan * transform.right * 1f / topCount));
		}
		if (Input.GetKeyDown (KeyCode.E)) {
			myPerformance.sendKeyPress (2, topLeft + (topSpan * transform.right * 2f / topCount));
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			myPerformance.sendKeyPress (3, topLeft + (topSpan * transform.right * 3f / topCount));
		}
		if (Input.GetKeyDown (KeyCode.T)) {
			myPerformance.sendKeyPress (4, topLeft + (topSpan * transform.right * 4f / topCount));
		}
		if (Input.GetKeyDown (KeyCode.Y)) {
			myPerformance.sendKeyPress (5, topLeft + (topSpan * transform.right * 5f / topCount));
		}
		if (Input.GetKeyDown (KeyCode.U)) {
			myPerformance.sendKeyPress (6, topLeft + (topSpan * transform.right * 6f / topCount));
		}
		if (Input.GetKeyDown (KeyCode.I)) {
			myPerformance.sendKeyPress (7, topLeft + (topSpan * transform.right * 7f / topCount));
		}
		if (Input.GetKeyDown (KeyCode.O)) {
			myPerformance.sendKeyPress (8, topLeft + (topSpan * transform.right * 8f / topCount));
		}
		if (Input.GetKeyDown (KeyCode.P)) {
			myPerformance.sendKeyPress (9, topLeft + (topSpan * transform.right * 9f / topCount));
		}


		if (Input.GetKeyDown (KeyCode.A)) {
			myPerformance.sendKeyPress (10, midLeft);
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			myPerformance.sendKeyPress (11, midLeft + (midSpan * transform.right * 1f / midCount));
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			myPerformance.sendKeyPress (12, midLeft + (midSpan * transform.right * 2f / midCount));
		}
		if (Input.GetKeyDown (KeyCode.F)) {
			myPerformance.sendKeyPress (13, midLeft + (midSpan * transform.right * 3f / midCount));
		}
		if (Input.GetKeyDown (KeyCode.G)) {
			myPerformance.sendKeyPress (14, midLeft + (midSpan * transform.right * 4f / midCount));
		}
		if (Input.GetKeyDown (KeyCode.H)) {
			myPerformance.sendKeyPress (15, midLeft + (midSpan * transform.right * 5f / midCount));
		}
		if (Input.GetKeyDown (KeyCode.J)) {
			myPerformance.sendKeyPress (16, midLeft + (midSpan * transform.right * 6f / midCount));
		}
		if (Input.GetKeyDown (KeyCode.K)) {
			myPerformance.sendKeyPress (17, midLeft + (midSpan * transform.right * 7f / midCount));
		}
		if (Input.GetKeyDown (KeyCode.L)) {
			myPerformance.sendKeyPress (18, midLeft + (midSpan * transform.right * 8f / midCount));
		}




		if (Input.GetKeyDown (KeyCode.Z)) {
			myPerformance.sendKeyPress (19, botLeft);
		}
		if (Input.GetKeyDown (KeyCode.X)) {
			myPerformance.sendKeyPress (20, botLeft + (botSpan * transform.right * 1f / botCount));
		}
		if (Input.GetKeyDown (KeyCode.C)) {
			myPerformance.sendKeyPress (21, botLeft + (botSpan * transform.right * 2f / botCount));
		}
		if (Input.GetKeyDown (KeyCode.V)) {
			myPerformance.sendKeyPress (22, botLeft + (botSpan * transform.right * 3f / botCount));
		}
		if (Input.GetKeyDown (KeyCode.B)) {
			myPerformance.sendKeyPress (23, botLeft + (botSpan * transform.right * 4f / botCount));
		}
		if (Input.GetKeyDown (KeyCode.N)) {
			myPerformance.sendKeyPress (24, botLeft + (botSpan * transform.right * 5f / botCount));
		}
		if (Input.GetKeyDown (KeyCode.M)) {
			myPerformance.sendKeyPress (25, botLeft + (botSpan * transform.right * 6f / botCount));
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			myPerformance.sendKeyPress (26, botLeft + (botSpan * transform.right * 1f / 2f) + Vector3.back * 5f + Vector3.down * 5f);
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}

		if (Input.GetKeyDown (KeyCode.Return)) {
			//SpeechSynthesizer sp = new 
			//Requester.getMp3 ("http://translate.google.com/translate_tts?tl=en&q=hello&ie=UTF-8&total=1&idx=0&client=t");
			//Requester.getMp3 ("http://translate.google.com/translate_tts?tl=en&q=Hello%20World&client=t");
			//Requester.getMp3 ();
//			Application.OpenURL("http://translate.google.com/translate_tts?ie=UTF-8&q=Hello&tl=en&client=t");
//			Application.OpenURL("https://www.google.com/?gws_rd=ssl");
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit();
		}
	}

	public void shootChar(int id, Vector3 pos) {
		if (Time.time - lastShotTime < 0.255f) {
			//return;
		}
		Rigidbody blockClone;

		if (id == 26) {
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
		//Color ech = computeColor (lastPos.x);



		//blockClone.GetComponent<Renderer> ().materials [0].color = ech;
		blockClone.GetComponent<Renderer> ().materials [0].mainTexture = textures[id];

		blockClone.angularVelocity = new Vector3 (Random.value * 5f - 2.5f, Random.value * 5f - 2.5f, Random.value * 5f - 2.5f);
		//blockClone.velocity = transform.forward.normalized * speed;//Vector3.forward * speed;
		blockClone.velocity = (Vector3.zero - pos).normalized * speed;
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
