using Lidgren.Network;
using UnityEngine;

// This class handles messages received from the network
public abstract class MessageHandler {
	
	public static void HandleMessage(NetIncomingMessage msg) {
		Debug.Log("parsing msg: "+msg);
		int messageType = msg.ReadInt32();
		switch(messageType) {
			case (int)MessageType.TurnDoneMessage:
				TurnDoneMessage turnDoneMessage = new TurnDoneMessage();
				turnDoneMessage.DeserializeFrom(msg);
				CommandHandler.StoreTurnDoneMessage(turnDoneMessage);
				break;
			case (int)MessageType.MoveCommand:
				MoveCommand moveCommand = new MoveCommand();
				moveCommand.DeserializeFrom(msg);
				CommandHandler.AddCommandFromNetwork(moveCommand);
				break;
			case (int)MessageType.AttackCommand:
				/*AttackCommand attackCommand = new AttackCommand();
				attackCommand.DeserializeFrom(msg);
				CommandHandler.AddCommandFromNetwork(attackCommand);*/
				break;
		}
	}
}
