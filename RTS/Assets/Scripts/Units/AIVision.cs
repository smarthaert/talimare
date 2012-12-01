using UnityEngine;
using System.Collections.Generic;

public class AIVision : MonoBehaviour {

	public float revealerRange = 4.0f;
	
	protected float LOSHeight = 0.0f;
	
	protected GameObject fogOfWar;
	protected Mesh fogOfWarMesh;
	protected FogOfWar fogOfWarScript;
	protected AIPathfinder pathfinder;
	
	// Layer mask for the fog layer only
	protected int fogLayerMask;
	// Layer mask for the layers that impede vision
	protected int LOSLayerMask;
	
	protected static float REVEALER_CIRCLE_SMOOTHNESS = 4.75f;
	
	void Start() {
		fogOfWar = GameObject.Find("FogOfWar");
		fogOfWarMesh = fogOfWar.GetComponent<MeshFilter>().mesh;
		fogOfWarScript = fogOfWar.GetComponent<FogOfWar>();
		pathfinder = GetComponent<AIPathfinder>();
		
		fogLayerMask = 1 << LayerMask.NameToLayer("FogOfWar");
		LOSLayerMask = (1 << LayerMask.NameToLayer("Terrain")) + (1 << LayerMask.NameToLayer("Building"));
	
		CalculateRevealPoints();
	}
	
	void Update() {
		// If this script is too inefficient, we can improve FogOfWar so that AIVisions don't need to recalculate vision while non-moving
		//if (pathfinder.IsMoving()) {
			CalculateRevealPoints();
		//}
	}
	
	void CalculateRevealPoints() {
		float x = transform.position.x;
		float z = transform.position.z;
		RevealFogOFWarAt(transform.position.x, transform.position.z);
		
		for (float i = 0.0f; i <= 2*Mathf.PI; i += REVEALER_CIRCLE_SMOOTHNESS/revealerRange) {
			RevealFogOFWarAt(x+(Mathf.Cos(i)*revealerRange), z+(Mathf.Sin(i)*revealerRange));
		}
	}
	
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
			Debug.Log("vision blocked!");
			Debug.DrawRay(LOSorigin, LOSdir*LOSdist, Color.red);
			return;
		}
		Debug.DrawRay(LOSorigin, LOSdir*LOSdist, Color.green);
		
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
}
