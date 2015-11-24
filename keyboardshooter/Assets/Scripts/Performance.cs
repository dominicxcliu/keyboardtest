using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Performance : NetworkManager {

	private float roundWaitTime;
	private float amountWaited;
	private float initCountdown;
	private float countDown;
	private int round;
	private int numRounds;
	private string[] words;
	private string[] descriptors;
	private StreamReader story; //stream of the text file of the story, read and write to this
	private StreamReader dictionaryFile;//check they are valid words
	private HashSet<string> dict;
	private bool finished;
	private bool lockedForRound; //if other client sends in a valid word for the round first, then you can't
	private bool isHost;
	public string inputStoryPath;
	public string dictPath;
	public string toWritePath;
	public string wordsPath;
	public string descriptorsPath;

	public GameObject[] spawners;
	private SpawnerScript mySpawner;
	private SpawnerScript otherSpawner;

	/*              network stuff                */
	//system information -> network -> wifi address
	//public string connectionIP = "128.237.182.237";
	public string connectionIP = "localhost";
	//some ridiculous number
	//public int portNumber = 8271;
	public int portNumber = 7777;
	private string currentMessage = string.Empty;
	private bool connected;
	public string userName;

	private int idNum;
	
	bool toggle = false;
	public GameObject button;
	
	public List<MyMessages.ChatMessage> chatHistory = new List<MyMessages.ChatMessage> ();
	//public List<string> userHistory = new List<string> ();
	public List<string> users = new List<string>();
	public static short MSGType = 555;
	private GUIStyle nameStyle;
	private GUIStyle msgStyle;

	private Vector2 chatScrollPosition = Vector2.zero;
	private Vector2 userScrollPosition = Vector2.zero;
	private float invalidTimer;
	private string invalidText = "";
	private bool started = false;
	private float startCount = 5f;
	private AudioSource enter;

	private string speechUrl;

	// Use this for initialization
	void Start () {
		roundWaitTime = 3f;
		amountWaited = 0f;
		initCountdown = 7f; //each round lasts 7 rounds
		countDown = initCountdown;
		round = 0;
		story = new StreamReader (inputStoryPath);
		dictionaryFile = new StreamReader (dictPath);
		dict = new HashSet<string> ();
		finished = false;
		initialzeStuff ();

		//network stuff
		connected = false;
		button = GameObject.Find("ToggleButton");
		nameStyle = new GUIStyle ();
		nameStyle.fontStyle = FontStyle.Bold;
		nameStyle.normal.textColor = Color.white;
		nameStyle.wordWrap = true;
		msgStyle = new GUIStyle ();
		msgStyle.fontStyle = FontStyle.Normal;
		msgStyle.wordWrap = true;
		msgStyle.normal.textColor = Color.white;
		isHost = false;
		idNum = 1;
		enter = GetComponent <AudioSource> ();
	}

	//prefill the words and these are the 
	void initialzeStuff(){
		//populate dict with american-english words
		string word;
		while ((word = dictionaryFile.ReadLine()) != null){
			dict.Add(word.ToLower());
		}
		dictionaryFile.Close();
		Debug.Log ("done with dict");
		Debug.Log (dict.Count);

		StreamReader wordReader = new StreamReader (wordsPath);
		StreamReader descriptorReader = new StreamReader (descriptorsPath);
		words = wordReader.ReadToEnd ().Split("\n"[0]);
		descriptors = descriptorReader.ReadToEnd ().Split ("\n" [0]);
		wordReader.Close ();
		descriptorReader.Close ();
		numRounds = Mathf.Min (words.Length, descriptors.Length);
		numRounds = 13;
		Debug.Log (numRounds);
	}

	//display the words that were chosen, write the words to the story, choose a background music
	//switch scenes to some end scene or something so this doesn't continue?
	void doEndSequence(){
		finished = true;
		int i = 0;
		string line;
		string final = "";
		while ((line = story.ReadLine()) != null){
			final += line;
			if (i < numRounds)
				final += " " + words[i] + " ";
			i++;
		}
		story.Close ();
		File.WriteAllText (toWritePath, final);
		Debug.Log ("finisehd");
		//Debug.Break ();
		//Regex.Replace (final, @",", "%2C");
		//Regex.Replace (final, @"\"" )
//		final = final.Replace (",", "%2C");
//		final = final.Replace ("\"", "%22");
//
//		speechUrl = "http://translate.google.com/translate_tts?ie=UTF-8&q=" + final + "&tl=en";
		//Debug.Log (url);
		//Application.OpenURL ("/Users/Dominic/Desktop/story.txt");
		//Application.OpenURL(url);
		//Application.OpenURL ("http://translate.google.com/translate_tts?ie=UTF-8&q=Hello&tl=en&client=t");
		Application.OpenURL ("file:///Users/Dominic/Desktop/story.txt");
		Application.Quit ();
		return;
	}



	/**************************             network stuff                *****************************/
	public override void OnClientConnect(NetworkConnection conn) {
		connected = true;
		base.OnClientConnect (conn);
		Debug.Log ("client is connected");
		sendUserConnect ();
		mySpawner = spawners [idNum].GetComponent <SpawnerScript> ();
		otherSpawner = spawners [1 - idNum].GetComponent <SpawnerScript> ();
	}
	
	public override void OnServerConnect(NetworkConnection conn) {
		base.OnServerConnect (conn);
		foreach (string u in users) {
			MyMessages.UserMessage um = new MyMessages.UserMessage ();
			um.user = u;
			um.connected = true;
			NetworkServer.SendToClient (conn.connectionId, (short) MyMessages.MyMessageTypes.USER_INFO, um);
		}
		Debug.Log ("new client's users updated");
	}
	
	public override void OnServerDisconnect (NetworkConnection conn) {
		base.OnServerDisconnect (conn);
		sendUserDisconnect ();
	}
	
	//automatically called when starting a client
	public override void OnStartClient(NetworkClient mClient)
	{
		Debug.Log ("onstartclient called");
		base.OnStartClient(mClient);
		mClient.RegisterHandler((short)MyMessages.MyMessageTypes.CHAT_MESSAGE, OnClientChatMessage);
		mClient.RegisterHandler((short)MyMessages.MyMessageTypes.USER_INFO, OnClientUserInfo);
		mClient.RegisterHandler((short)MyMessages.MyMessageTypes.KEY_MESSAGE, OnClientKeyMessage);
	}
	
	
	//automatically called when starting a server
	// hook into NetManagers server setup process
	public override void OnStartServer()
	{
		Debug.Log ("onstartserver called");
		base.OnStartServer(); //base is empty
		NetworkServer.RegisterHandler ((short)MyMessages.MyMessageTypes.CHAT_MESSAGE, OnServerChatMessage);
		NetworkServer.RegisterHandler ((short)MyMessages.MyMessageTypes.USER_INFO, OnServerUserInfo);
		NetworkServer.RegisterHandler ((short)MyMessages.MyMessageTypes.KEY_MESSAGE, OnServerKeyMessage);
		isHost = true;
		idNum = 0;
		//connected = true;
	}
	
	
	//when a chat message reaches the server
	private void OnServerChatMessage(NetworkMessage netMsg)
	{
		var msg = netMsg.ReadMessage<MyMessages.ChatMessage>();
		MyMessages.ChatMessage chat = new MyMessages.ChatMessage ();
		chat.user = msg.user;
		chat.message = msg.message;
		NetworkServer.SendToAll((short) MyMessages.MyMessageTypes.CHAT_MESSAGE, chat);
		//button.GetComponent<ToggleScript>().ToggleColor();
	}
	
	//when a chat message reaches the client
	private void OnClientChatMessage(NetworkMessage netMsg)
	{


		var msg = netMsg.ReadMessage <MyMessages.ChatMessage>();
		if (msg.message == "start" && !started) {
			started = true;
			return;
		}
		if (!started) {
			return;
		}
		lockedForRound = true; //someone hit send so no more typing
		//button.GetComponent<ToggleScript>().ToggleColor();
		//userHistory.Add (msg.user);
		chatHistory.Add ((MyMessages.ChatMessage)msg);
		chatScrollPosition = (new Vector2 (0, 1000000));
		//for each client, update the word array with this, restart round somehow
		words [round] = msg.message;
		enter.Play ();
		round++;
	}
	
	private void OnServerUserInfo(NetworkMessage netMsg)
	{
		var msg = netMsg.ReadMessage<MyMessages.UserMessage> ();
		MyMessages.UserMessage um = new MyMessages.UserMessage ();
		if (msg.connected && !users.Contains (msg.user)) {
			this.users.Add (msg.user);
			Debug.Log ("user added to host");
		} else if (!msg.connected && users.Contains (msg.user)) {
			this.users.Remove (msg.user);
			Debug.Log ("user removed from host");
		} else {
			Debug.Log ("nothing happened at host");
		}
		um.user = msg.user;
		um.connected = msg.connected;
		//		Debug.Log (um.users);
		NetworkServer.SendToAll((short) MyMessages.MyMessageTypes.USER_INFO, um);
	}
	
	private void OnClientUserInfo(NetworkMessage netMsg)
	{
		
		var msg = netMsg.ReadMessage<MyMessages.UserMessage> ();
		Debug.Log (msg.connected);
		Debug.Log (msg.user);
		if (msg.connected && !users.Contains (msg.user)) {
			this.users.Add (msg.user);
			Debug.Log ("user added to client");
		} else if (!msg.connected && users.Contains (msg.user)) {
			this.users.Remove (msg.user);
			Debug.Log ("user removed from client");
		} else {
			Debug.Log ("nothing happened at client");
		}
		//Debug.Log (this.users);
	}


	private void OnServerKeyMessage (NetworkMessage netMsg) {
		var msg = netMsg.ReadMessage<MyMessages.KeyMessage> ();
		MyMessages.KeyMessage km = new MyMessages.KeyMessage ();
		km.user = msg.user;
		km.id = msg.id;
		km.pos = msg.pos;
		NetworkServer.SendToAll((short) MyMessages.MyMessageTypes.KEY_MESSAGE, km);
	}

	private void OnClientKeyMessage (NetworkMessage netMsg) {
		var msg = netMsg.ReadMessage<MyMessages.KeyMessage> ();
		if (msg.user == userName) {
			mySpawner.shootChar (msg.id, msg.pos);
			Debug.Log ("shooting for myself");
		} else {
			otherSpawner.shootChar (msg.id, msg.pos);
			Debug.Log ("shooting for other");
		}
	}

	private void OnGUI(){

		if (!lockedForRound && GUI.GetNameOfFocusedControl() == "text") {
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Q) {
				this.sendKeyPress (0, mySpawner.topLeft);
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.W) {
				this.sendKeyPress (1, mySpawner.topLeft + (mySpawner.topSpan * transform.right * 1f / mySpawner.topCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.E) {
				this.sendKeyPress (2, mySpawner.topLeft + (mySpawner.topSpan * transform.right * 2f / mySpawner.topCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.R) {
				this.sendKeyPress (3, mySpawner.topLeft + (mySpawner.topSpan * transform.right * 3f / mySpawner.topCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.T) {
				this.sendKeyPress (4, mySpawner.topLeft + (mySpawner.topSpan * transform.right * 4f / mySpawner.topCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Y) {
				this.sendKeyPress (5, mySpawner.topLeft + (mySpawner.topSpan * transform.right * 5f / mySpawner.topCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.U) {
				this.sendKeyPress (6, mySpawner.topLeft + (mySpawner.topSpan * transform.right * 6f / mySpawner.topCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.I) {
				this.sendKeyPress (7, mySpawner.topLeft + (mySpawner.topSpan * transform.right * 7f / mySpawner.topCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.O) {
				this.sendKeyPress (8, mySpawner.topLeft + (mySpawner.topSpan * transform.right * 8f / mySpawner.topCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.P) {
				this.sendKeyPress (9, mySpawner.topLeft + (mySpawner.topSpan * transform.right * 9f / mySpawner.topCount));
			}
		

			else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.A) {
				this.sendKeyPress (10, mySpawner.midLeft);
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.S) {
				this.sendKeyPress (11, mySpawner.midLeft + (mySpawner.midSpan * transform.right * 1f / mySpawner.midCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.D) {
				this.sendKeyPress (12, mySpawner.midLeft + (mySpawner.midSpan * transform.right * 2f / mySpawner.midCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F) {
				this.sendKeyPress (13, mySpawner.midLeft + (mySpawner.midSpan * transform.right * 3f / mySpawner.midCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.G) {
				this.sendKeyPress (14, mySpawner.midLeft + (mySpawner.midSpan * transform.right * 4f / mySpawner.midCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.H) {
				this.sendKeyPress (15, mySpawner.midLeft + (mySpawner.midSpan * transform.right * 5f / mySpawner.midCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.J) {
				this.sendKeyPress (16, mySpawner.midLeft + (mySpawner.midSpan * transform.right * 6f / mySpawner.midCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.K) {
				this.sendKeyPress (17, mySpawner.midLeft + (mySpawner.midSpan * transform.right * 7f / mySpawner.midCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.L) {
				this.sendKeyPress (18, mySpawner.midLeft + (mySpawner.midSpan * transform.right * 8f / mySpawner.midCount));
			}
		
		
		
		
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Z) {
				this.sendKeyPress (19, mySpawner.botLeft);
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.X) {
				this.sendKeyPress (20, mySpawner.botLeft + (mySpawner.botSpan * transform.right * 1f / mySpawner.botCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.C) {
				this.sendKeyPress (21, mySpawner.botLeft + (mySpawner.botSpan * transform.right * 2f / mySpawner.botCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.V) {
				this.sendKeyPress (22, mySpawner.botLeft + (mySpawner.botSpan * transform.right * 3f / mySpawner.botCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.B) {
				this.sendKeyPress (23, mySpawner.botLeft + (mySpawner.botSpan * transform.right * 4f / mySpawner.botCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.N) {
				this.sendKeyPress (24, mySpawner.botLeft + (mySpawner.botSpan * transform.right * 5f / mySpawner.botCount));
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.M) {
				this.sendKeyPress (25, mySpawner.botLeft + (mySpawner.botSpan * transform.right * 6f / mySpawner.botCount));
			}
		
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space) {
				this.sendKeyPress (26, mySpawner.botLeft + (mySpawner.botSpan * transform.right * 1f / 2f) + Vector3.back * 5f + Vector3.down * 5f);
			}
		
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) {
				//SpeechSynthesizer sp = new 
				//Requester.getMp3 ("http://translate.google.com/translate_tts?tl=en&q=hello&ie=UTF-8&total=1&idx=0&client=t");
				//Requester.getMp3 ("http://translate.google.com/translate_tts?tl=en&q=Hello%20World&client=t");
				//Requester.getMp3 ();
				//			Application.OpenURL("http://translate.google.com/translate_tts?ie=UTF-8&q=Hello&tl=en&client=t");
				//			Application.OpenURL("https://www.google.com/?gws_rd=ssl");
			}
		

		}
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape) {
			Application.Quit ();
		}

		if (!connected) {
			
			GUILayout.BeginVertical (GUILayout.Width (300));
			{
				GUILayout.BeginHorizontal ();
				{
					GUILayout.Label ("Connection IP", GUILayout.Width (100));
					connectionIP = GUILayout.TextField (connectionIP);
				}
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				{
					GUILayout.Label ("Port Number ", GUILayout.Width (100));
					int.TryParse (GUILayout.TextField (portNumber.ToString ()), out portNumber);
				}
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				{
					GUILayout.Label ("Username ", GUILayout.Width (100));
					userName = GUILayout.TextField (userName);
				}
				GUILayout.EndHorizontal ();
				
				
			}
			GUILayout.EndVertical ();
			
			
			//if connect button clicked
			if (GUILayout.Button ("Connect")) {
				this.networkAddress = connectionIP;
				this.networkPort = portNumber;
				this.StartClient ();
			}
			//if host button clicked
			//a host is a server and a client at the same time
			if (GUILayout.Button ("Host")) {
				this.networkAddress = connectionIP;
				this.networkPort = portNumber;
				this.StartHost ();
			}
		} else if (!started) {
			displayStart ();
			return;
		} else if (started && startCount > 0) {
			displayStartCount ();
			return;
		}
		else if (!lockedForRound) {
			displayStuff ();
		} else {
			displayLocked();
		}
		invalidText = "";
		if (invalidTimer > 0) {
			invalidText = "your word is invalid";
			invalidTimer -= Time.deltaTime;
		}

	}

	void Update(){
		doTimerStuff();
		if (started && startCount > 0) {
			startCount -= Time.deltaTime;
		}
		if (finished) {
			if (Input.GetKeyDown (KeyCode.Return)) {
				Application.OpenURL ("file:///Users/Dominic/Desktop/story.txt");
			}
		}
	}

	void doTimerStuff(){
		if (users.Count == 2 && started && startCount < 0) {
			//check timer, if countDown is <= 0, send message with words[round]
			if (!lockedForRound)
				countDown -= Time.deltaTime;
			else {
				countDown = initCountdown;
				currentMessage = "";
			}
			//Debug.Log ("nextRound in: " + countDown);
			if (countDown <= 0f) {
				//no one supplied a word quick enough so host sends msg!
				if(round != 12){
					if (isHost) {
						countDown = initCountdown;
						Debug.Log ("TIMES UP");
						currentMessage = words [round];
						sendMessage ();
					}
				}
				if(round == 12) {
					countDown = initCountdown;
					string newMessage = Regex.Replace (currentMessage, @"[\p{P}\d]", "");

					currentMessage = newMessage;
					int numSpaces = 1;
					for(int i = 19; i < currentMessage.Length; i += 20 + numSpaces) {
						currentMessage = currentMessage.Insert (i, " ");
						numSpaces++;
					}
					currentMessage.Trim ();
					if(string.IsNullOrEmpty(currentMessage)) {
						currentMessage = words[12];
					}
					sendMessage ();
				}
			}

			if (lockedForRound) { //new round stuff is happening?
				//Debug.Log ("amountWaited: " + amountWaited);
				amountWaited += Time.deltaTime;
				if (amountWaited > roundWaitTime) {
					lockedForRound = false;
					amountWaited = 0f;
				}
			} else {
				//Debug.Log ("***** descriptor *****" + descriptors [round]);
			}
		}
	}

	void displayStart () {

		GUILayout.BeginHorizontal (GUILayout.Width (Screen.width));
		{
			//left
			GUILayout.Label ("", GUILayout.Width (Screen.width/3));
			
			//middle
			GUILayout.BeginVertical (GUILayout.Width (Screen.width/3));
			{
				GUILayout.Label ("", GUILayout.Height (Screen.height/3));
				
				if (isHost) {
					if(GUILayout.Button ("Start", GUILayout.Width(Screen.width/3), GUILayout.Height(Screen.height/3)))
					{
						currentMessage = "start";
						sendMessage ();
					}
				}
				else {
					GUILayout.Label ("Waiting to Start", GUILayout.Width(Screen.width/3), GUILayout.Height(Screen.height/3));
				}
				
			}
			GUILayout.EndVertical ();
		}
		GUILayout.EndHorizontal ();
	}

	void displayLocked() {

		GUILayout.BeginHorizontal (GUILayout.Width (Screen.width));
		{
			//left
			GUILayout.Label ("", GUILayout.Width (Screen.width/3));
			
			//middle
			GUILayout.BeginVertical (GUILayout.Width (Screen.width/3));
			{
				GUILayout.Label ("", GUILayout.Height (Screen.height/3));
				
				GUILayout.Label ("ROUND OVER");
				GUILayout.Label ("Time Until Next Round: " + (int)(roundWaitTime - amountWaited + 1));
				
			}
			GUILayout.EndVertical ();
		}
		GUILayout.EndHorizontal ();
	}

	void displayStartCount () {

		GUILayout.BeginHorizontal (GUILayout.Width (Screen.width));
		{
			//left
			GUILayout.Label ("", GUILayout.Width (Screen.width/3));
			
			//middle
			GUILayout.BeginVertical (GUILayout.Width (Screen.width/3));
			{
				GUILayout.Label ("", GUILayout.Height (Screen.height/3));
				
				GUILayout.Label ("STARTING IN: " + (int)(startCount + 1));
				
			}
			GUILayout.EndVertical ();
		}
		GUILayout.EndHorizontal ();
	}

	void displayStuff(){
		GUI.FocusControl("text");
		if (round < numRounds) {
			//chat display
			GUILayout.BeginHorizontal (GUILayout.Width (Screen.width));
			{
				//left
				GUILayout.Label ("", GUILayout.Width (Screen.width/3));

				//middle
				GUILayout.BeginVertical (GUILayout.Width (Screen.width/3));
				{
					GUILayout.Label ("", GUILayout.Height (Screen.height/3));

					GUI.SetNextControlName ("text");
					currentMessage = GUILayout.TextField (currentMessage, GUILayout.Width (Screen.width/3));
					GUILayout.Label (invalidText);
					if(Event.current.isKey) {
						switch (Event.current.keyCode) {
						case KeyCode.Return:
							if (!lockedForRound){
								sendMessage();
							}
							break;
						}
					}

					GUILayout.Label ("Description: " + descriptors[round]);
					GUILayout.Label ("Time Left: " + (int) (countDown + 1f));

				}
				GUILayout.EndVertical ();


			}
			GUILayout.EndHorizontal ();
		} 
		else {
			if (!finished)
				doEndSequence();
		}
	}

	bool isValidWord(string word){
		return ((!string.IsNullOrEmpty (word)) && word.Length >= 3 && dict.Contains (word));
	}

	//sends a chat message to server
	void sendMessage() {
		if (currentMessage == null)
			return;
		currentMessage.Trim ();
		currentMessage = currentMessage.ToLower ();
		Debug.Log ("CURRENTROUND: "+ round);
		if (isValidWord (currentMessage) || round == 0 || round == 8 || round == 12) {
			//play good sound or whatever to show someone got a word
			MyMessages.ChatMessage msg = new MyMessages.ChatMessage ();
			msg.user = userName;
			msg.message = currentMessage;
			NetworkManager.singleton.client.Send ((short)MyMessages.MyMessageTypes.CHAT_MESSAGE, msg);
			currentMessage = "";
		} else {
			//play invalid sound do some animation thing, show an x on both screens in the gui or something
			invalidTimer = 2f;
		}
	}

	//sends connect notification to server
	void sendUserConnect() {
		MyMessages.UserMessage umsg = new MyMessages.UserMessage ();
		umsg.user = userName;
		umsg.connected = true;
		NetworkManager.singleton.client.Send ((short)MyMessages.MyMessageTypes.USER_INFO, umsg);
	}
	
	//sends disconnect notification to server
	void sendUserDisconnect() {
		MyMessages.UserMessage umsg = new MyMessages.UserMessage ();
		umsg.user = userName;
		umsg.connected = false;
		NetworkManager.singleton.client.Send ((short)MyMessages.MyMessageTypes.USER_INFO, umsg);
	}

	public void sendKeyPress (int id, Vector3 pos) {
		MyMessages.KeyMessage kmsg = new MyMessages.KeyMessage ();
		kmsg.user = userName;
		kmsg.id = id;
		kmsg.pos = pos;
		NetworkManager.singleton.client.Send ((short)MyMessages.MyMessageTypes.KEY_MESSAGE, kmsg);
	}
}
