using UnityEngine;

public class TerrainOptimizer : MonoBehaviour
{
    public Transform player;
    public float disableDistance = 150f; 
    
    private Terrain terrainComponent;
    private TerrainCollider terrainCollider;

    void Start() 
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        
        terrainComponent = GetComponent<Terrain>();
        terrainCollider = GetComponent<TerrainCollider>();
    }

    void Update()
    {
        // Vi kollar bara var 30:e bildruta (ca 2 ggr i sekunden) för att spara kraft
        if (Time.frameCount % 30 != 0 || player == null) return;

        float distSq = (transform.position - player.position).sqrMagnitude;
        bool shouldBeVisible = distSq < (disableDistance * disableDistance);

        // Istället för SetActive stänger vi bara av komponenterna
        if (terrainComponent != null && terrainComponent.enabled != shouldBeVisible)
        {
            terrainComponent.enabled = shouldBeVisible;
            if (terrainCollider != null) terrainCollider.enabled = shouldBeVisible;
            
            Debug.Log(shouldBeVisible ? "Terräng tänd" : "Terräng släckt (Optimerad)");
        }
    }
}