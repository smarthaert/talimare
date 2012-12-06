using UnityEngine;
using System.Collections.Generic;

// This class keeps track of all the players currently in the game
public class PlayerHub : MonoBehaviour {
	
	// Set of all players
	protected static HashSet<Player> players = new HashSet<Player>();
	
	// Adds a player to the hub, and also adds the player to their team
	public static void AddPlayer(Player player) {
		players.Add(player);
		player.team.players.Add(player);
	}
}
