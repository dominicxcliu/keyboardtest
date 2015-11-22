using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;

public class Requester : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	public static void getMp3() {
		string url = "http://translate.google.com/translate_tts?ie=UTF-8&q=Hello&tl=en&client=t";
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
		//request.Referer = "";
		//request.Referer = "http://translate.google.com/";
		request.Referer = "https://translate.google.com/?ie=UTF-8&hl=en&client=tw-ob#en/ja/Hello";
		request.UserAgent = "stagefright/1.2 (Linux;Android 5.0)";

		HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
		Stream receiveStream = response.GetResponseStream();
		byte[] buffer = new byte[32768];
		using (FileStream fileStream = File.Create ("/Users/Dominic/Downloads/testtt.mp3")) {
			while(true) {
				int read = receiveStream.Read (buffer, 0, buffer.Length);
				if(read <= 0) {
					break;
				}
				fileStream.Write(buffer, 0, read);
			}
		}
		Debug.Log ("butts");
 	}
}
