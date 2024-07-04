using System;
using Colyseus.Schema;
using NetworkManagers.ChatRoom;

namespace NetworkManagers.Lobby
{
    [Serializable]
    public class LobbyState: Schema
    {
        [Colyseus.Schema.Type(0, "string")]
        public string initializedTimestamp = "";

        [Colyseus.Schema.Type(1, "map", typeof(MapSchema<ClientInfo>))]
        public MapSchema<ClientInfo> clients = new MapSchema<ClientInfo>();
        
        [Colyseus.Schema.Type(2, "map", typeof(MapSchema<ChatRoomInfo>))]
        public MapSchema<ChatRoomInfo> chatRooms = new MapSchema<ChatRoomInfo>();

        
    }
}