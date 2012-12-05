using UnityEngine;
using UnityEditor;

public class RTSGameObjects {
	
	[MenuItem("GameObject/Create Other/RTS Objects/Unit", false, 1100)]
	static void AddUnit() {
		GameObject unit = GameObject.CreatePrimitive(PrimitiveType.Capsule);
		unit.name = "Unit";
		unit.tag = "Unit";
		Object.DestroyImmediate(unit.GetComponent<CapsuleCollider>());
		unit.AddComponent<CharacterController>();
		unit.GetComponent<CharacterController>().radius = 0.5f;
		unit.GetComponent<CharacterController>().height = 2.0f;
		unit.AddComponent<Creatable>();
		unit.AddComponent<AIPathfinder>();
		unit.AddComponent<AIAttacker>();
		unit.AddComponent<UnitStatus>();
		unit.AddComponent<UnitControl>();
		
		GameObject vision = new GameObject("Vision");
		vision.transform.parent = unit.transform;
		vision.layer = LayerMask.NameToLayer("Ignore Raycast");
		vision.AddComponent<AIVision>();
		
		unit.transform.position = new Vector3(0, 1, 0);
	}
}
