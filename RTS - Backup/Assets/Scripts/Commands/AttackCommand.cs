using UnityEngine;
using Lidgren.Network;

public class AttackCommand : Command {
	
	public int ownedObjectId;
	public int targetOwnedObjectId;
	
	public AttackCommand() : base(MessageType.AttackCommand) {}
	
	public AttackCommand(int ownedObjectId, int targetOwnedObjectId) : base(MessageType.AttackCommand) {
		this.ownedObjectId = ownedObjectId;
		this.targetOwnedObjectId = targetOwnedObjectId;
	}
	
	public override void SerializeTo(NetOutgoingMessage msg) {
		base.SerializeTo(msg);
		msg.Write(ownedObjectId);
		msg.Write(targetOwnedObjectId);
	}
	
	public override void DeserializeFrom(NetIncomingMessage msg) {
		base.DeserializeFrom(msg);
		ownedObjectId = msg.ReadInt32();
		targetOwnedObjectId = msg.ReadInt32();
	}
}