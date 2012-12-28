using UnityEngine;

// This class handles the execution of commands
public abstract class CommandExecutor {
	
	public static void ExecuteCommand(Command command) {
		if(command.messageType == MessageType.MoveCommand) {
			MoveCommand moveCommand = (MoveCommand)command;
			Game.GetOwnedObjectById(moveCommand.ownedObjectId).SendMessage("ExecuteMove", moveCommand.target);
		}
	}
}