using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GuardVision : MonoBehaviour {

	public float MaxVisibilityDistance;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	public void Update()
	{
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

		// Filter out points not facing the same direction as the guard.
		HashSet<Vector2> filteredPoints = new HashSet<Vector2>();
		Direction currentDirection = gameObject.GetComponentInParent<DirectionComponent> ().Direction;
		Vector2 currentPosition = new Vector2 (transform.position.x, transform.position.y);
		foreach (Vector2 point in points) {
			if (MatchesVectorDirection(currentDirection, currentPosition, point)) {
				filteredPoints.Add(point);
			}
		}
		if (currentDirection == Direction.NORTH) {
			filteredPoints.Add (new Vector2 (currentPosition.x + 1, currentPosition.y + 1));
			filteredPoints.Add (new Vector2 (currentPosition.x + -1, currentPosition.y + 1));
		} else if (currentDirection == Direction.EAST) {
			filteredPoints.Add (new Vector2 (currentPosition.x + 1, currentPosition.y + 1));
			filteredPoints.Add (new Vector2 (currentPosition.x + 1, currentPosition.y + -1));
		} else if (currentDirection == Direction.SOUTH) {
			filteredPoints.Add (new Vector2 (currentPosition.x + 1, currentPosition.y + -1));
			filteredPoints.Add (new Vector2 (currentPosition.x + -1, currentPosition.y + -1));
		} else if (currentDirection == Direction.WEST) {
			filteredPoints.Add (new Vector2 (currentPosition.x + -1, currentPosition.y + -1));
			filteredPoints.Add (new Vector2 (currentPosition.x + -1, currentPosition.y + 1));
		}

		// cast a ray for each corners to find the enclosing polygon
		List<Vector2> extremeVisiblePoints = new List<Vector2>();
		foreach (Vector2 point in filteredPoints)
		{
			RaycastHit2D hit = Physics2D.Raycast(transform.position, point - (Vector2)transform.position, 9001, 1 << LayerMask.NameToLayer("VisibilityBlocking"));
			if (hit.collider != null)
			{
				extremeVisiblePoints.Add(hit.point - (Vector2)transform.position);
			}
		}

		// sort the points by angle (maybe hopefully?)
		List<Vector3> meshVertices = new List<Vector3>();
		meshVertices.AddRange(extremeVisiblePoints
			.OrderBy((p) => -Mathf.Atan2(p.y, p.x))
			.Select((v2) => new Vector3(v2.x, v2.y)));

		// Make sure Vector2.zero is inserted in the right place.
		if (currentDirection == Direction.WEST) {
			int index1 = FindIndex (meshVertices, new Vector3 (-1, -1));
			int index2 = FindIndex (meshVertices, new Vector3 (-1, 1));

			if (index1 > index2) {
				meshVertices.Insert (index1, Vector2.zero);
			} else {
				meshVertices.Insert (index2, Vector2.zero);
			}
		} else {
			meshVertices.Insert (0, Vector2.zero);
		}

		Mesh visibleMesh = new Mesh();
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
			uvs.Add(new Vector2(vertex.magnitude / MaxVisibilityDistance, 0));
			//Debug.Log(vertex.magnitude * 100);
		}
		visibleMesh.uv = uvs.ToArray();

		MeshFilter filter = GetComponent<MeshFilter>();
		filter.mesh.Clear();
		filter.mesh = visibleMesh;
		GetComponent<MeshRenderer>();

		// Update polygon collider
		Vector2[] colliderVertices = new Vector2[visibleMesh.vertices.Count()];
		for (int i = 0; i < visibleMesh.vertices.Count(); i++) {
			colliderVertices [i] = new Vector2 (visibleMesh.vertices [i].x, visibleMesh.vertices [i].y);
		}
        GetComponent<PolygonCollider2D>().SetPath(0, colliderVertices);			
	}

    void OnTriggerEnter2D(Collider2D collider) {
        // Check if touching player.
        PlayerController player = collider.GetComponent<PlayerController>();
        if (player != null) {
            GetComponentInParent<ChaseState>().HandlePlayerSpotted(player.transform.position);
        }
    }

	bool MatchesVectorDirection(Direction currentDirection, Vector2 rootOfVector, Vector2 endOfVector) {
		Vector2 difference = endOfVector - rootOfVector;
		if (Math.Abs (difference.x) == Math.Abs (difference.y)) {
			return false;
		} else if (Math.Abs (difference.x) > Math.Abs(difference.y)) {
			if (difference.x > 0) {
				return currentDirection == Direction.EAST;
			} else {
				return currentDirection == Direction.WEST;
			}
		} else {
			if (difference.y > 0) {
				return currentDirection == Direction.NORTH;
			} else {
				return currentDirection == Direction.SOUTH;
			}
		}
	}

	int FindIndex(List<Vector3> inMe, Vector3 ofMe) {
		for (int i = 0; i < inMe.Count; i++) {
			Vector2 norm1 = inMe [i];
			Vector2 norm2 = ofMe;
			norm1.Normalize ();
			norm2.Normalize ();
			if (Math.Abs (norm1.x - norm2.x) < .00001 && Math.Abs (norm1.y - norm2.y) < .00001) {
				return i;
			}
		}
		return -2;
	}
}
