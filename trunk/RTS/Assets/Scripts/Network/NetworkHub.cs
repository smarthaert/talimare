using UnityEngine;

public abstract class NetworkHub {
	
	// Returns the number of other clients we're communicating with
	public static int GetNumOtherClients() {
		return 2; //TODO network layer!
	}
	
	public static void SendMessage(Message message) {
		
	}
	
	public static void SendCommand(Command command) {
		
	}
}