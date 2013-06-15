using UnityEngine;

[RequireComponent(typeof(Controllable))]
[AddComponentMenu("Misc RTS/Vision")]
public class Vision : MonoBehaviour {

	public float visionRange = 10.0f;
	//public int visionCasts = 6; trying out using visionRange for this instead
	public bool RevealsFog { get; set; }
	public bool HidesInFog { get; set; }
	
	// Cast LOS lines from this height above the center of this object
	protected float LOSHeight = 0.0f;
	
	protected FogOfWar fogOfWarScript;
	protected MoveTaskScript moveTaskScript;
	protected Controllable controllable;
	protected PersonalAI personalAI;
	
	protected bool isUnit = false;
	public bool IsHiddenByFog { get; set; }
	
	// Layer mask for the fog layer only
	protected int fogLayerMask;
	// Layer mask for the layers that impede vision
	protected int LOSLayerMask;
	// This determines how many rays we cast evenly around in a circle around this object
	protected float circleStep;
	
	protected SphereCollider VisionCollider { get; set; }
	
	protected void Awake() {
		// A child GameObject is needed to attach a collider to. Attaching the collider to the parent object causes problems
		GameObject child = new GameObject(this.GetType().Name);
		child.transform.parent = transform;
		child.transform.localPosition = Vector3.zero;
		child.layer = LayerMask.NameToLayer("Ignore Raycast");
		
		// A capsule collider provides a trigger for the vision range
		VisionCollider = child.AddComponent<SphereCollider>();
		VisionCollider.isTrigger = true;
		VisionCollider.radius = visionRange;
		
		// Determine if this is a unit (the alternative would be a building)
		if(gameObject.CompareTag(GameUtil.TAG_UNIT)) {
			isUnit = true;
			moveTaskScript = GetComponent<MoveTaskScript>();
		}
		controllable = GetComponent<Controllable>();
		personalAI = GetComponent<PersonalAI>();
	}
	
	protected void Start() {
		ConfigureVisionSettings();
		
		GameObject fogOfWar = GameObject.Find("FogOfWar");
		if(fogOfWar != null) {
			fogOfWarScript = fogOfWar.GetComponent<FogOfWar>();
		}
		
		fogLayerMask = 1 << LayerMask.NameToLayer("FogOfWar");
		LOSLayerMask = 1 << LayerMask.NameToLayer("Terrain");
		
		circleStep = ((2*Mathf.PI) / visionRange) - 0.0001f;
		
		if(RevealsFog && fogOfWarScript != null) {
			IsHiddenByFog = false;
			CalculateRevealPoints();
		}
		
		// Evaluate objects already colliding
		foreach(Collider collider in Physics.OverlapSphere(VisionCollider.transform.position, VisionCollider.radius)) {
			OnTriggerEnter(collider);
		}
	}
	
	// Configures this object's vision settings based on its owner
	protected void ConfigureVisionSettings() {
		if(controllable.Owner == Game.ThisPlayer) {
			RevealsFog = true;
		} else {
			RevealsFog = false;
		}
		
		if(gameObject.CompareTag(GameUtil.TAG_UNIT)) {
			HidesInFog = true;
		} else {
			HidesInFog = false;
		}
	}
	
	void Update() {
		if(RevealsFog && fogOfWarScript != null) {
		//if(isUnit && revealsFog) {
			// If this script is too inefficient, we can improve FogOfWar so that AIVisions don't need to recalculate vision while non-moving
			//if(pathfinder.IsMoving()) {
				CalculateRevealPoints();
			//}
		} else if(HidesInFog) {
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
		// directly and not hit.transform.gameObject.GetComponent(MeshFilter).mesh;
		
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
		// directly and not hit.transform.gameObject.GetComponent(MeshFilter).mesh;
	
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
		Renderer[] allRenderer = gameObject.GetComponentsInChildren<Renderer>();
		foreach(Renderer rendR in allRenderer) {
			rendR.enabled = false;
		}
		IsHiddenByFog = true;
	}
	
	// Shows this object, enabling all renderers
	void Show() {
		Renderer[] allRenderer = gameObject.GetComponentsInChildren<Renderer>();
		foreach(Renderer rendR in allRenderer) {
			rendR.enabled = true;
		}
		IsHiddenByFog = false;
	}
	
	// Called when another collider enters this vision range
	void OnTriggerEnter(Collider other) {
		if(IsControllableWithDifferentOwner(other) && personalAI != null) {
			personalAI.ObjectEnteredVision(other.gameObject);
		}
	}
	
	// Called when another collider exits this vision range
	void OnTriggerExit(Collider other) {
		if(IsControllableWithDifferentOwner(other) && personalAI != null) {
			personalAI.ObjectLeftVision(other.gameObject);
		}
	}
	
	protected bool IsControllableWithDifferentOwner(Collider other) {
		return other.GetComponent<Controllable>() != null && other.GetComponent<Controllable>().Owner != controllable.Owner;
	}
}
