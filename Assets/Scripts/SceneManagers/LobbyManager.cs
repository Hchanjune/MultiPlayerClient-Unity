using NetworkManagers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace SceneManagers
{
    public class LobbyManager : MonoBehaviour
    {
        private NetworkManager _networkManager;
        [Inject]
        public void Constructor(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }
        
        // UI
        public Button LogoutBtn;
        
        void Start()
        {
            LogoutBtn.onClick.AddListener(OnLogout);
        }
        
        void Update()
        {
        
        }


        private void OnLogout()
        {
            _networkManager.Disconnect();
        }
        
    }
}
