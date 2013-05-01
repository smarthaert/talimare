using UnityEngine;

public class LocalResourceSupplier : MonoBehaviour {

	public float supplyRange;
	protected Controllable controllable;
	
	protected virtual void Start() {
		// A capsule collider provides a trigger for the supply range
		CapsuleCollider supplyCollider = gameObject.AddComponent<CapsuleCollider>();
		supplyCollider.isTrigger = true;
		supplyCollider.radius = supplyRange;
		supplyCollider.height = 99f;
		
		// A rigidbody allows this object's collider to trigger while it is moving
		Rigidbody rigidBody = gameObject.AddComponent<Rigidbody>();
		rigidBody.isKinematic = true;
		
		controllable = transform.parent.gameObject.GetComponent<Controllable>();
	}
	
	// Called when a collider enters this supply range
	protected virtual void OnTriggerEnter(Collider other) {}
	
	// Called when a collider exits this supply range
	protected virtual void OnTriggerExit(Collider other) {}
	
	public bool IsControllableWithSameOwner(Collider other) {
		return other.transform.parent != this.transform.parent && other.GetComponent<Controllable>() != null && other.GetComponent<Controllable>().owner == controllable.owner;
	}
}