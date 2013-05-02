using UnityEngine;
using System.Collections.Generic;

public class PowerSupplier : LocalResourceSupplier {
	
	public int powerSupplied;
	public int PowerFree { get; protected set; }
	
	// Holds all objects which are eligible for power supply (including those currently powered by this supplier)
	protected List<BuildingStatus> suppliablesInRange = new List<BuildingStatus>();
	// Holds all objects which are currently powered by this supply
	protected List<BuildingStatus> powerUsers = new List<BuildingStatus>();
	
	//TODO use partial power supply to power a building from multiple suppliers
	
	protected override void Start() {
		base.Start();
		
		PowerFree = powerSupplied;
	}
	
	public override void OnTriggerEnter(Collider other) {
		if(IsControllableWithSameOwner(other) && other.GetComponent<BuildingStatus>() != null) {
			BuildingStatus buildingStatus = other.GetComponent<BuildingStatus>();
			if(buildingStatus.powerRequired > 0) {
				suppliablesInRange.Add(buildingStatus);
				buildingStatus.PowerSuppliersInRange.Add(this);
				RecheckSuppliablesForNewPower();
			}
		}
	}
	
	public override void OnTriggerExit(Collider other) {
		if(other.GetComponent<BuildingStatus>() != null) {
			BuildingStatus buildingStatus = other.GetComponent<BuildingStatus>();
			
			suppliablesInRange.Remove(buildingStatus);
			buildingStatus.PowerSuppliersInRange.Remove(this);
			if(powerUsers.Contains(buildingStatus)) {
				Debug.Log("Unpowering "+other.name);
				powerUsers.Remove(buildingStatus);
				PowerFree += buildingStatus.powerRequired;
				buildingStatus.Powered = false;
				RecheckSuppliablesForNewPower();
			}
		}
	}
	
	// Checks all suppliables in range to see if any can now be supplied power
	protected void RecheckSuppliablesForNewPower() {
		foreach(BuildingStatus buildingStatus in suppliablesInRange) {
			if(!buildingStatus.Powered && buildingStatus.powerRequired < PowerFree) {
				Debug.Log("Powering up "+buildingStatus.name);
				powerUsers.Add(buildingStatus);
				PowerFree -= buildingStatus.powerRequired;
				buildingStatus.Powered = true;
			}
		}
	}
	
	protected void OnDestroy() {
		foreach(BuildingStatus buildingStatus in powerUsers) {
			Debug.Log("Unpowering "+buildingStatus.name);
			buildingStatus.Powered = false;
		}
		foreach(BuildingStatus buildingStatus in suppliablesInRange) {
			buildingStatus.PowerSuppliersInRange.Remove(this);
		}
	}
}