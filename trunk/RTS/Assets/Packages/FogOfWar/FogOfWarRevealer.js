
var revealerRange = 1.0;

var fogOfWar : GameObject;
private var fogOfWarMesh : Mesh;
private var fogOfWarScript : FogOfWar;
var LOSHeight = 5.0;

static var INITIAL_REVEALER_CIRCLE_SMOOTHNESS = 0.1;
static var REVEALER_CIRCLE_SMOOTHNESS = 4.75;

function Start()
{
	fogOfWarMesh = fogOfWar.GetComponent(MeshFilter).mesh;
	fogOfWarScript = fogOfWar.GetComponent(FogOfWar);

	fogOfWarScript.AddRevealer(this);

	InitialReveal();
}

function InitialReveal()
{
	//Debug.Log("InitialReveal()");
	var x = transform.position.x;
	var z = transform.position.z;
	RevealFogOFWarAt(x, z, true);

	var n : float;
	for (rangeStep = 0.0; rangeStep <= revealerRange; rangeStep+=5)
	{
		for (n = 0.0; n <= 2*Mathf.PI; n += INITIAL_REVEALER_CIRCLE_SMOOTHNESS)
		{
			//Debug.Log(n);
			RevealFogOFWarAt(x+(Mathf.Cos(n)*rangeStep),z+(Mathf.Sin(n)*rangeStep), true);
		}
	}
}

function Update()
{
	//var c : AIPathfinder = GetComponent(AIPathfinder);
	//if (c.IsMoving())
	//{
		var x = transform.position.x;
		var z = transform.position.z;

		for (i = 0.0; i <= 2*Mathf.PI; i += REVEALER_CIRCLE_SMOOTHNESS/revealerRange)
		{
			RevealFogOFWarAt(x+(Mathf.Cos(i)*revealerRange),z+(Mathf.Sin(i)*revealerRange), false);
		}
	//}
}


function RevealFogOFWarAt(x : float, z : float, quickReveal : boolean)
{
	var hit : RaycastHit;
	var layerMask = 1 << LayerMask.NameToLayer("FogOfWar");

	Debug.DrawRay(Vector3(x,FogOfWar.RAYCAST_HEIGHT,z), -Vector3.up*FogOfWar.RAYCAST_HEIGHT);
	if (!Physics.Raycast(Vector3(x,FogOfWar.RAYCAST_HEIGHT,z), -Vector3.up, hit, Mathf.Infinity, layerMask))
	{
		//Debug.Log("raycast did not hit anything");
		return;
	}
	// Just in case, also make sure the collider also has a renderer
	// material and texture
	var meshCollider = hit.collider as MeshCollider;
	if (meshCollider == null || meshCollider.sharedMesh == null)
	{
		return;
	}

	//Debug.Log(hit.collider.gameObject.name);


	// checking Line Of Sight (LOS):
	// if we have no line of sight to that area,
	// abort, so we don't reveal the fog of war there
	var LOSorigin = transform.position + Vector3(0, LOSHeight, 0);
	var LOSdir = (hit.point + Vector3(0, 3, 0)) - LOSorigin;
	var LOSdist = LOSdir.magnitude;
	LOSdir.Normalize();

	var LOShit : RaycastHit;
	var LOSLayerMask = (1 << 0) + (1 << 10);
	if (Physics.Raycast(LOSorigin, LOSdir, LOShit, LOSdist, LOSLayerMask))
	{
		Debug.DrawRay(LOSorigin, LOSdir*LOSdist, Color.red);
		return;
	}
	Debug.DrawRay(LOSorigin, LOSdir*LOSdist, Color.green);



	// here it is assumed that what we hit is the fog of war mesh, so we use it
	// directly and not hit.collider.gameObject.GetComponent(MeshFilter).mesh;
	var triangles = fogOfWarMesh.triangles;

	// get which vertices were hit
	var p0 = triangles[hit.triangleIndex * 3 + 0];
	var p1 = triangles[hit.triangleIndex * 3 + 1];
	var p2 = triangles[hit.triangleIndex * 3 + 2];

	fogOfWarScript.AddVertToReveal(p0, quickReveal);
	fogOfWarScript.AddVertToReveal(p1, quickReveal);
	fogOfWarScript.AddVertToReveal(p2, quickReveal);
}
