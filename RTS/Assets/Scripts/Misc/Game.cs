using UnityEngine;

// This class can be used to control the overall game and any game-wide classes that are not MonoBehaviours
public class Game : MonoBehaviour {
	
	// If the game is paused, no new commands should be able to be issued by the player
	public static bool paused = false; //TODO implement pausing in update loop of most scripts
	
	void Update() {
		CommandHandler.Update();
	}
}