using UnityEngine;
using System.Collections.Generic;

public abstract class GameUtil : Component {
	
	protected static AstarPath Pathfinding { get; set; }
	
	public const string TAG_UNIT = "Unit";
	public const string TAG_BUILDING = "Building";
	public const string TAG_TECH = "Tech";
	public const string TAG_BUILD_PROGRESS = "BuildProgress";
	public const string TAG_RESOURCE = "Resource";
	
	//TODO high: roads and walls. start by drawing a line between two clicked points on the ground
	
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
				minDist = dist;
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
	
	// Returns all unit types that can currently be created in all buildings by the given player
	public static HashSet<Creatable> GetAllCurrentUnitTypes(Player player) {
		HashSet<Creatable> allUnitTypes = new HashSet<Creatable>();
		Object[] allBuildings = GameObject.FindObjectsOfType(typeof(BaseBuildingControl));
		foreach(Object building in allBuildings) {
			if(((BaseBuildingControl)building).Owner == player) {
				foreach(CreatableUnit unit in ((BaseBuildingControl)building).units) {
					allUnitTypes.Add(unit);
				}
			}
		}
		return allUnitTypes;
	}
	
	// Returns the building that can create the given unit that is nearest to the given point in space and owned by the given player
	public static BaseBuildingControl FindNearestBuildingToCreateUnit(CreatableUnit unit, Vector3 point, Player player) {
		float minDist = Mathf.Infinity;
		BaseBuildingControl minBuilding = null;
		foreach(BaseBuildingControl building in FindAllOwnedInstancesOf<BaseBuildingControl>(player)) {
			if(building.units.Contains(unit)) {
				float dist = Vector3.Distance(point, building.transform.position);
				if(dist < minDist) {
					minBuilding = building;
					minDist = dist;
				}
			}
		}
		return minBuilding;
	}
	
	// Removes all null references from the given list. This is useful for scrubbing a list which may reference destroyed objects
	// (when destroyed objects are compared with null, true is returned)
	public static void ScrubNullsFromList<T>(ref List<T> collection) {
		for(int i = collection.Count - 1; i >= 0; i--) {
			Debug.Log("checking "+i+" which should be: "+collection[i]);
			if(collection[i] == null) {
				Debug.LogWarning("scrubbing nulls... coll:"+collection.Count);
				//collection.RemoveAt(i);
				Debug.LogWarning("done scrubbing nulls... coll:"+collection.Count);
			}
		}
	}
	
	// Removes all null references from the given set. This is useful for scrubbing a set which may reference destroyed objects
	// (when destroyed objects are compared with null, true is returned)
	public static void ScrubNullsFromSet<T>(ref HashSet<T> collection) {
		List<T> itemsToRemove = new List<T>();
		foreach(T item in collection) {
			if(item == null) {
				itemsToRemove.Add(item);
			}
		}
		foreach(T item in itemsToRemove) {
			Debug.LogWarning("scrubbing nulls... coll:"+collection.Count);
			collection.Remove(item);
			Debug.LogWarning("done scrubbing nulls... coll:"+collection.Count);
		}
	}
}