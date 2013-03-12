using UnityEngine;

// This class handles the execution of commands
public abstract class CommandExecutor {
	
	public static void ExecuteCommand(Command command) {
		switch(command.messageType) {
			case MessageType.MoveCommand:
				MoveCommand moveCommand = (MoveCommand)command;
				Game.GetOwnedObjectById(moveCommand.ownedObjectId).SendMessage("ExecuteMove", moveCommand.target);
				break;
			case MessageType.AttackCommand:
				AttackCommand attackCommand = (AttackCommand)command;
				Game.GetOwnedObjectById(attackCommand.ownedObjectId).SendMessage("ExecuteAttack", Game.GetOwnedObjectById(attackCommand.targetOwnedObjectId));
				break;
		}
	}
}