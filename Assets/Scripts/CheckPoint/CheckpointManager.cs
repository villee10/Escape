using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CheckpointManager : MonoBehaviour
{
    public static Vector3 lastCheckPointPos;
    public static bool hasReachedCheckpoint = false;

    // 1. Skapa en statisk referens till hotbaren
    public static GameObject hotbarStaticRef;
    // 2. Skapa en ruta i Inspectorn där vi kan dra in hotbaren
    public GameObject hotbarObject;

    void Awake()
    {
        // Koppla ihop rutan i Inspectorn med vår statiska referens
        if (hotbarObject != null)
        {
            hotbarStaticRef = hotbarObject;
        }
    }

    void Start()
    {
        if (hasReachedCheckpoint)
        {
            // Om vi spawnar om vid en checkpoint, se till att hotbaren är på
            if (hotbarStaticRef != null) hotbarStaticRef.SetActive(true);
            StartCoroutine(WaitAndMove());
        }
    }

    IEnumerator WaitAndMove()
    {
        yield return null; 
        transform.position = lastCheckPointPos;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        CinemachineCamera vcam = FindFirstObjectByType<CinemachineCamera>();
        if (vcam != null)
        {
            vcam.OnTargetObjectWarped(transform, lastCheckPointPos - transform.position);
        }
    }

    public static void SetCheckpoint(Vector3 pos)
    {
        lastCheckPointPos = pos;
        hasReachedCheckpoint = true;

        // 3. Aktivera hotbaren direkt när checkpointen sätts!
        if (hotbarStaticRef != null)
        {
            hotbarStaticRef.SetActive(true);
        }
    }
}