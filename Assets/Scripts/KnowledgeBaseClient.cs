using System.Collections;
using UnityEngine.Networking;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Linq;

public class NarrativeDTO
{
    public String status;
    public List<String> response;
}

public class KnowledgeBaseClient
{
    public static KnowledgeBaseClient Instance { get; private set; } = new KnowledgeBaseClient();

	private KnowledgeBaseClient(){ }
    
    private HashSet<string> facts = new HashSet<string>();

	public int Coins { get; set; }

    private string backendUri = "localhost:5000";

    public IEnumerator Reset()
    {
        string url = backendUri + "/reset";
        return Post(url);
    }

    public IEnumerator SaveFact(string fact)
    {
        string url = backendUri + "/assert";
        string body = "{\"fact\":\"" + fact + "\"}";
        return Post(url, body);
    }

    public IEnumerator GetMatchNarrative()
    {
        string url = backendUri + "/narrative";
        return Get(url, ResponseCallback);
    }

    // Callback to act on our response data
	private void ResponseCallback(string data)
	{
		//Debug.Log(data);
        NarrativeDTO narrative = JsonUtility.FromJson<NarrativeDTO>(data);
        HashSet<String> response = ScreenProcessText(narrative.response.ToHashSet());
        HashSet<String> newFacts = response.Except(facts).ToHashSet();

        HashSet<String> CommentaryScreenFact = newFacts.Where(fact => !Regex.IsMatch(fact, "Red .* - .* Blue")).ToHashSet();
        HashSet<String> ScoreScreenFact = newFacts.Except(CommentaryScreenFact).ToHashSet();

        foreach (String fact in newFacts) {
            Debug.Log("Narrator: " + fact); 
            
        }

        foreach (String fact in CommentaryScreenFact) {
            TextMesh textObject = GameObject.Find("Text").GetComponent<TextMesh>();
            textObject.text = fact;
        }

        foreach (String fact in ScoreScreenFact) {
            TextMesh textObject = GameObject.Find("ScoreText").GetComponent<TextMesh>();
            textObject.text = fact;
        }

        facts = response;
	}

    private HashSet<String> ScreenProcessText(HashSet<String> Predicates) {
        HashSet<String> ProcessedPredicates = new HashSet<String>();
        foreach (var predicate in Predicates) {
            if (predicate.Contains("point")) {
                ProcessedPredicates.Add(predicate.Replace("(point", "\n (point"));
            } else {
                ProcessedPredicates.Add(predicate);
            }
            
        }
        return ProcessedPredicates;
    }

    public IEnumerator QueryFact(string query) { 
        string url = backendUri + "/query";
        string body = "{\"query\":\"" + query + "\"}";
        return Post(url, body);
    }

    private IEnumerator Post(string url)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.SetRequestHeader("Content-Type", "application/json"); //important

        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("Error: " + request.error);
            }
        }
        request.Dispose();
    }

    private IEnumerator Post(string url, string body)
    {
        // post request to the server:
        // curl -X POST -H "Content-Type: application/json" -d '{"fact": "hitRedAgent"}' http://localhost:5000/fact
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = Encoding.UTF8.GetBytes(body);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json"); //important

        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                //Debug.Log("Result:" + request.downloadHandler.text);
            }
        }
        request.Dispose();
    }

    private IEnumerator Get(string url, System.Action<string> responseCallback)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            responseCallback(request.downloadHandler.text);
        }

        request.Dispose();

        yield return request;
    }
}