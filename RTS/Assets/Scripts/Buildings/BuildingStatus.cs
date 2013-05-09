using UnityEngine;
using System.Collections.Generic;

// Keeps information about a building's current status
public class BuildingStatus : ControllableStatus {
	
	// The amount of power which is required to fully power this object
	public int powerRequired;
	// Whether or not this object is currently supplied full power
	public bool Powered { get; set; }
	// Whether or not this object is currently accepting power
	public bool PowerEnabled { get; protected set; }
	// Holds all of the power suppliers of which this object is in range
	protected List<PowerSupplier> PowerSuppliersInRange { get; set; }
	
	protected override void Awake() {
		// Add a kinematic rigidbody if there isn't already one in order to make collisions work
		if(GetComponent<Rigidbody>() == null) {
			gameObject.AddComponent<Rigidbody>().isKinematic = true;
		}
	}
	
	protected override void Start() {
		base.Start();
		
		Powered = false;
		PowerEnabled = (powerRequired > 0 ? true : false);
		PowerSuppliersInRange = new List<PowerSupplier>();
	}
	
	protected override void Update() {
		base.Update();
	}
	
	// Adds a power supplier to the list of suppliers in range and checks for combined power.
	// NOTE: Make sure that Powered is set to true BEFORE calling this method, if applicable
	public void AddPowerSupplier(PowerSupplier supplier) {
		PowerSuppliersInRange.Add(supplier);
		if(!Powered && PowerSuppliersInRange.Count > 1) {
			RecheckPowerSuppliersForNewPower();
		}
	}
	
	protected void RecheckPowerSuppliersForNewPower() {
		int totalFreePower = 0;
		foreach(PowerSupplier powerSupplier in PowerSuppliersInRange) {
			if(powerSupplier.FreePower >= powerRequired) {
				//this supplier can power us fully, but somehow isn't already
				Debug.Log("Powering up "+name+" using self-gotten power");
				powerSupplier.CapturePower(this, powerRequired);
				Powered = true;
				totalFreePower = 0;
				break;
			} else {
				totalFreePower += powerSupplier.FreePower;
			}
		}
		if(totalFreePower >= powerRequired) {
			//there is enough free power to supply this object with combined power
			int powerNeeded = powerRequired;
			int powerCaptured;
			foreach(PowerSupplier powerSupplier in PowerSuppliersInRange) {
				powerCaptured = Mathf.Min(powerNeeded, powerSupplier.FreePower);
				powerNeeded -= powerCaptured;
				powerSupplier.CapturePower(this, powerCaptured);
				if(powerNeeded == 0) {
					break;
				}
			}
			Debug.Log("Powering up "+name+" using partial power");
			Powered = true;
		}
	}
	
	public void RemovePowerSupplier(PowerSupplier supplier) {
		PowerSuppliersInRange.Remove(supplier);
	}
	
	// Sets whether power is being accepted by this building and handles any change in status
	public void SetPowerEnabled(bool powerEnabled) {
		bool wasPowerEnabled = PowerEnabled;
		PowerEnabled = powerEnabled;
		if(!wasPowerEnabled && PowerEnabled && PowerSuppliersInRange.Count > 0) {
			//power switched on
			RecheckPowerSuppliersForNewPower();
		} else if(wasPowerEnabled && !PowerEnabled) {
			//power switched off
			foreach(PowerSupplier powerSupplier in PowerSuppliersInRange) {
				powerSupplier.ReleasePower(this);
			}
			Debug.Log("Unpowering "+name);
			Powered = false;
		}
	}
	
	protected void OnDestroy() {
		foreach(PowerSupplier powerSupplier in PowerSuppliersInRange.ToArray()) {
			powerSupplier.OnTriggerExit(collider);
		}
	}
}
