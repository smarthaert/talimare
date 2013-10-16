using UnityEngine;
using System.Collections;

public class PlayerProxy : uLink.MonoBehaviour
{
	public AnimationController controller = null;
	public GameObject snowball = null;

	[RPC]
	void Throw(Vector3 dir)
	{
		// TODO: add time to live to snowball

		controller.Throw();
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
		Color color = msg.networkView.initialData.Read<Color>();
		string name = msg.networkView.initialData.Read<string>();

		BroadcastMessage("SetLabel", name, SendMessageOptions.DontRequireReceiver);
		BroadcastMessage("SetColor", color, SendMessageOptions.DontRequireReceiver);
	}
}
