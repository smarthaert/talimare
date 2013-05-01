using UnityEngine;
using System.Collections;

// Keeps information about a building's current status
public class BuildingStatus : ControllableStatus {
	
	public int powerRequired;
	public bool Powered { get; set; }
	
	protected override void Start() {
		base.Start();
		
		Powered = false;
	}
	
	protected override void Update() {
		base.Update();
	}
}
