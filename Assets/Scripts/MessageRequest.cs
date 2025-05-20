using System.Collections.Generic;

public class MessageRequest
{
    public string Name;
    public string Text;

    private Queue<Phoneme> Phonemes;

    public MessageRequest(string name, string text)
    {
        Name = name;
        Text = text;
    }

    public Phoneme NextPhoneme()
    {
        if (Phonemes.Count == 0) return null;
        else return Phonemes.Dequeue();
    }
}
