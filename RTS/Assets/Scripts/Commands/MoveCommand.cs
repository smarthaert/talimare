using UnityEngine;

public class MoveCommand : Command {
	
	public int objectId;
	public Vector3 target;
	
	public MoveCommand(int unitId, Vector3 target) : base(CommandType.MoveCommand) {
		this.objectId = unitId;
		this.target = target;
	}
}