using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

// Keeps information about a player's current status
public class PlayerStatus : MonoBehaviour {
	
	// Resources
	public int food = 0;
	public int water = 0;
	public int electricity = 0;
	public int copper = 0;
	public int iron = 0;
	public int coal = 0;
	public int steel = 0;
	public int oil = 0;
	public int uranium = 0;
	public int unobtanium = 0;
	
	public Dictionary<Resource, int> resources;

	// Use this for initialization
	void Start () {
		//add each externally-set resource to the dictionary so we can manage player resources easier
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
