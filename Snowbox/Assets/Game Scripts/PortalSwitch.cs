using UnityEngine;

public class PortalSwitch : uLink.MonoBehaviour
{
	void Start()
	{
		if (uLink.Network.isServer)
		{
			SendSwitch(false);
		}
	}

	void uLink_OnPeerConnected(uLink.NetworkPeer peer)
	{
		SendSwitch(true);
	}

	void uLink_OnPeerDisconnected(uLink.NetworkPeer peer)
	{
		SendSwitch(false);
	}

	void SendSwitch(bool active)
	{
		Switch(active);

		networkView.RemoveRPCs();

		if (uLink.Network.isCellServer)
			networkView.RPC("Switch", uLink.RPCMode.Others, active); // TODO Pikko Server does not support buffered RPCs yet.
		else
			networkView.RPC("Switch", uLink.RPCMode.OthersBuffered, active);
	}

	[RPC]
	void Switch(bool active)
	{
		foreach (Transform child in transform)
		{
#if UNITY_4_0
			child.gameObject.SetActive(active);
#else
			child.gameObject.active = active;
#endif
		}
	}
}
