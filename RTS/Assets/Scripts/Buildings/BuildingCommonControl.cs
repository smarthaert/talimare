using UnityEngine;
using System.Collections.Generic;

// Contains general building utility functions (buildings and build progress
public abstract class BuildingCommonControl : Controllable {
	
	// The resources which are currently stored in this building in order to begin building or training (initialize in sub-classes)
	public Dictionary<Resource, int> StoredResources { get; protected set; }
}