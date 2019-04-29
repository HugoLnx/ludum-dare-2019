using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HunterBang : MonoBehaviour
{
    private const float DISTANCE = 0.3f;
    private const float DURATION = 0.2f;
    private RectTransform rect;
    private TMP_Text text;

    void Awake()
    {
        this.rect = GetComponentInChildren<RectTransform>();
        this.text = GetComponentInChildren<TMP_Text>();
    }

    void Start()
    {
        Hide();
    }

    public PremadeAnimation Alert()
    {
        Show();
        return new AnimationChain(this, new List<PremadeAnimation>
        {
            new TranslateAnimation(this, Vector3.up*DISTANCE, duration: DURATION/2f),
            new TranslateAnimation(this, Vector3.down*DISTANCE, duration: DURATION/2f),
            new WaitingAnimation(this, duration: 2f),
        }, after: () => Hide()).Start();
    }

    private void Show()
    {
        var c = this.text.color;
        this.text.color = new Color(c.r, c.g, c.b, 1f);
    }

    private void Hide()
    {
        var c = this.text.color;
        this.text.color = new Color(c.r, c.g, c.b, 0f);
    }
}
