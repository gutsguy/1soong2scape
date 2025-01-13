using UnityEngine;
using System;

public static class Levenshtein
{
    public static int CalculateDistance(string source, string target)
    {
        int[,] dp = new int[source.Length + 1, target.Length + 1];

        for (int i = 0; i <= source.Length; i++) dp[i, 0] = i;
        for (int j = 0; j <= target.Length; j++) dp[0, j] = j;

        for (int i = 1; i <= source.Length; i++)
        {
            for (int j = 1; j <= target.Length; j++)
            {
                int cost = source[i - 1] == target[j - 1] ? 0 : 1;

                dp[i, j] = Mathf.Min(
                    dp[i - 1, j] + 1,
                    dp[i, j - 1] + 1,
                    dp[i - 1, j - 1] + cost
                );
            }
        }

        return dp[source.Length, target.Length];
    }

    public static float CalculateSimilarity(string source, string target)
    {
        string source_final = HangulUtils.Decompose(source);
        string target_final = HangulUtils.Decompose(target);
        int distance = CalculateDistance(source_final, target_final);
        int maxLength = Mathf.Max(source_final.Length, target_final.Length);

        Debug.Log($"{source_final} AND {target_final}");
        Debug.Log($"{target_final.Length} ALSO {distance}");
        
        return 1.0f - (float)distance / maxLength; // 유사도 (0.0 ~ 1.0)
    }
}
