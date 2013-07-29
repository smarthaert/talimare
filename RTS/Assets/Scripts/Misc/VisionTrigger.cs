using UnityEngine;

// Catches triggered events and passes them to a parent Vision
public class VisionTrigger : MonoBehaviour {
	
	public Vision Vision { get; set; }
	
	public void OnTriggerEnter(Collider other) {
		if(IsControllableWithDifferentOwner(other)) {
			Vision.ObjectEnteredVision(other.gameObject);
		}
	}
	
	public void OnTriggerExit(Collider other) {
		if(IsControllableWithDifferentOwner(other)) {
			Vision.ObjectLeftVision(other.gameObject);
		}
	}
	
	protected bool IsControllableWithDifferentOwner(Collider other) {
		return other.GetComponent<Controllable>() != null && other.GetComponent<Controllable>().Owner != Vision.controllable.Owner;
	}
}