using UnityEngine;
using System.Collections;

// Root class for all tech scripts. Note: even though this script derives from MonoBahaviour, it is never actually instantiated
public class Tech : MonoBehaviour {
	
	// Adds this tech to the given Player's tech list and calls ApplyTech for all existing instances of affected objects
	public virtual void AddTechForPlayer(Player player) {
		player.PlayerStatus.techs.Add(this);
		foreach(Object obj in GameObject.FindSceneObjectsOfType(typeof(Controllable))) {
			Controllable controllable = (Controllable)obj;
			if(controllable.owner == player && controllable.applicableTechs.Contains(this)) {
				ApplyTechTo(controllable.gameObject);
			}
		}
		Debug.Log(this+" research completed!");
	}
	
	// Applies the effects of this tech to the given GameObject
	public virtual void ApplyTechTo(GameObject gameObject) {}
}
