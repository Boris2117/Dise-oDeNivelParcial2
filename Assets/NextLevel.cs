using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private string tagJugador = "Player";
    [SerializeField] private string nombreEscena;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;
        SceneManager.LoadScene(nombreEscena);
        
    }
}
