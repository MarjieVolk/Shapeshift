using System;
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
        float currentAngle = gameObject.GetComponentInParent<DirectionComponent>().Angle;
		Vector2 currentPosition = new Vector2 (transform.position.x, transform.position.y);
        HashSet<Vector2> filteredPoints = filterByDirection(new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)), currentPosition, points);
        filteredPoints.Add(currentPosition + new Vector2(Mathf.Cos(currentAngle - Mathf.PI / 4), Mathf.Sin(currentAngle - Mathf.PI / 4)));
        filteredPoints.Add(currentPosition + new Vector2(Mathf.Cos(currentAngle + Mathf.PI / 4), Mathf.Sin(currentAngle + Mathf.PI / 4)));

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
        Direction currentDirection = gameObject.GetComponentInParent<DirectionComponent>().Direction;
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
    
    HashSet<Vector2> filterByDirection(Vector2 directionVector, Vector2 position, HashSet<Vector2> points)
    {
        HashSet<Vector2> ret = new HashSet<Vector2>();
        foreach (Vector2 point in points)
        {
            Vector2 difference = point - position;
            //directionness here really means 'projection of difference onto directionVector'
            //but it's, you know, funnier
            float directionness = Vector2.Dot(directionVector, difference.normalized);
            //this being sqrt(2), this is true iff this vector is less than 45 degrees off of directionVector
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
