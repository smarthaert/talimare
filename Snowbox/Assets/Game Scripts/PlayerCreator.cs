using UnityEngine;
using System.Collections;

public class PlayerCreator : uLink.MonoBehaviour
{
	public GameObject snowball = null;

	private bool isAlreadyHandovering = false;
	private ulong timeWhenCreated = 0;
	
	[HideInInspector]
	public Color playerColor;
	public string playerName;

	private const float HANDOVER_OFFSETZ = 20;
	private const float HANDOVER_OFFSETY = 15;
	private const ulong HANDOVER_COOLDOWN = 500; // in milliseconds
	
	void Awake()
	{
		timeWhenCreated = uLink.Network.timeInMillis;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (isAlreadyHandovering || (uLink.Network.timeInMillis - timeWhenCreated) < HANDOVER_COOLDOWN) return;

		uLink.NetworkP2P p2p = other.transform.parent.GetComponent<uLink.NetworkP2P>();

		if (!p2p) return;

		uLink.NetworkPeer[] peers = p2p.connections;
		if (peers.Length <= 0) return;

        if (Network.isServer && !networkView.owner.isConnected) return;

		isAlreadyHandovering = true;
		
		Vector3 relativeDir = p2p.transform.InverseTransformDirection(transform.forward);
		float portalSide = Mathf.Sign(relativeDir.z);
		Vector3 offsetPos = new Vector3(0, HANDOVER_OFFSETY, portalSide * HANDOVER_OFFSETZ);
		
		p2p.HandoverPlayerObjects(networkView.owner, peers[0], offsetPos, Quaternion.identity);
	}

	[RPC]
	void Throw(Vector3 dir)
	{
		// TODO: add time to live to snowball

		networkView.RPC("Throw", uLink.RPCMode.OthersExceptOwner, dir);

		Vector3 pos = transform.position + transform.rotation * PlayerOwner.handOffset;

		GameObject newball = Instantiate(snowball, pos, Quaternion.LookRotation(dir)) as GameObject;
		Physics.IgnoreCollision(collider, newball.collider);

#if UNITY_4_0
		newball.SetActive(true);
#else
		newball.SetActiveRecursively(true);
#endif
	}

	void uLink_OnNetworkInstantiate(uLink.NetworkMessageInfo msg)
	{
		uLink.NetworkPlayer myPlayer = uLink.Network.player;
		myPlayer.localData = this;
		
		playerColor = msg.networkView.initialData.Read<Color>();
		playerName = msg.networkView.initialData.Read<string>();

		BroadcastMessage("SetLabel", playerName, SendMessageOptions.DontRequireReceiver);
		BroadcastMessage("SetColor", playerColor, SendMessageOptions.DontRequireReceiver);
	}
}
