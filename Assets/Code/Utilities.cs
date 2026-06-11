using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static string GenerateUID()
    {
        return Guid.NewGuid().ToString();
    }

    private static System.Random rng = new System.Random();

    public static List<Player> ShufflePlayerList(List<Player> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Player value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }
}
