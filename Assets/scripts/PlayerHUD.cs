using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class PlayerHUD : MonoBehaviour
{
    public TMP_Text score;
    public const int SCORE_MAX_DIGITS = 6;

    public void SetScore(int score)
    {
        this.score.text = FormatScore(score);
    }

    public static string FormatScore(int score)
    {
        var score_str = score.ToString();
        var builder = new StringBuilder();
        for (int i = score_str.Length; i < SCORE_MAX_DIGITS; i++)
            builder.Append("0");

        builder.Append(score_str);
        return builder.ToString();

    }
}
