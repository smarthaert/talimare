using UnityEngine;
using System.Collections;

// Root class for all tech scripts. Note: Even though this is a MonoBehaviour, this class is never instantiated, only Executed
public class Tech : MonoBehaviour {
	
	// Executes this tech, adding it to the tech list for the given player and performing any other changes
	public virtual void Execute(Player player) {
		player.PlayerStatus.techs.Add(this);
		Debug.Log(this+" research completed!");
	}
}
