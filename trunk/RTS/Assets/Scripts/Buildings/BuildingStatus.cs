using UnityEngine;
using System.Collections.Generic;

// Keeps information about a building's current status
[AddComponentMenu("Buildings/Building Status")]
public class BuildingStatus : ControllableStatus {
	
	// The amount of power which is required to fully power this object
	public int powerRequired;
	// Whether or not this object is currently supplied full power
	public bool Powered { get; set; }
	// Whether or not this object is currently accepting power
	public bool PowerEnabled { get; protected set; }
	// Holds all of the power suppliers of which this object is in range
	protected List<PowerSource> _powerSuppliersInRange = new List<PowerSource>();
	protected List<PowerSource> PowerSuppliersInRange
	{
		get {
			_powerSuppliersInRange.RemoveAll(m => m == null);
			return _powerSuppliersInRange;
		}
	}
	
	protected override void Awake() {
		base.Awake();
		
		Powered = false;
		PowerEnabled = (powerRequired > 0 ? true : false);
	}
	
	protected override void Start() {
		base.Start();
	}
	
	protected override void Update() {
		base.Update();
	}
	
	// Adds a power supplier to the list of suppliers in range and checks for combined power.
	// NOTE: Make sure that Powered is set to true BEFORE calling this method, if applicable
	public void AddPowerSupplier(PowerSource supplier) {
		PowerSuppliersInRange.Add(supplier);
		if(!Powered && PowerSuppliersInRange.Count > 1) {
			RecheckPowerSuppliersForNewPower();
		}
	}
	
	protected void RecheckPowerSuppliersForNewPower() {
		int totalFreePower = 0;
		foreach(PowerSource powerSupplier in PowerSuppliersInRange) {
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
			foreach(PowerSource powerSupplier in PowerSuppliersInRange) {
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
	
	public void RemovePowerSupplier(PowerSource supplier) {
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
			foreach(PowerSource powerSupplier in PowerSuppliersInRange) {
				powerSupplier.ReleasePower(this);
			}
			Powered = false;
		}
	}
}
