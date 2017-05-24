using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region StreamToServer

#region JSON Blueprints
internal class AddUser
{
    public AddUser(string p_user)
    {
        user = p_user;
    }
    public string user;
}
#endregion

public static class StreamToServer 
{
    public static void ConnectNewUser(string p_user)
    {
        AddUser __blueprint = new AddUser(p_user);
        string __dataToSend = VoxJSON.ToJSON(true, __blueprint);
        Server.instance.SendDataStreamToServer(__dataToSend);
    }
}

#endregion

#region StreamToClient

#region JSON Blueprints
internal class LobbyMembers
{
    public LobbyMembers(List<ServerData.UserData> p_listUsers)
    {
        listUsers = p_listUsers;
    }
    public List<ServerData.UserData> listUsers;
}
#endregion

public static class StreamParser
{
    public static string GetInformUserStream(Dictionary<string, ServerData.UserData> p_dictUsers)
    {
        string __stream = "{ \"LobbyMembers\": [";
        int __counter = 0;
        foreach (var content in p_dictUsers)
        {
            __stream += "{";

            __stream += "\"id\": \"" + content.Value.id + "\",";
            __stream += "\"user\": \"" + content.Value.userName + "\"";

            if (__counter < p_dictUsers.Count - 1)
            {
                __stream += "}, ";
            }
            else
            {
                __stream += "}";
            }

            __counter++;
        }
        __stream += "]}";
        return __stream;
    }
}
#endregion
