using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Misc RTS/Fog Of War")]
public class FogOfWar : MonoBehaviour {
	
	public Material newMaterial;
	public float undiscoveredAlpha = 1f;
	public float fogAlpha = 0.5f;
	public float alphaAnimationSpeed = 0.8f;
	
	public float gridSize = 5.0f;
	public int xLength = 10;
	public int yLength = 10;
	
	public static float RAYCAST_HEIGHT = 50.0f;
	
	protected Mesh mesh;
	protected List<Color> colors;
	
	// Provides a public shortcut to the mesh's triangles array, since calling mesh.triangles
	// actually creates a copy of that huge array for every access
	public int[] meshTriangles;
	
	// Holds all verts that became visible to a unit
	protected List<int> verticesToReveal = new List<int>();
	// Holds all verts that are fully revealed
	protected List<int> revealedVerts = new List<int>(); // the verts that need to be darkened again
	
	// Builds the fog mesh and colors list based on the settings provided
	void Awake() {
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		mesh = GetComponent<MeshFilter>().mesh;
	
		mesh.Clear();
	
		// vertices ---------------------------------------------------------------------------------
		List<Vector3> verts = new List<Vector3>();
	
		Vector3 pos = transform.position;
		//var locZ = transform.position.z;
	
		for (int y = 0; y < yLength; ++y) {
			for (int x = 0; x < xLength; ++x) {
				verts.Add( AddVert(pos,(x*gridSize),(y*gridSize)) );
			}
		}
		mesh.vertices = verts.ToArray();
	
		// uv's ----------------------------------------------------------------------------------------
		List<Vector2> uvs = new List<Vector2>(verts.Count);
	
		for (int i=0; i<verts.Count; i++) {
			uvs.Add(new Vector2(verts[i].x, verts[i].z));
		}
		mesh.uv = uvs.ToArray();
	
		// triangles ---------------------------------------------------------------------------------
		List<int> tris = new List<int>();
	
		for (int y = 0; y < yLength-1; ++y) {
			for (int x = 0; x < xLength-1; ++x) {
				int vertHere = x+(y*xLength);
				int vertAbove = x+((y+1)*xLength);
				tris.Add(vertHere); 
				tris.Add(vertAbove);
				tris.Add(vertAbove+1);
	
				tris.Add(vertHere);
				tris.Add(vertAbove+1);
				tris.Add(vertHere+1);
			}
		}
		mesh.triangles = tris.ToArray();
		meshTriangles = tris.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	
		// vertex colors ------------------------------------------------------------------------------
		colors = new List<Color>(mesh.vertices.Length);
	
		for (int i=0; i<mesh.vertices.Length; i++) {
			colors.Add(new Color(0, 0, 0, undiscoveredAlpha));
		}
		mesh.colors = colors.ToArray();
	
	
		renderer.castShadows = false;
		renderer.receiveShadows = false;
		renderer.material = newMaterial;
		
		gameObject.AddComponent("MeshCollider");
		MeshCollider meshCollider = GetComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	}
	
	
	Vector3 AddVert(Vector3 pos, float x, float z) {
		RaycastHit hitInfo;
		int layerMask = 1 << LayerMask.NameToLayer("FogOfWar");
		Physics.Raycast(new Vector3(x+pos.x, RAYCAST_HEIGHT, z+pos.z), -Vector3.up, out hitInfo, Mathf.Infinity, layerMask);
		return new Vector3(x, hitInfo.point.y, z);
	}
	
	// Adds a vert that should be revealed because it is visible to some unit
	public void AddVertToReveal(int vertIdx) {
		if (colors[vertIdx].a > 0.0f && !verticesToReveal.Contains(vertIdx)) {
			verticesToReveal.Add(vertIdx);
		}
	}
	
	public bool IsVertRevealed(int vertIdx) {
		return (colors[vertIdx].a < fogAlpha);
	}
	
	public bool IsVertDiscovered(int vertIdx) {
		return (colors[vertIdx].a < undiscoveredAlpha);
	}
	
	void Update() {
		// Darken all revealed verts
		for (int i = 0; i < revealedVerts.Count; ++i) {
			int vertIdxToDarken = revealedVerts[i];
			
			/*
			 * to optimize, we could keep a dictionary of verts with a list of non-moving aivisions that can see them
			 * then ignore verts in that dictionary to darken. that way non-moving aivisions wouldn't have to recalculate their vision every frame
			 */
			
			// Except don't darken a vert if it is supposed to be revealed right now
			if(!verticesToReveal.Contains(vertIdxToDarken)) {
				Color color = colors[vertIdxToDarken];
				color.a += alphaAnimationSpeed * Time.deltaTime;
				colors[vertIdxToDarken] = color;
				if(color.a >= fogAlpha) {
					// Vert is totally darkened, so remove it from revealedVerts
					revealedVerts.RemoveAt(i);
					--i;
					if(i <= -1) {
						i = 0;
					}
				}
			}
		}
		
		// And lighten all visible ones
		for (int j = 0; j < verticesToReveal.Count; ++j) {
			int vertIdxToReveal = verticesToReveal[j];
			
			// Except don't lighten a vert if it is already totally revealed
			if(!revealedVerts.Contains(vertIdxToReveal)) {
				Color color = colors[vertIdxToReveal];
				color.a -= alphaAnimationSpeed * Time.deltaTime;
				colors[vertIdxToReveal] = color;
				if(color.a <= 0.0f) {
					// Vert is totally transparent, so remove it from verticesToReveal and add it to revealedVerts
					if(!revealedVerts.Contains(vertIdxToReveal)) {
						revealedVerts.Add(vertIdxToReveal);
					}
					verticesToReveal.RemoveAt(j);
					--j;
					if(j <= -1) {
						j = 0;
					}
				}
			} else {
				verticesToReveal.RemoveAt(j);
			}
		}
		
		mesh.colors = colors.ToArray();
	}

}