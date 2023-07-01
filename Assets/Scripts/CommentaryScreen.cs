using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class CommentaryScreen: MonoBehaviour
{
    int maxTextEntries = 3;
    List<String> textEntries = new List<String>();
    String scoreTextEntry = "";
    
    public void AddNewFacts(HashSet<String> newFacts)
    {
        HashSet<String> ScoreScreenFact = newFacts.Where(fact => Regex.IsMatch(fact, "Red .* - .* Blue")).ToHashSet();
        HashSet<String> CommentaryScreenFact = newFacts.Except(ScoreScreenFact).ToHashSet();

        foreach (String fact in CommentaryScreenFact) {
            AddTextEntry(fact);
        }

        foreach (String fact in ScoreScreenFact) {
            scoreTextEntry = fact;
        }

        RefreshText();
    }

    public void AddTextEntry(String text)
    {
        // keep at most maxTextEntries entries
        if (textEntries.Count >= maxTextEntries)
        {
            textEntries.RemoveAt(0);
        }
        textEntries.Add(text);
    }

    private void RefreshText()
    {
        TextMesh textMesh = GameObject.Find("Text").GetComponent<TextMesh>();
        textMesh.text = String.Join("\n", textEntries);
        TextMesh scoreTextMesh = GameObject.Find("ScoreText").GetComponent<TextMesh>();
        scoreTextMesh.text = scoreTextEntry;
    }

}