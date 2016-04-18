using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class VisibilityHelper : MonoBehaviour
{
    public float maxVisibilityDistance;

	public Shader shoop;
    private Mesh visibleMesh;
	private RenderTexture shadowMap;
	private Mesh screenMesh;
	private Material playerLightMat;
	private Material shadowMat;

	public void Start() {
		visibleMesh = new Mesh ();
		screenMesh = new Mesh ();
		List<Vector3> verts = new List<Vector3> ();
		Vector3 origin = Camera.main.ScreenToWorldPoint (new Vector2 (0, 0));
		Vector3 outer = Camera.main.ScreenToWorldPoint (new Vector2 (Screen.width, Screen.height));

		verts.Add (new Vector3 (origin.x, origin.y, 0));
		verts.Add (new Vector3 (origin.x, outer.y, 0));
		verts.Add (new Vector3 (outer.x, outer.y, 0));
		verts.Add (new Vector3 (outer.x, origin.y, 0));
		screenMesh.SetVertices (verts);
		List<Vector2> uvs = new List<Vector2> ();
		uvs.Add (new Vector2 (0, 0));
		uvs.Add (new Vector2 (0, 1));
		uvs.Add (new Vector2 (1, 1));
		uvs.Add (new Vector2 (1, 0));
		screenMesh.uv = uvs.ToArray ();
		List<int> idxs = new List<int> ();
		idxs.Add (0);
		idxs.Add (1);
		idxs.Add (2);
		idxs.Add (2);
		idxs.Add (3);
		idxs.Add (0);
		screenMesh.triangles = idxs.ToArray ();

		// TODO(nelk): Shoop doesn't work; this doesn't work either.
		shadowMap 	= new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
		shadowMat = (Material)Resources.Load("SpecialShadowMat", typeof(Material));
		//shadowMat = new Material(shoop);
		shadowMat.mainTexture = shadowMap;

		playerLightMat = (Material)Resources.Load ("PlayerLightMaterial", typeof(Material));
	}

    public void Update()
    {
        Vector2 sourcePosition = new Vector2(transform.position.x, transform.position.y);

        // collect LOS blocking item in the scene
        BlocksLineOfSight[] blockers = GameObject.FindObjectsOfType<BlocksLineOfSight>();

        // Collect collider corners
        HashSet<Vector2> points = new HashSet<Vector2>();
        foreach (BlocksLineOfSight blocker in blockers)
        {
            GameObject blockingObject = blocker.gameObject;
            BoxCollider2D blockingCollider = blockingObject.GetComponent<BoxCollider2D>();
            Vector2 size = blockingCollider.size;
            Vector2 position = blockingCollider.offset + (Vector2) blockingCollider.transform.position;
            points.Add(position + size / 2.01f);
            points.Add(position + size / 1.99f);
            points.Add(position - size / 2.01f);
            points.Add(position - size / 1.99f);
            points.Add(position + Vector2.Reflect(size, Vector2.right) / 2.01f);
            points.Add(position + Vector2.Reflect(size, Vector2.right) / 1.99f);
            points.Add(position - Vector2.Reflect(size, Vector2.right) / 2.01f);
            points.Add(position - Vector2.Reflect(size, Vector2.right) / 1.99f);
        }

        // cast a ray for each corners to find the enclosing polygon
        List<Vector2> extremeVisiblePoints = new List<Vector2>();
        foreach (Vector2 point in points)
        {
            RaycastHit2D hit = Physics2D.Raycast(sourcePosition, point - sourcePosition, 9001, 1 << LayerMask.NameToLayer("VisibilityBlocking"));
            if (hit.collider != null)
            {
                extremeVisiblePoints.Add(hit.point - sourcePosition);
            }
        }

		visibleMesh = new Mesh ();
        // sort the points by angle (maybe hopefully?)
        List<Vector3> meshVertices = new List<Vector3>();
        meshVertices.Add(Vector2.zero);
        meshVertices.AddRange(extremeVisiblePoints
            .OrderBy((p) => -Mathf.Atan2(p.y, p.x))
            .Select((v2) => new Vector3(v2.x, v2.y)));
        meshVertices.Add(meshVertices[1]);
        visibleMesh.SetVertices(meshVertices);
        List<int> triangleIndices = new List<int>();
        for(int i = 0; i < meshVertices.Count - 2; i++)
        {
            triangleIndices.Add(0);
            triangleIndices.Add(i + 1);
            triangleIndices.Add(i + 2);
        }
        visibleMesh.triangles = triangleIndices.ToArray();

        List<Vector2> uvs = new List<Vector2>();
        foreach (Vector3 vertex in meshVertices)
        {
            uvs.Add(new Vector2(vertex.magnitude / maxVisibilityDistance, 0));
            //Debug.Log(vertex.magnitude * 100);
        }
        visibleMesh.uv = uvs.ToArray();

		//Graphics.DrawMesh (visibleMesh, new Vector3(0, 0, -2), Quaternion.identity, playerLightMat, 0);
		//Graphics.DrawMesh (visibleMesh, Camera.current.transform.position - , Quaternion.identity, (Material)Resources.Load("FailMat", typeof(Material)), 0);
    }

	private Mesh m;

    public void OnRenderObject() {
		// Render visibility mesh overtop of shadowmap.
		RenderTexture.active = shadowMap;
		GL.Clear(true, true, new Color(0.1f, 0.1f, 0.1f, 0.1f));
		playerLightMat.SetPass(0);
		Graphics.DrawMeshNow (visibleMesh, transform.position, Quaternion.identity);

		// Render cut out shadowmap onto screen.
		RenderTexture.active = null;
		shadowMat.SetPass(0);
		// Debug.Log (Camera.current.transform.position);
		Graphics.DrawMeshNow (screenMesh, Camera.current.transform.position + new Vector3(5, -2, -1), Quaternion.identity);
    }
}
