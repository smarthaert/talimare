using UnityEngine;
using System.Collections;

// Root class for all tech scripts
public class Tech : MonoBehaviour {
	
	private static PlayerStatus playerStatus;
	
	void Start() {
		playerStatus = (PlayerStatus)GameObject.Find("Main Camera").GetComponent<PlayerStatus>();
	}

	public virtual void Execute() {
		playerStatus.techs.Add(this);
		Debug.Log(this+" research completed!");
	}
}
