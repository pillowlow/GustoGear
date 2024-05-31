using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private Image UIFill;
    [SerializeField] private TMP_Text UIText;
    public int duration;
    private int remainingTime;
    private bool Pause = false;
    private void Start()
    {
        Pause = false;
        Being(duration);
    }
    private void Being(int second)
    {
        remainingTime = second;
        StartCoroutine(UpdateTimer());
    }
    private IEnumerator UpdateTimer()
    {
        while (remainingTime >= 0)
        {
            if (!Pause)
            {
                UIText.text = $"{remainingTime / 60:00}:{remainingTime % 60:00}";
                UIFill.fillAmount = Mathf.InverseLerp(0, duration, remainingTime);
                remainingTime--;
                yield return new WaitForSeconds(1);
            }
            yield return null;
        }
        OnEnd();
    }
    private void OnEnd()
    {
        print("Time's up!");
    }
}
