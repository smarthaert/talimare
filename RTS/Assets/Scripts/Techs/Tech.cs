using UnityEngine;
using System.Collections;

// Root class for all tech scripts
public class Tech : MonoBehaviour {

	public virtual void Execute() {
		Debug.Log(this+" research completed!");
	}
}
