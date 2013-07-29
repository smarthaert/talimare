using UnityEngine;

// Catches triggered events and passes them to a parent PowerSource
public class PowerSourceTrigger : MonoBehaviour {
	
	public PowerSource PowerSource { get; set; }
	
	public void OnTriggerEnter(Collider other) {
		if(IsControllableWithSameOwner(other) && other.GetComponent<BuildingStatus>() != null) {
			PowerSource.SuppliableEnteredRange(other.GetComponent<BuildingStatus>());
		}
	}
	
	public void OnTriggerExit(Collider other) {
		if(IsControllableWithSameOwner(other) && other.GetComponent<BuildingStatus>() != null) {
			PowerSource.SuppliableLeftRange(other.GetComponent<BuildingStatus>());
		}
	}
	
	public bool IsControllableWithSameOwner(Collider other) {
		return other.GetComponent<Controllable>() != null && other.GetComponent<Controllable>().Owner == PowerSource.controllable.Owner;
	}
}