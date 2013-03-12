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
		
		AddVision(unit);
		
		unit.transform.position = new Vector3(0, 1, 0);
	}
	
	[MenuItem("GameObject/Create Other/RTS Objects/Building", false, 1101)]
	static void AddBuilding() {
		GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
		building.name = "Building";
		building.tag = "Building";
		building.layer = LayerMask.NameToLayer("Building");
		building.transform.localScale = new Vector3(2, 2, 2);
		building.AddComponent<Creatable>();
		building.AddComponent<BuildingControl>();
		building.AddComponent<BuildingStatus>();
		
		AddVision(building);
		
		building.transform.position = new Vector3(0, 1, 0);
	}
	
	static void AddVision(GameObject obj) {
		GameObject vision = new GameObject("Vision");
		vision.transform.parent = obj.transform;
		vision.layer = LayerMask.NameToLayer("Ignore Raycast");
		vision.AddComponent<AIVision>();
	}
}
