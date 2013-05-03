using UnityEngine;
using System.Collections.Generic;

public class PowerSupplier : LocalResourceSupplier {
	
	public int powerSupplied;
	public int FreePower { get; protected set; }
	
	// Holds all objects which are eligible for power supply (including those currently powered by this supplier).
	// NOTE: Objects are held regardless of whether power is currently enabled for them
	protected List<BuildingStatus> suppliablesInRange = new List<BuildingStatus>();
	// Holds all objects which are currently powered by this supply
	protected Dictionary<BuildingStatus, int> powerUsers = new Dictionary<BuildingStatus, int>();
	
	protected override void Start() {
		base.Start();
		
		FreePower = powerSupplied;
	}
	
	public override void OnTriggerEnter(Collider other) {
		if(IsControllableWithSameOwner(other) && other.GetComponent<BuildingStatus>() != null) {
			BuildingStatus buildingStatus = other.GetComponent<BuildingStatus>();
			if(buildingStatus.powerRequired > 0) {
				suppliablesInRange.Add(buildingStatus);
				RecheckSuppliablesForNewPower(); //must recheck power before calling AddPowerSupplier
				buildingStatus.AddPowerSupplier(this);
			}
		}
	}
	
	// Checks all suppliables in range to see if any can now be supplied power
	protected void RecheckSuppliablesForNewPower() {
		foreach(BuildingStatus buildingStatus in suppliablesInRange) {
			if(buildingStatus.PowerEnabled && !buildingStatus.Powered && buildingStatus.powerRequired < FreePower) {
				Debug.Log("Powering up "+buildingStatus.name+" using pushed power");
				buildingStatus.Powered = true;
				CapturePower(buildingStatus, buildingStatus.powerRequired);
			}
		}
	}
	
	public override void OnTriggerExit(Collider other) {
		if(other.GetComponent<BuildingStatus>() != null) {
			BuildingStatus buildingStatus = other.GetComponent<BuildingStatus>();
			
			suppliablesInRange.Remove(buildingStatus);
			buildingStatus.RemovePowerSupplier(this);
			if(powerUsers.ContainsKey(buildingStatus)) {
				if(buildingStatus.Powered) {
					Debug.Log("Unpowering "+other.name);
					buildingStatus.Powered = false;
				}
				ReleasePower(buildingStatus);
				RecheckSuppliablesForNewPower();
			}
		}
	}
	
	// Captures the given amount of power for use by the given object
	public void CapturePower(BuildingStatus buildingStatus, int amount) {
		powerUsers.Add(buildingStatus, amount);
		FreePower -= powerUsers[buildingStatus];
	}
	
	// Releases any power captured by the given object
	public void ReleasePower(BuildingStatus buildingStatus) {
		FreePower += powerUsers[buildingStatus];
		powerUsers.Remove(buildingStatus);
	}
	
	protected void OnDestroy() {
		BuildingStatus[] powerUsersClone = new BuildingStatus[powerUsers.Count];
		powerUsers.Keys.CopyTo(powerUsersClone, 0);
		foreach(BuildingStatus buildingStatus in powerUsersClone) {
			if(buildingStatus.Powered) {
				Debug.Log("Unpowering "+buildingStatus.name);
				buildingStatus.Powered = false;
			}
			ReleasePower(buildingStatus);
		}
		foreach(BuildingStatus buildingStatus in suppliablesInRange) {
			buildingStatus.RemovePowerSupplier(this);
		}
	}
}