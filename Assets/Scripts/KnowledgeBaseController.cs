using UnityEngine;
using Unity.MLAgents;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class AssertDTO
{
    public string fact { get; set; }
}

public class KnowledgeBaseController : MonoBehaviour
{
    void Awake() { 
        StartCoroutine(SaveFact());
        StartCoroutine(QueryFact());   
    }

    public void ResolveEvent(Event triggerEvent)
    {
        switch (triggerEvent)
        {
            case Event.HitRedAgent:
                break;
            case Event.HitBlueAgent:
                break;
            case Event.HitOutOfBounds:
                break;
            case Event.HitBlueGoal:
                break;
            case Event.HitRedGoal:
                break;
            case Event.HitIntoBlueArea:
                break;
            case Event.HitIntoRedArea:
                break;
            case Event.HitWall:
                break;
            case Event.EpisodeEnd:
                break;
        }
    }

    public IEnumerator SaveFact()
    {
        // post request to the server:
        // curl -X POST -H "Content-Type: application/json" -d '{"fact": "hitRedAgent"}' http://localhost:5000/fact
        UnityWebRequest request = new UnityWebRequest("localhost:5000/assert", "POST");
        byte[] jsonToSend = Encoding.UTF8.GetBytes("{\"fact\": \"hitRedAgent\"}");
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json"); //important

        {
            yield return request.SendWebRequest();
        
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }

    }

    public IEnumerator QueryFact()
    {
        // post request to the server:
        // curl -X POST -H "Content-Type: application/json" -d '{"fact": "hitRedAgent"}' http://localhost:5000/fact
        UnityWebRequest request = new UnityWebRequest("localhost:5000/query", "POST");
        byte[] jsonToSend = Encoding.UTF8.GetBytes("{\"query\": \"hit(X)\"}");
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json"); //important

        {
            yield return request.SendWebRequest();
        
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log(request.result.ToString());
            }
        }
    }

}