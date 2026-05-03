using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDeformer : MonoBehaviour
{
    [Range(0, 10)]
    public float deformRadius = 0.2f;

    [Range(0, 10)]
    public float maxDeform = 0.1f;

    [Range(0, 1)]
    public float damageFalloff = 1;

    [Range(0, 10)]
    public float damageMultiplier = 1;

    [Range(0, 100000)]
    public float minDamage = 1;

    public AudioClip[] collisionSounds;

    public MeshFilter filter;
    public Rigidbody physics;
    public MeshCollider coll;
    private Mesh m_originalMesh;

    private Vector3[] m_startingVerticies;

    private Vector3[] m_meshVerticies;
    private Vector3[] m_collisionVertices;

    void Start()
    {
        if (filter == null) filter = GetComponentInChildren<MeshFilter>();
        if (coll == null) coll = GetComponentInChildren<MeshCollider>();
        if (physics == null) physics = GetComponentInParent<Rigidbody>();
        if (filter == null) { Debug.LogWarning("[DamageDeformer] no MeshFilter found"); enabled = false; return; }

        m_startingVerticies = filter.mesh.vertices;
        m_meshVerticies = filter.mesh.vertices;

        m_originalMesh = Instantiate(filter.mesh);

        coll.sharedMesh = filter.mesh;

    }

    void OnCollisionEnter(Collision collision)
    {

        float collisionPower = collision.impulse.magnitude;

        if (collisionPower > minDamage)
        {

            foreach (ContactPoint point in collision.contacts)
            {

                for (int i = 0; i < m_meshVerticies.Length; i++)
                {
                    Vector3 vertexPosition = m_meshVerticies[i];

                    Vector3 pointPosition = filter.transform.InverseTransformPoint(point.point);

                    float distanceFromCollision = Vector3.Distance(vertexPosition, pointPosition);

                    float distanceFromOriginal = Vector3.Distance(m_startingVerticies[i], vertexPosition);

                    if (distanceFromCollision < deformRadius && distanceFromOriginal < maxDeform)
                    {

                        float falloff = 1 - (distanceFromCollision / deformRadius) * damageFalloff;

                        float xDeform = pointPosition.x * falloff;
                        float yDeform = pointPosition.y * falloff;
                        float zDeform = pointPosition.z * falloff;

                        xDeform = Mathf.Clamp(xDeform, 0, maxDeform);
                        yDeform = Mathf.Clamp(yDeform, 0, maxDeform);
                        zDeform = Mathf.Clamp(zDeform, 0, maxDeform);

                        Vector3 deform = new Vector3(xDeform, yDeform, zDeform);

                        m_meshVerticies[i] -= deform * damageMultiplier;
                    }
                }
            }

            UpdateMeshVerticies();
        }
    }

    void UpdateMeshVerticies()
    {

        filter.mesh.vertices = m_meshVerticies;

        coll.sharedMesh = filter.mesh;
    }

    public void ResetDeformation()
    {

        m_meshVerticies = m_originalMesh.vertices;
        filter.mesh.vertices = m_meshVerticies;
        filter.mesh.normals = m_originalMesh.normals;
        filter.mesh.triangles = m_originalMesh.triangles;
        filter.mesh.uv = m_originalMesh.uv;

        coll.sharedMesh = null;
        coll.sharedMesh = filter.mesh;
    }

}
