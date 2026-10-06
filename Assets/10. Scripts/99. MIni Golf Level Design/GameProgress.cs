using System.Collections.Generic;

public static class GameProgress
{
    private static HashSet<string> unlocked = new HashSet<string>();

    public static void Unlock(string id)
    {
        unlocked.Add(id);
    }

    public static bool IsUnlocked(string id)
    {
        return unlocked.Contains(id);
    }
}