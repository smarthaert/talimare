
var newMaterial : Material;
var targetAlpha = 0.75;
var alphaAnimationSpeed = 0.5;

var gridSize = 3.0;
var xLength = 10;
var yLength = 10;


static var RAYCAST_HEIGHT = 50.0;

private var verticesToReveal = new Array();
private var mesh : Mesh;
private var colors : Color[];

private var resettingFog = false;
private var revealedVerts = new Array(); // the verts that need to be darkened again
private var initialRevealVerts = new Array(); // the verts that don't get darkened again since there are units on them
private var revealers = new Array();

function AddVert(pos : Vector3, x : float, z : float) : Vector3
{
	var hitInfo : RaycastHit;
	var layerMask = 1 << 10;
	Physics.Raycast(Vector3(x+pos.x,RAYCAST_HEIGHT,z+pos.z), -Vector3.up, hitInfo, Mathf.Infinity, layerMask);
	//Debug.Log((x+pos.x) + ", " + hitInfo.point.y + ", " + (z+pos.z));
	return Vector3(x,hitInfo.point.y,z);
}

function Awake()
{
	gameObject.AddComponent("MeshFilter");
	gameObject.AddComponent("MeshRenderer");
	mesh = GetComponent(MeshFilter).mesh;

	mesh.Clear();

	// vertices ---------------------------------------------------------------------------------
	var verts = new Array();

	var pos = transform.position;
	//var locZ = transform.position.z;

	for (var y = 0; y < yLength; ++y)
	{
		for (var x = 0; x < xLength; ++x)
		{
			verts.Push( AddVert(pos,(x*gridSize),(y*gridSize)) );
		}
	}
	mesh.vertices = verts.ToBuiltin(Vector3);

	// uv's ----------------------------------------------------------------------------------------
	var uvs = new  Vector2[verts.length];

	for (var i=0;i<uvs.Length;i++) {
		uvs[i] = Vector2(verts[i].x, verts[i].z);
	}
	mesh.uv = uvs;

	// triangles ---------------------------------------------------------------------------------
	var tris = new Array();
	//var trilen = (mesh.vertices.length-2)/2;

	for (y = 0; y < yLength-1; ++y)
	{
		for (x = 0; x < xLength-1; ++x)
		{
			var vertHere = x+(y*xLength);
			var vertAbove = x+((y+1)*xLength);
			tris.Push(vertHere); 
			tris.Push(vertAbove);
			tris.Push(vertAbove+1);

			tris.Push(vertHere);
			tris.Push(vertAbove+1);
			tris.Push(vertHere+1);
			//Debug.Log(x + "," + y + ": " +
			//	vertHere + "," + vertAbove + "," + (vertAbove+1) + " " +
			//	vertHere + "," + (vertAbove+1) + "," + (vertHere+1));
		}
	}
	mesh.triangles = tris.ToBuiltin(int);
	mesh.RecalculateNormals();
	mesh.RecalculateBounds();

	// vertex colors ------------------------------------------------------------------------------
	var vertices = mesh.vertices;
	colors = new Color[vertices.Length];

	for (i=0;i<vertices.Length;i++)
	{
		colors[i] = Color(0,0,0,targetAlpha);
	}
	mesh.colors = colors;


	renderer.castShadows = false;
	renderer.receiveShadows = false;
	renderer.material = newMaterial;
	
	gameObject.AddComponent("MeshCollider");
	var meshCollider : MeshCollider = GetComponent(MeshCollider);
	meshCollider.sharedMesh = mesh;
}

// same as Array.Push but doesn't
// push it if it already exists in that Array
function UniquePush(arr : Array, elementToAdd)
{
	var putIn = true;
	for (var i = 0; i < arr.length; ++i)
	{
		if (arr[i] == elementToAdd)
		{
			// already in, no need to put
			putIn = false;
			break;
		}
	}

	if (putIn)
	{
		arr.Push(elementToAdd);
	}
}


function AddVertToReveal(vertIdx : int, quickReveal : boolean)
{
	if (quickReveal)
	{
		if (colors[vertIdx].a >= targetAlpha)
		{
			colors[vertIdx].a = 0;
			mesh.colors = colors;
		}
		
		UniquePush(initialRevealVerts, vertIdx);
		//Debug.Log("initialRevealVerts now: " + initialRevealVerts.length);
	}
	else
	{
		if (colors[vertIdx].a <= 0.0)
		{
			return;
		}

		UniquePush(verticesToReveal, vertIdx);
	}
}

function IsVertRevealed(vertIdx : int) : boolean
{
	return (colors[vertIdx].a < targetAlpha);
}

function Update()
{
	//Debug.Log(verticesToReveal.length);

	if (verticesToReveal.length > 0)
	{
		for (var i = 0; i < verticesToReveal.length; ++i)
		{
			var vertIdxToReveal = verticesToReveal[i];

			colors[vertIdxToReveal].a -= alphaAnimationSpeed * Time.deltaTime;
			if (colors[vertIdxToReveal].a <= 0.0)
			{
				verticesToReveal.RemoveAt(i);
				if (!resettingFog)
				{
					UniquePush(revealedVerts, vertIdxToReveal);
				}
				--i;
				if (i <= -1)
				{
					i = 0;
				}
			}
		}
	}

	if (resettingFog)
	{
		if (revealedVerts.length == 0)
		{
			resettingFog = false;
		}
		else
		{
			//Debug.Log("darkening");
			for (i = 0; i < revealedVerts.length; ++i)
			{
				var vertIdxToDarken = revealedVerts[i];

				colors[vertIdxToDarken].a += alphaAnimationSpeed * Time.deltaTime;
				if (colors[vertIdxToDarken].a >= targetAlpha)
				{
					revealedVerts.RemoveAt(i);
					--i;
					if (i <= -1)
					{
						i = 0;
					}
				}
			}
		}
	}

	if ((verticesToReveal.length > 0) || resettingFog)
	{
		mesh.colors = colors;
	}
}



function ResetFog()
{
	// add contents of initialRevealVerts to revealedVerts
	for (idx in initialRevealVerts)
	{
		UniquePush(revealedVerts, idx);
	}

	// clear initialRevealVerts
	initialRevealVerts.Clear();
	//Debug.Log("initialRevealVerts now: " + initialRevealVerts.length);

	// tell all revealers to call their InitialReveal to repopulate initialRevealVerts
	//Debug.Log("revealers: " + revealers.length);
	for (r in revealers)
	{
		r.InitialReveal();
	}

	//Debug.Log("initialRevealVerts after all revealers to call their InitialReveal: " + initialRevealVerts.length);

	// remove elements of initialRevealVerts from revealedVerts
	// revealedVerts - initialRevealVerts
	for (initial in initialRevealVerts)
	{
		for (var i = 0; i < revealedVerts.length; ++i)
		{
			if (revealedVerts[i] == initial)
			{
				revealedVerts.RemoveAt(i);
				--i;
				if (i <= -1)
				{
					i = 0;
				}
			}
		}
	}


	// redarken verts found in revealedVerts
	resettingFog = true;

	//for (vertIdx in revealedVerts)
	//{
	//	colors[vertIdx].a = targetAlpha;
	//}
	//revealedVerts.Clear();
	//mesh.colors = colors;
}



function AddRevealer(r : FogOfWarRevealer)
{
	UniquePush(revealers, r);
}
