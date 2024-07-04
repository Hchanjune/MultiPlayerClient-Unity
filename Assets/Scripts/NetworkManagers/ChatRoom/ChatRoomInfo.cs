using Colyseus.Schema;

namespace NetworkManagers.ChatRoom
{
    public class ChatRoomInfo: Schema
    {
        [Type(0, "string")] 
        public string roomName = "";

        [Type(1, "string")] 
        public string roomOwner = "";
        
        [Type(2, "int32")] 
        public int maxClients = 10;
        
        [Type(3, "bool")] 
        public bool isPrivate = false;
        
        [Type(4, "string")] 
        public string players = "";
        
        [Type(5, "bool")] 
        public bool isPlaying = false;
        

    }
}