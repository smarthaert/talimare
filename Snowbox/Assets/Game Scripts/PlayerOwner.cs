using UnityEngine;
using System.Collections;

public class PlayerOwner : uLink.MonoBehaviour
{
	public static readonly Vector3 handOffset = new Vector3(2.29778f, 8.992505f, 1.130554f);

	public AnimationController controller = null;
	public GameObject snowball = null;
	public Transform hand = null;
	
	[HideInInspector]
	public Color playerColor;
	public string playerName;

	private PlatformCharacterController platformer = null;

	private bool isReloading = false;

	void Awake()
	{
		platformer = GetComponent<PlatformCharacterController>();
	}

	void Start()
	{
		ReloadBegin();
	}

	void Update()
	{
		if (Input.GetMouseButton(0))
		{
			Screen.lockCursor = true;
		}

		// Play the shoot animation
		if (Screen.lockCursor && !isReloading && Input.GetButtonDown("Fire1") && platformer.aimTarget != null)
		{
			ReloadBegin();

			controller.Throw();

			Invoke("SpawnSnowball", 0.1f);

			Vector3 handpos = transform.position + transform.rotation * handOffset;
			Vector3 aimDir = platformer.aimTarget.position - handpos;
			networkView.RPC("Throw", uLink.RPCMode.Server, aimDir);
		}
	}

	void SpawnSnowball()
	{
		// TODO: add time to live to snowball

		Vector3 handpos = transform.position + transform.rotation * handOffset;
		Vector3 aimDir = platformer.aimTarget.position - handpos;
		GameObject newball = Instantiate(snowball, handpos, Quaternion.LookRotation(aimDir)) as GameObject;
		Physics.IgnoreCollision(collider, newball.collider);

#if UNITY_4_0
		newball.SetActive(true);
#else
		newball.SetActiveRecursively(true);
#endif
	}

	void ReloadBegin()
	{
		isReloading = true;
		Invoke("ReloadEnd", 0.5f);
	}

	void ReloadEnd()
	{
		isReloading = false;
	}

	void uLink_OnNetworkInstantiate(uLink.NetworkMessageInfo msg)
	{
		uLink.NetworkPlayer myPlayer = uLink.Network.player;
		myPlayer.localData = this;
		
		playerColor = msg.networkView.initialData.Read<Color>();
		playerName = msg.networkView.initialData.Read<string>();

		BroadcastMessage("SetColor", playerColor, SendMessageOptions.DontRequireReceiver);
	}
}
