using UnityEngine;

public abstract class NetworkHub {
	
	// The time each synchronization turn lasts, in ms
	protected static float timePerTurn = 200;
	// Number of turns to delay an incoming command
	protected static int commandTurnDelay = 2;
	
	// The current turn number
	protected static int turn = 0;
	
	public static void SendCommand(Command command) {
		
	}
}