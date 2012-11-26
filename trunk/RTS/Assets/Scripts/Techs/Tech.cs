using UnityEngine;
using System.Collections;

// Root class for all tech scripts
public class Tech : MonoBehaviour {
	
	private static PlayerStatus playerStatus;

	public virtual void Execute() {
		// Player status must be gotten here since techs are never actually instantiated
		if(playerStatus == null)
			playerStatus = GameObject.Find("Main Camera").GetComponent<PlayerStatus>();
		playerStatus.techs.Add(this);
		Debug.Log(this+" research completed!");
	}
}
