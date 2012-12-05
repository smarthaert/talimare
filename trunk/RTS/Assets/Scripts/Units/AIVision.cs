using UnityEngine;
using System.Collections.Generic;

public class AIVision : MonoBehaviour {

	public float visionRange = 10.0f;
	//public int visionCasts = 6; trying out using visionRange for this instead
	public bool revealsFog = true;
	public bool hidesInFog = true;
	
	// Cast LOS lines from this height above the center of this object
	protected float LOSHeight = 0.0f;
	
	protected FogOfWar fogOfWarScript;
	protected AIPathfinder pathfinder;
	
	protected bool isUnit = false;
	
	// Layer mask for the fog layer only
	protected int fogLayerMask;
	// Layer mask for the layers that impede vision
	protected int LOSLayerMask;
	// This determines how many rays we cast evenly around in a circle around this object
	protected float circleStep;
	
	void Start() {
		// A capsule collider provides a trigger for the vision range
		gameObject.AddComponent<CapsuleCollider>();
		CapsuleCollider visionCollider = GetComponent<CapsuleCollider>();
		visionCollider.isTrigger = true;
		visionCollider.radius = visionRange;
		visionCollider.height = 99f;
		
		// A rigidbody allows this object's collider to trigger while it is moving
		gameObject.AddComponent<Rigidbody>();
		GetComponent<Rigidbody>().isKinematic = true;
		
		// Determine if this a unit (the alternative would be a building)
		if(transform.root.gameObject.CompareTag("Unit"))
			isUnit = true;
		
		GameObject fogOfWar = GameObject.Find("FogOfWar");
		fogOfWarScript = fogOfWar.GetComponent<FogOfWar>();
		if(isUnit)
			pathfinder = transform.root.gameObject.GetComponent<AIPathfinder>();
		
		fogLayerMask = 1 << LayerMask.NameToLayer("FogOfWar");
		LOSLayerMask = 1 << LayerMask.NameToLayer("Terrain");
		
		circleStep = ((2*Mathf.PI) / visionRange) - 0.0001f;
		
		if(revealsFog)
			CalculateRevealPoints();
	}
	
	void Update() {
		if(revealsFog) {
		//if(isUnit && revealsFog) {
			// If this script is too inefficient, we can improve FogOfWar so that AIVisions don't need to recalculate vision while non-moving
			//if(pathfinder.IsMoving()) {
				CalculateRevealPoints();
			//}
		} else if(hidesInFog) {
			CheckHideOrShow();
		}
	}
	
	void CalculateRevealPoints() {
		float x = transform.position.x;
		float z = transform.position.z;
		RevealFogOFWarAt(transform.position.x, transform.position.z);
		
		for (float visionRangeStep = fogOfWarScript.gridSize; visionRangeStep <= visionRange; visionRangeStep += fogOfWarScript.gridSize) {
			for (float i = 0; i <= 2*Mathf.PI; i += circleStep) {
				RevealFogOFWarAt(x+(Mathf.Cos(i)*visionRangeStep), z+(Mathf.Sin(i)*visionRangeStep));
			}
		}
	}
	
	// Reveals fog of war at the given position
	void RevealFogOFWarAt(float x, float z) {
		RaycastHit hit;
		
		Debug.DrawRay(new Vector3(x,FogOfWar.RAYCAST_HEIGHT,z), -Vector3.up*FogOfWar.RAYCAST_HEIGHT);
		if (!Physics.Raycast(new Vector3(x, FogOfWar.RAYCAST_HEIGHT, z), -Vector3.up, out hit, Mathf.Infinity, fogLayerMask)) {
			return;
		}
		
		// Checking Line Of Sight (LOS):
		// If we have no line of sight to that area, abort so we don't reveal the fog of war there
		Vector3 LOSorigin = transform.position + new Vector3(0, LOSHeight, 0);
		Vector3 LOSdir = (hit.point + new Vector3(0, 3, 0)) - LOSorigin;
		float LOSdist = LOSdir.magnitude;
		LOSdir.Normalize();
	
		RaycastHit LOShit;
		if (Physics.Raycast(LOSorigin, LOSdir, out LOShit, LOSdist, LOSLayerMask)) {
			Debug.DrawRay(LOSorigin, LOSdir*LOSdist, Color.red);
			return;
		}
		Debug.DrawRay(LOSorigin, LOSdir*LOSdist, Color.green);
		
		// By now it is assumed that we hit the fog of war mesh, so we use it
		// directly and not hit.collider.gameObject.GetComponent(MeshFilter).mesh;
		
		// Get which vertices were hit
		int p0 = fogOfWarScript.meshTriangles[hit.triangleIndex * 3 + 0];
		int p1 = fogOfWarScript.meshTriangles[hit.triangleIndex * 3 + 1];
		int p2 = fogOfWarScript.meshTriangles[hit.triangleIndex * 3 + 2];
		
		fogOfWarScript.AddVertToReveal(p0);
		fogOfWarScript.AddVertToReveal(p1);
		fogOfWarScript.AddVertToReveal(p2);
	}
	
	// Checks whether this object should be hidden under the fog, or shown if it no longer under fog
	void CheckHideOrShow() {
		float x = transform.position.x;
		float z = transform.position.z;
		
		RaycastHit hit;
		if (!Physics.Raycast(new Vector3(x, FogOfWar.RAYCAST_HEIGHT, z), -Vector3.up, out hit, Mathf.Infinity, fogLayerMask)) {
			return;
		}
		
		// By now it is assumed that we hit the fog of war mesh, so we use it
		// directly and not hit.collider.gameObject.GetComponent(MeshFilter).mesh;
	
		// Get which vertices were hit
		int p0 = fogOfWarScript.meshTriangles[hit.triangleIndex * 3 + 0];
		int p1 = fogOfWarScript.meshTriangles[hit.triangleIndex * 3 + 1];
		int p2 = fogOfWarScript.meshTriangles[hit.triangleIndex * 3 + 2];
	
		if (fogOfWarScript.IsVertRevealed(p0) && fogOfWarScript.IsVertRevealed(p1) && fogOfWarScript.IsVertRevealed(p2)) {
			Show();
		} else {
			Hide();
		}
	}
	
	// Hides this object, disabling all renderers
	void Hide() {
		Renderer[] allRenderer = transform.root.gameObject.GetComponentsInChildren<Renderer>();
		foreach(Renderer rendR in allRenderer) {
			rendR.enabled = false;
		}
	}
	
	// Shows this object, enabling all renderers
	void Show() {
		Renderer[] allRenderer = transform.root.gameObject.GetComponentsInChildren<Renderer>();
		foreach(Renderer rendR in allRenderer) {
			rendR.enabled = true;
		}
	}
	
	// Called when another collider enters this vision range
	void OnTriggerEnter(Collider other) {
		if(other.transform.root != this.transform.root) {
			if(other.CompareTag("Unit") || other.CompareTag("Building") || other.CompareTag("BuildProgress")) {
				transform.root.gameObject.SendMessage("ObjectEnteredVision", other.gameObject);
			}
		}
	}
	
	// Called when another collider exits this vision range
	void OnTriggerExit(Collider other) {
		if(other.transform.root != this.transform.root) {
			if(other.CompareTag("Unit") || other.CompareTag("Building") || other.CompareTag("BuildProgress")) {
				transform.root.gameObject.SendMessage("ObjectLeftVision", other.gameObject);
			}
		}
	}
}
