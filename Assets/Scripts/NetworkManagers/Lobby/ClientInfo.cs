using System;
using Colyseus.Schema;

namespace NetworkManagers.Lobby
{
    [Serializable]
    public class ClientInfo: Schema
    {
        [Colyseus.Schema.Type(0, "string")] 
        public string sessionId = "";
        [Colyseus.Schema.Type(1, "string")] 
        public string id = "";
        [Colyseus.Schema.Type(2, "string")] 
        public string name = "";
        
    }
}