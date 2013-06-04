using UnityEngine;

// Handles unit gathering
[RequireComponent(typeof(MoveTaskScript))]
[RequireComponent(typeof(MoveResourceTaskScript))]
public class GatherTaskScript : MonoBehaviour {
	
	// The amount this unit gathers each time gathering triggers on a resource node
	public int gatherAmount;
	
	// Gathering triggers once per this many seconds
	protected float gatherTime = 5;
	
	// The resource node this unit is currently gathering from
	protected ResourceNode GatherTarget { get; set; }
	protected float GatherTimer { get; set; }
	
	protected Controllable Controllable { get; set; }
	protected MoveTaskScript MoveTaskScript { get; set; }
	protected MoveResourceTaskScript MoveResourceTaskScript { get; set; }
	
	protected void Awake() {
		Controllable = GetComponent<Controllable>();
		MoveTaskScript = GetComponent<MoveTaskScript>();
		MoveResourceTaskScript = GetComponent<MoveResourceTaskScript>();
	}
	
	protected void Update () {
		if(GatherTarget != null) {
			UpdateGather();
		}
	}
	
	protected void UpdateGather() {
		if(GatherTimer > 0) {
			// Currently gathering
			GatherTimer -= Time.deltaTime;
			if(GatherTimer <= 0) {
				// Timer's up, trigger gather from node if still in range
				if(IsInRange(GatherTarget.gameObject)) {
					int gatheredAmount = GatherTarget.GatherFrom(Mathf.Min(gatherAmount, MoveResourceTaskScript.HeldResourceSpaceAvailable(GatherTarget.resource)));
					MoveResourceTaskScript.HoldResource(GatherTarget.resource, gatheredAmount);
					if(MoveResourceTaskScript.HeldResourceSpaceAvailable(GatherTarget.resource) == 0) {
						// Return gathered resources to depot
						MoveResourceTaskScript.StartTask();
					} else {
						GatherTimer = gatherTime;
					}
				}
			}
		} else {
			// Not currently gathering (either due to being out of range, or just haven't started yet)
			if(IsInRange(GatherTarget.gameObject)) {
				// In range, start gathering
				MoveTaskScript.StopTask();
				GatherTimer = gatherTime;
			} else {
				// Not in range, make sure we're moving toward node
				MoveTaskScript.StartTask(GatherTarget.transform);
			}
		}
	}
	
	public void StartTask(ResourceNode target) {
		if(GatherTarget != target) {
			GatherTarget = target;
			GatherTimer = 0;
			// Return any other incorrect held resources to a depot before beginning
			if(MoveResourceTaskScript.HeldResourceSpaceAvailable(GatherTarget.resource) == 0) {
				MoveResourceTaskScript.StartTask();
			}
		}
	}
	
	public bool IsTaskRunning() {
		return GatherTarget != null;
	}
	
	public void StopTask() {
		GatherTarget = null;
		MoveTaskScript.StopTask();
	}
	
	protected bool IsInRange(GameObject gameObject) {
		float range = this.collider.bounds.size.magnitude/2 + gameObject.collider.bounds.size.magnitude/2 + 0.5f;
		return (gameObject.transform.position - this.transform.position).magnitude <= range;
	}
}
