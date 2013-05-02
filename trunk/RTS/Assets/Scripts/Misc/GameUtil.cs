using UnityEngine;
using System.Collections.Generic;

public abstract class GameUtil : Component {
	
	protected static AstarPath Pathfinding { get; set; }
	
	// Creates a new instance of the given Controllable for the given Player at the given position.
	// Also applies applicable techs to this new object
	public static GameObject InstantiateControllable(Controllable controllable, Player player, Vector3 position) {
		GameObject newObject = (GameObject)Instantiate(controllable.gameObject, position, Quaternion.identity);
		Controllable newControllable = newObject.GetComponent<Controllable>();
		newControllable.owner = player;
		newControllable.name = controllable.gameObject.name;
		newControllable.transform.parent = player.transform;
		//apply all applicable techs to the new object
		PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
		foreach(Tech appliedTech in newControllable.applicableTechs) {
			if(playerStatus.techs.Contains(appliedTech)) {
				appliedTech.ApplyTechTo(newObject);
			}
		}
		return newObject;
	}
	
	// Returns the instance of the given Component type (on a GameObject) that is nearest to the given point in space and owned by the given player
	public static T FindNearestOwnedInstanceOf<T>(Vector3 point, Player player) where T : Component {
		float minDist = Mathf.Infinity;
		T minComp = null;
		foreach(T component in FindAllOwnedInstancesOf<T>(player)) {
			float dist = Vector3.Distance(point, component.transform.position);
			if(dist < minDist) {
				minComp = component;
			}
		}
		return minComp;
	}
	
	// Returns all instances of the given Component type (on a GameObject) that are owned by the given player
	public static ICollection<T> FindAllOwnedInstancesOf<T>(Player player) where T : Component {
		ICollection<T> components = new HashSet<T>();
		foreach(T component in (T[])GameObject.FindObjectsOfType(typeof(T))) {
			if(component.GetComponent<Controllable>() != null && component.GetComponent<Controllable>().owner == player) {
				components.Add(component);
			}
		}
		return components;
	}
	
	public static void RescanPathfinding() {
		if(Pathfinding == null) {
			Pathfinding = GameObject.Find("Pathfinding").GetComponent<AstarPath>();
		}
		Pathfinding.Scan();
	}
}