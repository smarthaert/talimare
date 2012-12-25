using UnityEngine;

// This class handles the execution of commands
public abstract class CommandExecutor {
	
	public static void ExecuteCommand(Command command) {
		if(command.commandType == CommandType.MoveCommand) {
			MoveCommand moveCommand = (MoveCommand)command;
			Game.GetObjectById(moveCommand.objectId).SendMessage("ExecuteMove", moveCommand.target);
		}
	}
}