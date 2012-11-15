using UnityEngine;
using System.Collections;

public class BaseControl : BuildingControl {
	
	public GameObject civilian;
	public KeyCode civilianTrainKey = KeyCode.C;
	public float civilianTrainTime = 5;

	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
	}
	
	public override void KeyPressed() {
		if(Input.GetKeyDown(civilianTrainKey)) {
			TrainUnit(new TrainingTask(civilian, civilianTrainTime));
		}
	}
}
