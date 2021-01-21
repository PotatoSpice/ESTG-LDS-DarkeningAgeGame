using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;

public class Toast : MonoBehaviour
{
    public ConcurrentQueue<string> toasts;
    public Text toastText;
    private bool canShow;

    private void Awake()
    {
        toasts = new ConcurrentQueue<string>();
        canShow = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!toasts.IsEmpty && canShow)
        {
            string text;
            toasts.TryDequeue(out text);
            this.ShowToast(text, 2);
        }
    }

    private void ShowToast(string text, int duration)
    {
        StartCoroutine(showToastCOR(text, duration));
    }

    private IEnumerator showToastCOR(string text, int duration)
    {
        canShow = false;
        Color orginalColor = this.gameObject.GetComponent<Image>().color;
        toastText.text = text;
        this.gameObject.SetActive(true);

        //Fade in
        yield return fadeInAndOut(orginalColor, true, 0.5f);

        //Wait for the duration
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return fadeInAndOut(orginalColor, false, 0.5f);

        toastText.text = "";
        this.gameObject.GetComponent<Image>().color = orginalColor;
        canShow = true;
    }

    IEnumerator fadeInAndOut(Color orginalColor, bool fadeIn, float duration)
    {
        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 0.7f;
        }
        else
        {
            a = 0.7f;
            b = 0f;
        }

        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            this.gameObject.GetComponent<Image>().color = new Color(orginalColor.r, orginalColor.g, orginalColor.b, alpha);
            yield return null;
        }
    }
}
