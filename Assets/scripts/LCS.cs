using UnityEngine;
using System;

public static class LCS
{
    public static int FindLCSLength(string str1, string str2)
    {
        int m = str1.Length;
        int n = str2.Length;

        // DP 테이블 초기화
        int[,] dp = new int[m + 1, n + 1];

        // DP를 이용한 LCS 길이 계산
        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                if (str1[i - 1] == str2[j - 1])
                {
                    dp[i, j] = dp[i - 1, j - 1] + 1;
                }
                else
                {
                    dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }
        }

        // LCS 길이 반환
        return dp[m, n];
    }

    public static float CalculateSimilarity(string source, string target)
    {
        string source_final = HangulUtils.Decompose(source);
        string target_final = HangulUtils.Decompose(target);
        Debug.Log($"{source_final} AND {target_final}");
        int distance = FindLCSLength(source_final, target_final);

        Debug.Log($"{target_final.Length} ALSO {distance}");

        return (float)distance / target_final.Length; // 유사도 (0.0 ~ 1.0)
    }
}
