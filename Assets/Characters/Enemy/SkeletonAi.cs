using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.RayTracingAccelerationStructure;

public class SkeletonAi : EnemyAi
{
    [Header("Skeleton")]
    public GameObject[] parts;

    public override void onDeath()
    {
        base.onDeath(); // Default called.
        animator.enabled = false;

        // To make a skeleton explode effect. A mesh renderer is added, so that rigidbody will affect it.
        foreach(GameObject obj in parts)
        {
            Mesh mesh = obj.GetComponent<SkinnedMeshRenderer>().sharedMesh;
            Material[] materials = obj.GetComponent<SkinnedMeshRenderer>().sharedMaterials;

            Destroy(obj.GetComponent<SkinnedMeshRenderer>());
            
            MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
            meshRenderer.materials = materials;

            MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            meshCollider.sharedMesh = mesh;
            
            Rigidbody rb = obj.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        
        Destroy(GetComponent<CapsuleCollider>());
        Destroy(gameObject, 10f); // Destroy the game object after a short delay.
        Enemies.Remove(this); // Remove this enemy from the list when destroyed
        Destroy(this);
    }
}

