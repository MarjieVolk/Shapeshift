﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GuardVision : MonoBehaviour {

	public float MaxVisibilityDistance;

    private List<FurnitureItem> furnitureInSight;
    private System.Random random;

	// Use this for initialization
	void Start () {
        furnitureInSight = new List<FurnitureItem>();
        random = new System.Random();
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
		Direction currentDirection = gameObject.GetComponentInParent<DirectionComponent> ().Direction;
		Vector2 currentPosition = new Vector2 (transform.position.x, transform.position.y);
        HashSet<Vector2> filteredPoints = filterByDirection(currentDirection, currentPosition, points);
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
			RaycastHit2D hit = Physics2D.Raycast(transform.position, point - (Vector2)transform.position, MaxVisibilityDistance, 1 << LayerMask.NameToLayer("VisibilityBlocking"));
            if (hit.collider != null)
            {
                extremeVisiblePoints.Add(hit.point - (Vector2)transform.position);
            }
            else
            {
                Vector2 extremePoint = (point - (Vector2)transform.position).normalized;
                extremePoint.Scale(new Vector2(MaxVisibilityDistance, MaxVisibilityDistance));
                extremeVisiblePoints.Add(extremePoint);
            }
		}

        //offset the angle sort if necessary so the circle isn't split in the guard's flashlight
        Func<Vector2, float> orderingFunction = (p) => -Mathf.Atan2(p.y, p.x);
        if (currentDirection == Direction.WEST)
        {
            // basically flip west and east :-P
            orderingFunction = (p) => -Mathf.Atan2(p.y, -p.x);
        }

		// sort the points by angle (actually definitely)
		List<Vector3> meshVertices = new List<Vector3>();
		meshVertices.AddRange(extremeVisiblePoints
			.OrderBy(orderingFunction)
			.Select((v2) => new Vector3(v2.x, v2.y)));

		// Make sure Vector2.zero is inserted in the right place.
        meshVertices.Insert (0, Vector2.zero);

        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh visibleMesh = filter.mesh;
        visibleMesh.Clear();
        visibleMesh.SetVertices(meshVertices);
		visibleMesh.triangles = Triangulate(meshVertices).ToArray();
		visibleMesh.uv = makeUVs(meshVertices).ToArray();

		// Update polygon collider
		Vector2[] colliderVertices = new Vector2[visibleMesh.vertices.Count()];
		for (int i = 0; i < visibleMesh.vertices.Count(); i++) {
			colliderVertices [i] = new Vector2 (visibleMesh.vertices [i].x, visibleMesh.vertices [i].y);
		}
        GetComponent<PolygonCollider2D>().SetPath(0, colliderVertices);	
	}

    List<int> Triangulate(List<Vector3> meshVertices)
    {
        List<int> triangleIndices = new List<int>();
        for (int i = 0; i < meshVertices.Count - 2; i++)
        {
            triangleIndices.Add(0);
            triangleIndices.Add(i + 1);
            triangleIndices.Add(i + 2);
        }
        return triangleIndices;
    }

    List<Vector2> makeUVs(List<Vector3> meshVertices)
    {
        List<Vector2> uvs = new List<Vector2>();
        foreach (Vector3 vertex in meshVertices)
        {
            uvs.Add(new Vector2(vertex.magnitude / MaxVisibilityDistance, 0));
        }
        return uvs;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.GetComponent<FurnitureItem>() != null) {
            furnitureInSight.Add(collider.GetComponent<FurnitureItem>());
            collider.GetComponent<CollisionEventCommunicator>().OnDestroyed += () => {
                furnitureInSight.Remove(collider.GetComponent<FurnitureItem>());
            };
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.GetComponent<FurnitureItem>() != null) {
            furnitureInSight.Remove(collider.GetComponent<FurnitureItem>());
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        PlayerTransformer player = collider.GetComponent<PlayerTransformer>();
        if (player != null && !player.GetComponent<PlayerCaughtHandler>().isOnCatchCooldown()) {
            // Guard sees the player
            if (player.getTransformation() == null) {
                // Player is human, begin chasing
                GetComponentInParent<NoticingState>().HandlePlayerDetected();
            } else if (transform.parent.GetComponent<StateMachine>().CurrentState != transform.parent.GetComponent<InvestigatingFurnitureState>()) {
                // Player is furniture and guard is not already in investiate state, begin suspicion analysis.
                int maxSuspicion = 0;
                List<FurnitureItem> mostSuspicious = new List<FurnitureItem>();
                foreach (FurnitureItem item in furnitureInSight) {
                    int suspicion = Suspicionomatic.getSuspicionLevel(item);

                    if (suspicion > maxSuspicion) {
                        Debug.Log("" + item + " is more suspicious at " + suspicion);
                        maxSuspicion = suspicion;
                        mostSuspicious.Clear();
                        mostSuspicious.Add(item);
                    } else if (suspicion == maxSuspicion) {
                        mostSuspicious.Add(item);
                    }
                }

                if (maxSuspicion > 0) {
                    FurnitureItem toMove = mostSuspicious[random.Next(mostSuspicious.Count)];
                    transform.parent.GetComponent<InvestigatingFurnitureState>().HandleFurnitureInvestigation(toMove);
                }
            }
        }
    }
    
    HashSet<Vector2> filterByDirection(Direction currentDirection, Vector2 position, HashSet<Vector2> points)
    {
        Vector2 directionVector = new Vector2(-1, 0);
        switch (currentDirection)
        {
            case Direction.EAST:
                directionVector = new Vector2(1, 0);
                break;
            case Direction.NORTH:
                directionVector = new Vector2(0, 1);
                break;
            case Direction.SOUTH:
                directionVector = new Vector2(0, -1);
                break;
        }

        HashSet<Vector2> ret = new HashSet<Vector2>();
        foreach (Vector2 point in points)
        {
            Vector2 difference = point - position;
            float directionness = Vector2.Dot(directionVector, difference.normalized);
            if (directionness > .707162)
            {
                ret.Add(point);
            }
        }

        return ret;
    }

	int FindIndex(List<Vector3> inMe, Vector3 ofMe) {
        Vector3 norm2 = ofMe.normalized;
		for (int i = 0; i < inMe.Count; i++) {
            if ((norm2 - inMe[i].normalized).sqrMagnitude < .00001)
            {
                return i;
            }
		}
		return 0;
	}
}
