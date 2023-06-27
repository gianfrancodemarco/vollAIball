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
        {"PlaysInTeam", "playsinteam(<player_name>, <team_name>)"},
        {"TouchPlayerAtAction", "touchplayerataction(<player_name>, <point>, <action>, <touch>)"},
        {"HitOutOfBounds", "hitoutofbounds(<player_name>, <point>, <action>)"},
        {"HitBlueGoal", "hitbluegoal(<player_name>, <point>, <action>)"},
        {"HitRedGoal", "hitredgoal(<player_name>, <point>, <action>)"},
        {"HitIntoBlueArea", "hitintobluearea(<player_name>, <point>, <action>)"},
        {"HitIntoRedArea", "hitintoredarea(<player_name>, <point>, <action>)"},
        {"HitWall", "hitwall(<player_name>, <point>, <action>)"}
    };

}