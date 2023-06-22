using System.Collections;
using UnityEngine.Networking;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

public static class KnowledgeBasePredicates
{
    public static Dictionary<string, string> predicateMap = new Dictionary<string, string>()
    {
        {"Team", "team(<team_name>)"},
        {"Player", "player(<player_name>)"},
        {"PlaysInTeam", "playsInTeam(<player_name>, <team_name>)"},
        {"TouchPlayerAtAction", "touchPlayerAtAction(<player_name>, <action_name>)"}
    };
}