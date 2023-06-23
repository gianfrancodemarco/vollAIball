using System.Collections;
using UnityEngine.Networking;
using System.Text;
using UnityEngine;

public class KnowledgeBaseClient
{
    public static KnowledgeBaseClient Instance { get; private set; } = new KnowledgeBaseClient();

	private KnowledgeBaseClient(){ }

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
                Debug.Log("Result:" + request.downloadHandler.text);
            }
        }
        request.Dispose();
    }
}