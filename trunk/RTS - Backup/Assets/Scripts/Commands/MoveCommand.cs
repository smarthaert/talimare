using UnityEngine;
using Lidgren.Network;

public class MoveCommand : Command {
	
	public int ownedObjectId;
	public Vector3 target;
	
	public MoveCommand() : base(MessageType.MoveCommand) {}
	
	public MoveCommand(int ownedObjectId, Vector3 target) : base(MessageType.MoveCommand) {
		this.ownedObjectId = ownedObjectId;
		this.target = target;
	}
	
	public override void SerializeTo(NetOutgoingMessage msg) {
		base.SerializeTo(msg);
		msg.Write(ownedObjectId);
		msg.Write(target.x);
		msg.Write(target.y);
		msg.Write(target.z);
	}
	
	public override void DeserializeFrom(NetIncomingMessage msg) {
		base.DeserializeFrom(msg);
		ownedObjectId = msg.ReadInt32();
		target = new Vector3();
		target.x = msg.ReadFloat();
		target.y = msg.ReadFloat();
		target.z = msg.ReadFloat();
	}
}