using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace _10._Scripts._99._MIni_Golf_Level_Design
{
    public class TestMoveScene : MonoBehaviour
    {
        public string sceneName = "01. List";

        public void Update()
        {
            if (SimpleInput.GetKeyDown(Key.Q))
            {
                SceneManager.LoadScene(sceneName);
            }
        }
        
        
        
        
    }
}