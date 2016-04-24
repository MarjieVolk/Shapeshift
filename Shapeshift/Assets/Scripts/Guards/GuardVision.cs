using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GuardVision : MonoBehaviour {

	public float MaxVisibilityDistance;
    public int NumExtraRaycasts = 2;

    private List<FurnitureItem> furnitureInSight;
    private System.Random random;

    private static List<BoxCollider2D> blockers = new List<BoxCollider2D>();

    public static void addBlocker(BlocksLineOfSight blocksLineOfSight)
    {
        blockers.Add(blocksLineOfSight.GetComponent<BoxCollider2D>());
    }

    public static void removeBlocker(BlocksLineOfSight blocksLineOfSight)
    {
        blockers.Remove(blocksLineOfSight.GetComponent<BoxCollider2D>());
    }

	// Use this for initialization
	void Start () {
        furnitureInSight = new List<FurnitureItem>();
        random = new System.Random();
	}
	
	// Update is called once per frame
	public void Update()
	{
        List<Vector3> points = enumerateColliderPoints();

		// Filter out points not facing the same direction as the guard.
        float currentAngle = gameObject.GetComponentInParent<DirectionComponent>().Angle;
		Vector3 currentPosition = new Vector3 (transform.position.x, transform.position.y);
        List<Vector3> filteredPoints = filterByDirection(new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)), currentPosition, points);

        // Add raycasts spread throughout visible cone in case there's nothing nearby
        insertExtraRaycasts(filteredPoints, currentPosition, currentAngle);

		// cast a ray for each corners to find the enclosing polygon
        List<Vector3> extremeVisiblePoints = doRaycasts(filteredPoints);

        //offset the angle sort if necessary so the circle isn't split in the guard's flashlight
        Func<Vector3, float> orderingFunction = (p) => -Mathf.Atan2(p.y, p.x);
        Direction currentDirection = gameObject.GetComponentInParent<DirectionComponent>().Direction;
        if (currentDirection == Direction.WEST)
        {
            // basically flip west and east :-P
            orderingFunction = (p) => -Mathf.Atan2(p.y, -p.x);
        }

		// sort the points by angle (actually definitely)
		List<Vector3> meshVertices = new List<Vector3>();
		meshVertices.AddRange(extremeVisiblePoints
			.OrderBy(orderingFunction));

		// Make sure Vector2.zero is inserted in the right place.
        meshVertices.Insert (0, Vector2.zero);

        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh visibleMesh = filter.mesh;
        visibleMesh.Clear();
        visibleMesh.SetVertices(meshVertices);
		visibleMesh.triangles = Triangulate(meshVertices).ToArray();
		visibleMesh.uv = makeUVs(meshVertices).ToArray();

		// Update polygon collider
        Vector2[] colliderVertices = new Vector2[meshVertices.Count()];
        for (int i = 0; i < meshVertices.Count(); i++)
        {
            colliderVertices[i] = new Vector2(meshVertices[i].x, meshVertices[i].y);
		}
        GetComponent<PolygonCollider2D>().SetPath(0, colliderVertices);	
	}

    void insertExtraRaycasts(List<Vector3> filteredPoints, Vector3 currentPosition, float midpointAngle)
    {
        float startingAngle = midpointAngle - Mathf.PI / 4;
        float endingAngle = midpointAngle + Mathf.PI / 4;
        float intervalAngle = (endingAngle - startingAngle) / (NumExtraRaycasts - 1);
        for (int i = 0; i < NumExtraRaycasts; i++)
        {
            float currentAngle = startingAngle + intervalAngle * i;
            filteredPoints.Add(currentPosition + new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)));
        }

        filteredPoints.Add(currentPosition + new Vector3(Mathf.Cos(midpointAngle - Mathf.PI / 4), Mathf.Sin(midpointAngle - Mathf.PI / 4)));
        filteredPoints.Add(currentPosition + new Vector3(Mathf.Cos(midpointAngle + Mathf.PI / 4), Mathf.Sin(midpointAngle + Mathf.PI / 4)));
    }

    List<Vector3> doRaycasts(List<Vector3> filteredPoints)
    {
        List<Vector3> extremeVisiblePoints = new List<Vector3>();
        int layer = 1 << LayerMask.NameToLayer("VisibilityBlocking");
        foreach (Vector3 point in filteredPoints)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, point - transform.position, MaxVisibilityDistance, layer);
            if (hit.collider != null)
            {
                extremeVisiblePoints.Add(hit.point - (Vector2)transform.position);
            }
            else
            {
                extremeVisiblePoints.Add(computeExtremePoint(point));
            }
        }

        return extremeVisiblePoints;
    }

    Vector3 computeExtremePoint(Vector3 point)
    {
        Vector3 extremePoint = (point - transform.position);
        extremePoint.z = 0;
        extremePoint.Normalize();
        extremePoint *= MaxVisibilityDistance;

        return extremePoint;
    }

    List<Vector3> enumerateColliderPoints()
    {
        // Collect collider corners
        List<Vector3> points = new List<Vector3>();
        foreach (BoxCollider2D blockingCollider in blockers)
        {
            //TODO figure out which points to add more better
            //should be able to get away with 4 or 5 points, every time, instead of 8
            Vector3 size = blockingCollider.size;
            Vector3 position = blockingCollider.offset + (Vector2)blockingCollider.transform.position;

            Vector3 relativePosition = position - transform.position;
            relativePosition.z = 0;
            if (relativePosition.magnitude - size.magnitude >= MaxVisibilityDistance) continue;

            addPointsForBox(position, size, points);
        }

        return points;
    }

    void addPointsForBox(Vector3 position, Vector3 size, List<Vector3> points)
    {
        Vector3 rotatedSize = Vector2.Reflect(size, Vector2.right);

        Vector3 smallSize = size / 2.01f;
        size /= 1.99f;

        Vector3 smallRotatedSize = rotatedSize / 2.01f;
        rotatedSize /= 1.99f;

        points.Add(position + smallSize);
        points.Add(position + size);
        points.Add(position - smallSize);
        points.Add(position - size);
        points.Add(position + smallRotatedSize);
        points.Add(position + rotatedSize);
        points.Add(position - smallRotatedSize);
        points.Add(position - rotatedSize);
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
    
    List<Vector3> filterByDirection(Vector3 directionVector, Vector3 position, List<Vector3> points)
    {
        List<Vector3> ret = new List<Vector3>();
        foreach (Vector3 point in points)
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
