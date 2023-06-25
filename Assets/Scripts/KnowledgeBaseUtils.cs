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
        {"TouchPlayerAtAction", "touchPlayerAtAction(<player_name>, <episode_name>, <action_name>, <number_of_team_pass>)"},
        {"HitOutOfBounds", "hitOutOfBounds(<player_name>, <episode_name>, <action_name>)"},
        {"HitGoal", "hitGoal(<player_name>, <episode_name>, <action_name>)"},
        {"HitIntoBlueArea", "hitIntoBlueArea(<player_name>, <episode_name>, <action_name>)"},
        {"HitIntoRedArea", "hitIntoRedArea(<player_name>, <episode_name>, <action_name>)"},
        {"HitWall", "hitWall(<player_name>, <episode_name>, <action_name>)"}
    };

}