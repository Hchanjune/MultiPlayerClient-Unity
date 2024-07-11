using Colyseus;
using NetworkManagers;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils.Extensions;
using Zenject;

namespace SceneManagers
{
    public class EntryPointManager : MonoBehaviour
    {

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

        }
        
        void Start()
        {
            ConnectBtn.onClick.AddListener(ConnectToServer);
            QuitBtn.onClick.AddListener(Quit);
        }

        private async void ConnectToServer()
        {
            if (!UsernameInput.text.IsNullOrBlank() && !PasswordInput.text.IsNullOrBlank())
            {
                bool isConnected = await _networkManager.LobbyNetwork.Connect(UsernameInput.text, PasswordInput.text, UpdateStatusMessage);
                if (isConnected)
                {
                    SceneManager.LoadSceneAsync("Lobby");
                }
            }
            else
            {
                UpdateStatusMessage("아이디와 비밀번호를 입력하여 주세요.");
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
