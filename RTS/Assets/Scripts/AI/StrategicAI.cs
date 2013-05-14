using UnityEngine;
using System.Collections.Generic;

// This class is the base for strategic AI.
// It represents the overall decision-making ability of a player (human or computer) based on the current state of the player's objects
[RequireComponent(typeof(Player))]
public class StrategicAI : MonoBehaviour {
	
	protected Player Player { get; set; }
	
	protected List<Job> Jobs { get; set; }
	
	protected void Start() {
		Player = GetComponent<Player>();
		
		Jobs = new List<Job>();
	}
	
	public void AddJob(Job job) {
		Jobs.Add(job);
	}
	
	// Assigns the next available job to the given Controllable
	public bool AssignJob(Controllable assignee, bool appendToTaskQueue) {
		foreach(Job job in Jobs) {
			if(job.AssignNextJob(assignee, appendToTaskQueue)) {
				return true;
			}
		}
		return false;
	}
	
	public void JobComplete(Job job) {
		Jobs.Remove(job);
	}
}