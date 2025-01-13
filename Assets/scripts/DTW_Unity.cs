using UnityEngine;

public class DTWProcessor : MonoBehaviour
{
    public static float CalculateDTW(float[] seq1, float[] seq2)
    {
        int n = seq1.Length;
        int m = seq2.Length;
        float[,] dtw = new float[n + 1, m + 1];

        for (int i = 1; i <= n; i++) dtw[i, 0] = Mathf.Infinity;
        for (int j = 1; j <= m; j++) dtw[0, j] = Mathf.Infinity;

        dtw[0, 0] = 0;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                float cost = Mathf.Abs(seq1[i - 1] - seq2[j - 1]);
                dtw[i, j] = cost + Mathf.Min(dtw[i - 1, j], Mathf.Min(dtw[i, j - 1], dtw[i - 1, j - 1]));
            }
        }

        return dtw[n, m];
    }
}
