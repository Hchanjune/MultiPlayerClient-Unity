using Colyseus;
using NetworkManagers.Lobby;
using UnityEngine.SceneManagement;
using Zenject;

namespace NetworkManagers
{
    public class NetworkManager
    {
        public ColyseusClient Client;
    
        public LobbyNetwork LobbyNetwork { get; private set; }

        [Inject]
        public void Construct(LobbyNetwork lobbyNetwork)
        {
            LobbyNetwork = lobbyNetwork;
        }

        public void RegisterClient(ColyseusClient client)
        {
            Client = client;
            LobbyNetwork.InitializeClient(client);
        }

        public void Disconnect()
        {
            LobbyNetwork.Lobby.Leave();
            Client = null;
            SceneManager.LoadScene("EntryPoint");
        }
    
    }
}