using UnityEngine;
using uLink;

public class ServerLogic : uLink.MonoBehaviour {
	
	public GameObject creatorPrefab;
	public GameObject proxyPrefab;

	protected void uLink_OnServerInitialized() {
		uLink.Network.Instantiate(uLink.Network.AllocateViewID(), uLink.NetworkPlayer.server, proxyPrefab, null, creatorPrefab, Vector3.zero, creatorPrefab.transform.rotation, 0);
	}
}
