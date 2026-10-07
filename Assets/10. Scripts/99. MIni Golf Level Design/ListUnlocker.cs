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
    public string[] requiredIds;
    public string finalId = "Clear";
    public Behaviour[] enableOnFinal;

    private void Start()
    {
        CheakFinalUnlock();

        if (GameProgress.IsUnlocked(finalId))
        {
            foreach (Behaviour b in enableOnFinal)
            {
                b.enabled = true;
            }
        }

        foreach (UnlockTarget t in targets)
        {
            if (GameProgress.IsUnlocked(t.id))
            {
                t.target.SetActive(true);
            }
        }
    }

    private void CheakFinalUnlock()
    {
        if (requiredIds.Length == 0) return;

        foreach (string id in requiredIds)
        {
            if (!GameProgress.IsUnlocked(id)) return;
        }

        GameProgress.Unlock(finalId);
    }
}