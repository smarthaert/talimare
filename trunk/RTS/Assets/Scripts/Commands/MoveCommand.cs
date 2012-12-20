using UnityEngine;

public class MoveCommand : Command {
	
	public int unitId;
	public Vector3 target;
	
	public MoveCommand(int unitId, Vector3 target) {
		this.unitId = unitId;
		this.target = target;
	}
}