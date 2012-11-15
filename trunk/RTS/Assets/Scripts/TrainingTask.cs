using UnityEngine;

public class TrainingTask {
	public GameObject TrainingObject { get; set; }
	public float TrainingTime { get; set; }
	
	public TrainingTask (GameObject trainingObject, float trainingTime) {
		TrainingObject = trainingObject;
		TrainingTime = trainingTime;
	}
}