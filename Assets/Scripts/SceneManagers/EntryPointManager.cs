using Colyseus;
using NetworkManagers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils.Extensions;
using Zenject;

namespace SceneManagers
{
    public class EntryPointManager : MonoBehaviour
    {
        public static EntryPointManager Instance { get; private set; }

        private NetworkManager _networkManager;
        [Inject]
        public void Construct(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }
        
        // UI
        public TMP_InputField UsernameInput;
        public TMP_InputField PasswordInput;
        public Button ConnectBtn;
        public Button QuitBtn;
        public TMP_Text StatusMessage;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            ColyseusClient client = new ColyseusClient("ws://localhost:3000");
            _networkManager.RegisterClient(client);
            
            ConnectBtn.onClick.AddListener(ConnectToServer);
            QuitBtn.onClick.AddListener(Quit);
        }

        private async void ConnectToServer()
        {
            if (!UsernameInput.text.IsNullOrBlank() && !PasswordInput.text.IsNullOrBlank())
            {
                await _networkManager.LobbyNetwork.Connect(UsernameInput.text, PasswordInput.text);
            }
            else
            {
                UpdateStatusMessage("아이디와 비밀번호를 입력하여 주세요.");
            }

            if (_networkManager.LobbyNetwork.IsConnected)
            {
                SceneManager.LoadScene("Lobby");
            }
        }

        private void Quit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        public void UpdateStatusMessage(string message)
        {
            StatusMessage.text = message;
        }

    }
}
