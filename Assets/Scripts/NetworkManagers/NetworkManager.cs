using Colyseus;
using NetworkManagers.Lobby;
using UnityEngine.SceneManagement;
using Zenject;

namespace NetworkManagers
{
    public class NetworkManager
    {
        public ColyseusClient Client;

        public ClientInfo ClientInfo;
    
        public LobbyNetwork LobbyNetwork { get; private set; }

        [Inject]
        public void Construct(LobbyNetwork lobbyNetwork)
        {
            Client = new ColyseusClient("ws://localhost:3000");;
            InitializeLobbyNetwork(lobbyNetwork);
        }

        private void InitializeLobbyNetwork(LobbyNetwork lobbyNetwork)
        {
            LobbyNetwork = lobbyNetwork;
            LobbyNetwork.OnConnection += RegisterClientInfo;
            LobbyNetwork.InitializeClient(Client);
        }

        private void RegisterClientInfo(ClientInfo clientInfo)
        {
            ClientInfo = clientInfo;
        }

        public void Disconnect()
        {
            LobbyNetwork.Lobby.Leave();
            Client = null;
            ClientInfo = null;
            SceneManager.LoadScene("EntryPoint");
        }
    
    }
}