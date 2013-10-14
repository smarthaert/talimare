using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {
	
	protected void Start() {
		LaunchServer();
	}
	
	protected void LaunchServer() {
		//Network.incomingPassword = "HolyMoly";
		bool useNat = !Network.HavePublicAddress();
		Network.InitializeServer(32, 25000, useNat);
	}
}
