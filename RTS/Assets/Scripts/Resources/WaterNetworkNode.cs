using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Resources/Water Network Node")]
public class WaterNetworkNode : MonoBehaviour {
	
	// The water network this node currently belongs to
	protected WaterNetwork Network { get; set; }
	
	public float supplyRange;
	protected Controllable controllable;
	
	protected virtual void Awake() {
		controllable = GetComponent<Controllable>();
	}
	
	protected virtual void Start() {
		base.Start();
		
		//search for a network in range to join
	}
	
	
}