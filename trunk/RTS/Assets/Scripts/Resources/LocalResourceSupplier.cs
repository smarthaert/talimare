using UnityEngine;

public class LocalResourceSupplier : MonoBehaviour {

	public float supplyRange;
	protected Controllable controllable;
	
	protected virtual void Awake() {
		// A child GameObject is needed to attach a collider to. Attaching the collider to the parent object causes problems
		GameObject child = new GameObject(this.GetType().Name);
		child.transform.parent = transform;
		child.transform.localPosition = Vector3.zero;
		child.layer = LayerMask.NameToLayer("Ignore Raycast");
		
		// A CapsuleCollider provides a trigger for the supply range
		CapsuleCollider supplyCollider = child.AddComponent<CapsuleCollider>();
		supplyCollider.isTrigger = true;
		supplyCollider.radius = supplyRange;
		supplyCollider.height = 99f;
		
		// Add a kinematic rigidbody if there isn't already one in order to make collisions work
		if(GetComponent<Rigidbody>() == null) {
			gameObject.AddComponent<Rigidbody>().isKinematic = true;
		}
	}
	
	protected virtual void Start() {
		controllable = GetComponent<Controllable>();
	}
	
	// Called when a collider enters this supply range
	public virtual void OnTriggerEnter(Collider other) {}
	
	// Called when a collider exits this supply range
	public virtual void OnTriggerExit(Collider other) {}
	
	public bool IsControllableWithSameOwner(Collider other) {
		return other.GetComponent<Controllable>() != null && other.GetComponent<Controllable>().owner == controllable.owner;
	}
}