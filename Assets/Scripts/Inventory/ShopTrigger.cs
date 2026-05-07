using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    public GameObject shopPanel; // Dra in din ShopPanel här
    private bool canOpen = false;

    void Update()
    {
        // Kollar om spelaren är nära och trycker på E
        if (canOpen && Input.GetKeyDown(KeyCode.E))
        {
            bool isActive = shopPanel.activeSelf;
            shopPanel.SetActive(!isActive);
            Debug.Log("Shoppen " + (!isActive ? "öppnas!" : "stängs!"));
            
            // Tips: Om du vill att muspekaren ska synas när shoppen öppnas:
            // Cursor.visible = !isActive;
            // Cursor.lockState = !isActive ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    // Notera: Här har vi tagit bort "2D" från metodnamnen
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canOpen = true;
            Debug.Log("Spelare i närheten av häxan!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canOpen = false;
            shopPanel.SetActive(false);
            Debug.Log("Spelaren gick iväg.");
        }
    }
}