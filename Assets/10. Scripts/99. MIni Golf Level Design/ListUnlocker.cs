using UnityEngine;

public class ListUnlocker : MonoBehaviour
{
    [System.Serializable]
    public class UnlockTarget
    {
        public string id;
        public GameObject target;
    }

    public UnlockTarget[] targets;

    private void Start()
    {
        foreach (UnlockTarget t in targets)
        {
            if (GameProgress.IsUnlocked(t.id))
            {
                t.target.SetActive(true);
            }
        }
    }
}