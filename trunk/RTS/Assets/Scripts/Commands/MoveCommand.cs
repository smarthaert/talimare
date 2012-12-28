using UnityEngine;
using Lidgren.Network;

public class MoveCommand : Command {
	
	public int objectId;
	public Vector3 target;
	
	public MoveCommand() : base(MessageType.MoveCommand) {}
	
	public MoveCommand(int unitId, Vector3 target) : base(MessageType.MoveCommand) {
		this.objectId = unitId;
		this.target = target;
	}
	
	public override void SerializeTo(NetOutgoingMessage msg) {
		base.SerializeTo(msg);
		msg.Write(objectId);
		msg.Write(target.x);
		msg.Write(target.y);
		msg.Write(target.z);
	}
	
	public override void DeserializeFrom(NetIncomingMessage msg) {
		base.DeserializeFrom(msg);
		objectId = msg.ReadInt32();
		target = new Vector3();
		target.x = msg.ReadFloat();
		target.y = msg.ReadFloat();
		target.z = msg.ReadFloat();
	}
}