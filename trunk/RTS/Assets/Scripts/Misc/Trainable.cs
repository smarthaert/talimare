using UnityEngine;
using System.Collections.Generic;

// Defines values for a trainable object (unit or tech)
public class Trainable : MonoBehaviour {
	public KeyCode trainingKey;
	public int trainingTime;
	public List<ResourceAmount> resourceCosts;
}