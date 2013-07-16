using UnityEngine;
using System.Collections.Generic;

public class WaterNetwork : MonoBehaviour {
	
	protected MeshCollider SupplyCollider { get; set; }
	
	protected virtual void Awake() {
		// A child GameObject is needed to attach a collider to. Attaching the collider to the parent object causes problems
		GameObject child = new GameObject(this.GetType().Name);
		child.transform.parent = transform;
		child.transform.localPosition = Vector3.zero;
		child.layer = LayerMask.NameToLayer("Ignore Raycast");
		
		// A CapsuleCollider provides a trigger for the supply range
		SupplyCollider = child.AddComponent<SphereCollider>();
		SupplyCollider.isTrigger = true;
		SupplyCollider.radius = supplyRange;
	}
	
	protected virtual void Start() {
		// Evaluate objects already colliding
		foreach(Collider collider in Physics.OverlapSphere(SupplyCollider.transform.position, SupplyCollider.radius)) {
			OnTriggerEnter(collider);
		}
	}
	
	// Called when a collider enters this supply range
	public virtual void OnTriggerEnter(Collider other) {}
	
	// Called when a collider exits this supply range
	public virtual void OnTriggerExit(Collider other) {}
	
	public bool IsControllableWithSameOwner(Collider other) {
		return other.GetComponent<Controllable>() != null && other.GetComponent<Controllable>().Owner == controllable.Owner;
	}
	
	
	
	protected List<WaterNetworkSource> Sources { get; set; }
	protected List<WaterNetworkNode> Nodes { get; set; }
	
	// The objects which are currently within supply range and eligible for supply
	protected List<UnitStatus> suppliablesInRange = new List<UnitStatus>();
	
	// Water ticks every this many seconds (supply and loss)
	public static float waterTickRate = 2;
	protected float waterTickTimer = 0;
	
	public WaterNetwork() {
		Sources = new List<WaterNetworkSource>();
		Nodes = new List<WaterNetworkNode>();
	}
	
	public void AddSource(WaterNetworkSource source) {
		Sources.Add(source);
	}
}