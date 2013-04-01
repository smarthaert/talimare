using UnityEngine;

public abstract class GameUtil : Component {
	
	// Creates a new instance of the given Controllable for the given Player at the given position.
	// Also applies applicable techs to this new object
	public static GameObject InstantiateControllable(Controllable controllable, Player player, Vector3 position) {
		GameObject newObject = (GameObject)Instantiate(controllable.gameObject, position, Quaternion.identity);
		Controllable newControllable = newObject.GetComponent<Controllable>();
		newControllable.owner = player;
		newControllable.name = controllable.gameObject.name;
		//apply all applicable techs to the new object
		PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
		foreach(Tech appliedTech in newControllable.applicableTechs) {
			if(playerStatus.techs.Contains(appliedTech)) {
				appliedTech.ApplyTechTo(newObject);
			}
		}
		return newObject;
	}
}