using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TextBoxEnabler : MonoBehaviour
{
    public SoundManager SoundManager;
    public float LetterDuration = 0.1f;

    public float DisplayDuration = 6.0f;
    public float PauseDuration = 3.0f;

    public TMP_Text Text;
    public TMP_Text Name;

    public NameColor NameColor;

    public GameObject NextArrow;

    public List<EnableAnimation> enableAnimations;
    private bool _prevEnabled;
    public bool Enabled;

    private Queue<MessageRequest> MessageRequests;

    private void Start()
    {
        MessageRequests = new Queue<MessageRequest>();
        NextArrow.SetActive(false);
    }

    private void Update()
    {
        if (!Enabled && MessageRequests.Count > 0)
        {
            StartCoroutine(PlayTextCoroutine(MessageRequests.Dequeue()));
        }
    }

    public void PlayText(string name, string text)
    {
        MessageRequests.Enqueue(new MessageRequest(name, text));
    }

    IEnumerator PlayTextCoroutine(MessageRequest messageRequest)
    {
        Enabled = true;

        NameColor.SetName(messageRequest.Name);
        Name.text = messageRequest.Name;
        Text.text = "";
        List<string> strings = SplitToLines(messageRequest.Text, 100).ToList();

        foreach (EnableAnimation enableAnimation in enableAnimations)
        {
            enableAnimation.SetEnabled(true);
        }

        bool ready = true;
        do
        {
            ready = true;
            foreach (EnableAnimation enableAnimation in enableAnimations)
            {
                ready = ready && enableAnimation.IsReady();
            }
            yield return new WaitForSeconds(0.1f);
        } while (!ready);

        int i = 0;
        foreach (string text in strings)
        {
            Text.text = "";
            i++;

            foreach (char letter in text.ToCharArray())
            {
                Text.text += letter;
                yield return SoundManager.PlayLetter(char.ToLower(letter));
            }

            if (i < strings.Count)
            {
                NextArrow.SetActive(true);
            }
            else
            {
                NextArrow.SetActive(false);
            }

            yield return new WaitForSeconds(DisplayDuration);
            NextArrow.SetActive(false);
        }

        foreach (EnableAnimation enableAnimation in enableAnimations)
        {
            enableAnimation.SetEnabled(false);
        }

        yield return new WaitForSeconds(PauseDuration);
        Enabled = false;
    }

    private IEnumerable<string> SplitToLines(string stringToSplit, int maximumLineLength)
    {
        var words = stringToSplit.Split(' ').Concat(new[] { "" });
        return
            words
                .Skip(1)
                .Aggregate(
                    words.Take(1).ToList(),
                    (a, w) =>
                    {
                        var last = a.Last();
                        while (last.Length > maximumLineLength)
                        {
                            a[a.Count() - 1] = last.Substring(0, maximumLineLength);
                            last = last.Substring(maximumLineLength);
                            a.Add(last);
                        }
                        var test = last + " " + w;
                        if (test.Length > maximumLineLength)
                        {
                            a.Add(w);
                        }
                        else
                        {
                            a[a.Count() - 1] = test;
                        }
                        return a;
                    });
    }



}
