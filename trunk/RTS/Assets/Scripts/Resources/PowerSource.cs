using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Controllable))]
[AddComponentMenu("Resources/Power Source")]
public class PowerSource : MonoBehaviour {
	
	public int powerSupplied;
	public float supplyRange;
	
	public Controllable controllable;
	
	protected SphereCollider SupplyCollider { get; set; }
	
	// Holds all objects which are eligible for power supply (including those currently powered by this supplier).
	// NOTE: Objects are held regardless of whether power is currently enabled for them
	private List<BuildingStatus> _suppliablesInRange = new List<BuildingStatus>();
	protected List<BuildingStatus> SuppliablesInRange
	{
		get {
			_suppliablesInRange.RemoveAll(m => m == null);
			return _suppliablesInRange;
		}
	}
	
	// Holds all objects which are currently powered by this supply
	private Dictionary<BuildingStatus, int> _powerUsers = new Dictionary<BuildingStatus, int>();
	protected Dictionary<BuildingStatus, int> PowerUsers
	{
		get {
			//this is a bit inefficient, but it's the best I can do for now
			BuildingStatus[] powerUsersClone = new BuildingStatus[_powerUsers.Count];
			_powerUsers.Keys.CopyTo(powerUsersClone, 0);
			foreach(BuildingStatus suppliable in powerUsersClone) {
				if(suppliable == null) {
					_powerUsers.Remove(suppliable);
				}
			}
			return _powerUsers;
		}
	}
	
	// Returns the amount of power that is still available to be captured
	public int FreePower
	{
		get {
			int _freePower = powerSupplied;
			foreach(BuildingStatus suppliable in PowerUsers.Keys) {
				_freePower -= PowerUsers[suppliable];
			}
			return _freePower;
		}
	}
	
	protected void Awake() {
		controllable = GetComponent<Controllable>();
		
		// A child GameObject is needed to attach a collider to. Attaching the collider to the parent object causes problems
		GameObject child = new GameObject(this.GetType().Name);
		child.transform.parent = transform;
		child.transform.localPosition = Vector3.zero;
		child.layer = LayerMask.NameToLayer("Ignore Raycast");
		
		// A CapsuleCollider provides a trigger for the supply range
		SupplyCollider = child.AddComponent<SphereCollider>();
		SupplyCollider.isTrigger = true;
		SupplyCollider.radius = supplyRange;
		
		// A trigger script passes triggered events back to this one
		PowerSourceTrigger trigger = child.AddComponent<PowerSourceTrigger>();
		trigger.PowerSource = this;
	}
	
	public void SuppliableEnteredRange(BuildingStatus suppliable) {
		if(suppliable.powerRequired > 0 && !SuppliablesInRange.Contains(suppliable)) {
			SuppliablesInRange.Add(suppliable);
			RecheckSuppliablesForNewPower(); //must recheck power BEFORE calling AddPowerSupplier
			suppliable.AddPowerSupplier(this);
		}
	}
	
	// Checks all suppliables in range to see if any can now be supplied power
	protected void RecheckSuppliablesForNewPower() {
		foreach(BuildingStatus buildingStatus in SuppliablesInRange) {
			if(buildingStatus.PowerEnabled && !buildingStatus.Powered && buildingStatus.powerRequired < FreePower) {
				Debug.Log("Powering up "+buildingStatus.name+" using pushed power");
				buildingStatus.Powered = true;
				CapturePower(buildingStatus, buildingStatus.powerRequired);
			}
		}
	}
	
	public void SuppliableLeftRange(BuildingStatus suppliable) {
		SuppliablesInRange.Remove(suppliable);
		suppliable.RemovePowerSupplier(this);
		if(PowerUsers.ContainsKey(suppliable)) {
			if(suppliable.Powered) {
				Debug.Log("Unpowering "+suppliable.name);
				suppliable.Powered = false;
			}
			ReleasePower(suppliable);
			RecheckSuppliablesForNewPower();
		}
	}
	
	// Captures the given amount of power for use by the given object
	public void CapturePower(BuildingStatus suppliable, int amount) {
		PowerUsers.Add(suppliable, amount);
	}
	
	// Releases any power captured by the given object
	public void ReleasePower(BuildingStatus suppliable) {
		PowerUsers.Remove(suppliable);
	}
	
	protected void OnDestroy() {
		foreach(BuildingStatus suppliable in PowerUsers.Keys) {
			if(suppliable.Powered) {
				Debug.Log("Unpowering "+suppliable.name);
				suppliable.Powered = false;
			}
		}
		PowerUsers.Clear();
	}
}