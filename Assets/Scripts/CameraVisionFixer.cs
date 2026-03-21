using UnityEngine;
using System.Collections.Generic;

public class CameraVisionFixer : MonoBehaviour
{
    [Header("Referenser")]
    public Transform player;          // Dra in din gubbe här
    public LayerMask wallLayer;       // Välj lagret där dina väggar ligger
    
    [Header("Inställningar")]
    public float sphereRadius = 0.5f; // Hur bred "lasern" är. Justera om för många/få väggar försvinner.
    
    private List<Renderer> currentlyHiddenWalls = new List<Renderer>();

    void Update()
    {
        // 1. Återställ gamla väggar (Gör dem synliga igen)
        // Detta är den viktigaste delen! När gubben går, måste de gamla väggarna komma tillbaka.
        foreach (var wall in currentlyHiddenWalls)
        {
            if (wall != null)
            {
                wall.enabled = true; // Slår på bilden (visar väggen)
            }
        }
        currentlyHiddenWalls.Clear();

        // 2. Skjut strålen (en "SphereCast") från kameran till gubben
        Vector3 direction = player.position - transform.position;
        float distance = Vector3.Distance(transform.position, player.position);
        
        // Vi använder SphereCastAll för att hitta ALLA väggar som skymmer
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, sphereRadius, direction, distance, wallLayer);

        foreach (var hit in hits)
        {
            // Vi vill bara dölja saker som har en Renderer (en bild)
            Renderer wallRender = hit.collider.GetComponent<Renderer>();
            
            // Om den har en bild och INTE är gubben (bara för säkerhets skull)
            if (wallRender != null && hit.collider.transform != player)
            {
                currentlyHiddenWalls.Add(wallRender);
                wallRender.enabled = false; // Slår av bilden (väggen försvinner helt!)
            }
        }
    }
}