using Study.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TestUnlock : MonoBehaviour
{
    public string[] ids;

    private void Update()
    {
        if (SimpleInput.GetKeyDown(Key.Digit1))
        {
            foreach (string id in ids)
            {
                GameProgress.Unlock(id);
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}