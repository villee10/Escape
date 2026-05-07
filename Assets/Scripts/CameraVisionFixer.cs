using UnityEngine;
using System.Collections.Generic;

public class CameraVisionFixer : MonoBehaviour
{
    [Header("Referenser")]
    public Transform player;
    public LayerMask wallLayer;
    
    [Header("Inställningar")]
    public float sphereRadius = 0.5f;
    [Tooltip("Max antal väggar som kan döljas samtidigt. Högre siffra = tyngre för CPU.")]
    public int maxHits = 10; 
    
    private List<Renderer> currentlyHiddenWalls = new List<Renderer>();
    private RaycastHit[] hitBuffer; // Här sparar vi träffarna utan att skapa nytt minne

    void Start()
    {
        // Vi skapar "lådan" för träffar en gång i början
        hitBuffer = new RaycastHit[maxHits];
    }

    void Update()
    {
        // Kör var 5:e frame för att spara kraft
        if (Time.frameCount % 5 != 0 || player == null) return;

        // 1. Återställ gamla väggar
        for (int i = 0; i < currentlyHiddenWalls.Count; i++)
        {
            if (currentlyHiddenWalls[i] != null)
                currentlyHiddenWalls[i].enabled = true;
        }
        currentlyHiddenWalls.Clear();

        // 2. Skjut strålen med NonAlloc
        Vector3 origin = transform.position;
        Vector3 targetPos = player.position + Vector3.up * 1.2f;
        Vector3 direction = targetPos - origin;
        float distance = direction.magnitude;

        // Physics.SphereCastNonAlloc returnerar ANTALET träffar istället för en hel lista
        int numHits = Physics.SphereCastNonAlloc(origin, sphereRadius, direction.normalized, hitBuffer, distance, wallLayer);

        for (int i = 0; i < numHits; i++)
        {
            RaycastHit hit = hitBuffer[i];
            
            // Försök hitta renderaren (leta först på objektet, sen i barnen)
            Renderer wallRender = hit.collider.GetComponent<Renderer>();
            if (wallRender == null) wallRender = hit.collider.GetComponentInChildren<Renderer>();

            if (wallRender != null && hit.collider.transform != player)
            {
                if (!currentlyHiddenWalls.Contains(wallRender))
                {
                    currentlyHiddenWalls.Add(wallRender);
                    wallRender.enabled = false;
                }
            }
        }
    }
}