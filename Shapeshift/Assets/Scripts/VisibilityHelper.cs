﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class VisibilityHelper : MonoBehaviour
{
    public float maxVisibilityDistance;

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

        // sort the points by angle (maybe hopefully?)
        List<Vector3> meshVertices = new List<Vector3>();
        meshVertices.Add(Vector2.zero);
        meshVertices.AddRange(extremeVisiblePoints
            .OrderBy((p) => -Mathf.Atan2(p.y, p.x))
            .Select((v2) => new Vector3(v2.x, v2.y)));
        meshVertices.Add(meshVertices[1]);
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
            uvs.Add(new Vector2(vertex.magnitude / maxVisibilityDistance, 0));
            //Debug.Log(vertex.magnitude * 100);
        }
        visibleMesh.uv = uvs.ToArray();

        MeshFilter filter = GetComponent<MeshFilter>();
        filter.mesh.Clear();
        filter.mesh = visibleMesh;
    }
}
