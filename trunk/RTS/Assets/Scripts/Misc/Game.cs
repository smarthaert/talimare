using UnityEngine;

// This class can be used to control the overall game and any game-wide classes that are not MonoBehaviours
public class Game : MonoBehaviour {
	
	void Update() {
		CommandHandler.Update();
	}
}