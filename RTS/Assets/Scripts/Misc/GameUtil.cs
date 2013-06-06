using UnityEngine;
using System.Collections.Generic;

public abstract class GameUtil : Component {
	
	protected static AstarPath Pathfinding { get; set; }
	
	public const string TAG_UNIT = "Unit";
	public const string TAG_BUILDING = "Building";
	public const string TAG_TECH = "Tech";
	public const string TAG_BUILD_PROGRESS = "BuildProgress";
	public const string TAG_RESOURCE = "Resource";
	
	// Creates a new instance of the given Controllable for the given Player at the given position.
	// Also applies applicable techs to this new object
	public static GameObject InstantiateControllable(Controllable controllable, Player player, Vector3 position) {
		GameObject newObject = (GameObject)Instantiate(controllable.gameObject, position, Quaternion.identity);
		Controllable newControllable = newObject.GetComponent<Controllable>();
		newControllable.name = controllable.gameObject.name;
		newControllable.transform.parent = player.transform;
		newControllable.Owner = player;
		
		//apply all applicable techs to the new object
		PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
		foreach(Tech appliedTech in newControllable.applicableTechs) {
			if(playerStatus.techs.Contains(appliedTech)) {
				appliedTech.ApplyTechTo(newObject);
			}
		}
		return newObject;
	}
	
	public static GameObject InstantiateConvertedControllable(Controllable oldControllable, Controllable targetControllable, Player player, Vector3 position) {
		GameObject newObject = (GameObject)Instantiate(targetControllable.gameObject, position, Quaternion.identity);
		Controllable newControllable = newObject.GetComponent<Controllable>();
		newControllable.name = targetControllable.gameObject.name;
		newControllable.transform.parent = player.transform;
		newControllable.Owner = player;
		
		//apply all applicable techs to the new object
		PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
		foreach(Tech appliedTech in newControllable.applicableTechs) {
			if(playerStatus.techs.Contains(appliedTech)) {
				appliedTech.ApplyTechTo(newObject);
			}
		}
		
		//update the new object's hp
		ControllableStatus oldStatus = oldControllable.GetComponent<ControllableStatus>();
		ControllableStatus newStatus = newControllable.GetComponent<ControllableStatus>();
		newStatus.SetHP((oldStatus.HP / oldStatus.maxHP) * newStatus.maxHP);
		
		Destroy(oldControllable.gameObject);
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
			if(component.GetComponent<Controllable>() != null && component.GetComponent<Controllable>().Owner == player) {
				components.Add(component);
			}
		}
		return components;
	}
	
	public static void RescanPathfinding() {
		if(Pathfinding == null) {
			GameObject pathfinding = GameObject.Find("Pathfinding");
			if(pathfinding != null)
				Pathfinding = pathfinding.GetComponent<AstarPath>();
		}
		if(Pathfinding != null) {
			Pathfinding.Scan();
		}
	}
}