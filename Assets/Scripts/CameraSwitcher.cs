using UnityEngine;
using Unity.Cinemachine; // Observera: Om man kör en äldre version av Cinemachine kan det vara 'using Cinemachine;'

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineCamera Inside_Camera_start;
    public CinemachineCamera OutsideCamera_start;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // När vi går UT ur ladan (om triggern sitter precis vid dörröppningen)
            OutsideCamera_start.Priority = 20;
            Inside_Camera_start.Priority = 10;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // När vi går in i ladan igen
            Inside_Camera_start.Priority = 20;
            OutsideCamera_start.Priority = 10;
        }
    }
}