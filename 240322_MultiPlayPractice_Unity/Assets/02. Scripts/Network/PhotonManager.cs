using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace MP.Network
{
    public class PhotonManager : MonoBehaviourPunCallbacks //모든 인스펙터가 다 구현되어 있다
    {
        #region Singleton
        public static PhotonManager instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new GameObject(typeof(PhotonManager).Name).AddComponent<PhotonManager>();
                    DontDestroyOnLoad(s_instance);
                }
                return s_instance;
            }
        }

        private static PhotonManager s_instance;
        #endregion

        private void Awake()
        {
            if (PhotonNetwork.IsConnected == false)
            {
                bool isConnected = PhotonNetwork.ConnectUsingSettings();
                Debug.Assert(isConnected, "[PhotonManager] : Connected to photon serer.");
            }
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnected();
            PhotonNetwork.AutomaticallySyncScene = true; //PhotonNetwork.LoadLevel() 호출시 현재 동일한 방에있는 모든 클라이언트의 씬을 동기화 하는 옵션
            //PhotonNetwork.JoinLobby();
            //PhotonNetwork.CreateRoom();

        }

        public override void OnJoinedLobby()
        {
            base.OnLeftLobby();
            Debug.Log($"[PhotonManager] : Joined Lobby.");
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable()
            {
                {"is Ready",false }
            });//레디상태 설정
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable()
            {

            });
        }
    }


}

