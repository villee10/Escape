using UnityEngine;
using System.Collections; // Krävs för IEnumerator
using Unity.Cinemachine;

public class CheckpointManager : MonoBehaviour
{
    public static Vector3 lastCheckPointPos;
    public static bool hasReachedCheckpoint = false;

    void Start()
    {
        if (hasReachedCheckpoint)
        {
            StartCoroutine(WaitAndMove());
        }
    }

    IEnumerator WaitAndMove()
    {
        // Vänta tills nästa frame så alla andra skript hunnit starta
        yield return null; 

        // 1. Flytta gubben
        transform.position = lastCheckPointPos;

        // 2. Nollställ fysik
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 3. Fixa kameran
        CinemachineCamera vcam = FindFirstObjectByType<CinemachineCamera>();
        if (vcam != null)
        {
            vcam.OnTargetObjectWarped(transform, lastCheckPointPos - transform.position);
        }

        Debug.Log("NU tvingade vi gubben till bron!");
    }

    public static void SetCheckpoint(Vector3 pos)
    {
        lastCheckPointPos = pos;
        hasReachedCheckpoint = true;
    }
}