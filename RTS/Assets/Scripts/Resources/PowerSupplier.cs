using UnityEngine;
using System.Collections.Generic;

public class PowerSupplier : LocalResourceSupplier {
	
	public int powerSupplied;
	
	public List<BuildingStatus> powerUsers = new List<BuildingStatus>();
	public int PowerFree { get; protected set; }
	
	protected override void Start() {
		base.Start();
		
		PowerFree = powerSupplied;
	}
	
	protected override void OnTriggerEnter(Collider other) {
		if(IsControllableWithSameOwner(other) && other.GetComponent<BuildingStatus>() != null) {
			BuildingStatus buildingStatus = other.GetComponent<BuildingStatus>();
			if(buildingStatus.powerRequired > 0 && buildingStatus.powerRequired < PowerFree) {
				powerUsers.Add(buildingStatus);
				PowerFree -= buildingStatus.powerRequired;
				buildingStatus.Powered = true;
			}
		}
	}
	
	protected override void OnTriggerExit(Collider other) {
		if(IsControllableWithSameOwner(other) && other.GetComponent<BuildingStatus>() != null) {
			BuildingStatus buildingStatus = other.GetComponent<BuildingStatus>();
			if(buildingStatus.powerRequired > 0 && buildingStatus.powerRequired < PowerFree) {
				powerUsers.Remove(buildingStatus);
				PowerFree += buildingStatus.powerRequired;
				buildingStatus.Powered = false;
			}
		}
	}
}