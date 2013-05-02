using UnityEngine;
using System.Collections.Generic;

// Keeps information about a building's current status
public class BuildingStatus : ControllableStatus {
	
	public int powerRequired;
	public bool Powered { get; set; } //TODO add ability to toggle a building's power need
	// Holds all of the power suppliers of which this object is in range
	public List<PowerSupplier> PowerSuppliersInRange { get; protected set; }
	//TODO when a new power supplier is added, if this building doesn't become powered then call out to all power suppliers asking for
		//partial power. if combined they can supply enough, ask them to do it
	
	protected override void Start() {
		base.Start();
		
		PowerSuppliersInRange = new List<PowerSupplier>();
	}
	
	protected override void Update() {
		base.Update();
	}
	
	protected void OnDestroy() {
		foreach(PowerSupplier powerSupplier in PowerSuppliersInRange.ToArray()) {
			powerSupplier.OnTriggerExit(collider);
		}
	}
}
