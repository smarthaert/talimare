using UnityEngine;
using System.Collections.Generic;

public class AIVision : MonoBehaviour {

	public float visionRange = 10.0f;
	//public int visionCasts = 6; trying out using visionRange for this instead
	public bool revealsFog = true;
	public bool hidesInFog = true;
	
	// Cast LOS lines from this height above the center of this object
	protected float LOSHeight = 0.0f;
	
	protected GameObject fogOfWar;
	protected Mesh fogOfWarMesh;
	protected FogOfWar fogOfWarScript;
	protected AIPathfinder pathfinder;
	protected UnitControl unitControl;
	
	// Layer mask for the fog layer only
	protected int fogLayerMask;
	// Layer mask for the layers that impede vision
	protected int LOSLayerMask;
	
	void Start() {
		fogOfWar = GameObject.Find("FogOfWar");
		fogOfWarMesh = fogOfWar.GetComponent<MeshFilter>().mesh;
		fogOfWarScript = fogOfWar.GetComponent<FogOfWar>();
		pathfinder = transform.root.gameObject.GetComponent<AIPathfinder>();
		unitControl = transform.root.gameObject.GetComponent<UnitControl>();
		
		fogLayerMask = 1 << LayerMask.NameToLayer("FogOfWar");
		LOSLayerMask = 1 << LayerMask.NameToLayer("Terrain");
	
		CalculateRevealPoints();
	}
	
	void Update() {
		if(revealsFog) {
			// If this script is too inefficient, we can improve FogOfWar so that AIVisions don't need to recalculate vision while non-moving
			//if (pathfinder.IsMoving()) {
				CalculateRevealPoints();
			//}
		} else if(hidesInFog) {
			//TODO hide in fog
		}
	}
	
	void CalculateRevealPoints() {
		float x = transform.position.x;
		float z = transform.position.z;
		RevealFogOFWarAt(transform.position.x, transform.position.z);
		
		// This determines how many rays we cast evenly around the circle around this object
		float circleStep = ((2*Mathf.PI) / (visionRange+1)) - 0.0001f;
		
		for (float visionRangeStep = fogOfWarScript.gridSize; visionRangeStep <= visionRange; visionRangeStep += fogOfWarScript.gridSize) {
			for (float i = 0; i <= 2*Mathf.PI; i += circleStep) {
				RevealFogOFWarAt(x+(Mathf.Cos(i)*visionRangeStep), z+(Mathf.Sin(i)*visionRangeStep));
			}
		}
	}
	
	void RevealFogOFWarAt(float x, float z) {
		RaycastHit hit;
		
		//Debug.DrawRay(new Vector3(x,FogOfWar.RAYCAST_HEIGHT,z), -Vector3.up*FogOfWar.RAYCAST_HEIGHT);
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
		//Debug.DrawRay(LOSorigin, LOSdir*LOSdist, Color.green);
		
		// By now it is assumed that we hit the fog of war mesh, so we use it
		// directly and not hit.collider.gameObject.GetComponent(MeshFilter).mesh;
		
		// Get which vertices were hit
		int p0 = fogOfWarMesh.triangles[hit.triangleIndex * 3 + 0];
		int p1 = fogOfWarMesh.triangles[hit.triangleIndex * 3 + 1];
		int p2 = fogOfWarMesh.triangles[hit.triangleIndex * 3 + 2];
	
		fogOfWarScript.AddVertToReveal(p0);
		fogOfWarScript.AddVertToReveal(p1);
		fogOfWarScript.AddVertToReveal(p2);
	}
	
	void OnTriggerEnter(Collider other) {
		//TODO test vision triggering more, not sure if it's working right
		if(other.transform.root != this.transform.root) {
			if(other.CompareTag("Unit") || other.CompareTag("Building") || other.CompareTag("BuildProgress")) {
				unitControl.ObjectSighted(other.gameObject);
			}
		}
	}
}
