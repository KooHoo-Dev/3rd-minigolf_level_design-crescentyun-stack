using UnityEngine;
using UnityEngine.SceneManagement;

public class StageZone : MonoBehaviour
{
    public string sceneName = "01. List";
    public string unlockId;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!string.IsNullOrEmpty(unlockId))
        {
            GameProgress.Unlock(unlockId);
        }

        SceneManager.LoadScene(sceneName);
    }
}