using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	
	public string ip = "127.0.0.1";
	public int port = 25000;
	
	protected void OnGUI() {
		if(Network.peerType == NetworkPeerType.Disconnected) {
			if(GUI.Button(new Rect(100, 100, 100, 25), "Start Client")) {
				
			}
		}
	}
}
