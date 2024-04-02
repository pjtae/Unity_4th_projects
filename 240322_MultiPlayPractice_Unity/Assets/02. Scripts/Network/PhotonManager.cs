using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace MP.Network
{
    public class PhotonManager : MonoBehaviourPunCallbacks //��� �ν����Ͱ� �� �����Ǿ� �ִ�
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
            PhotonNetwork.AutomaticallySyncScene = true; //PhotonNetwork.LoadLevel() ȣ��� ���� ������ �濡�ִ� ��� Ŭ���̾�Ʈ�� ���� ����ȭ �ϴ� �ɼ�
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
            });//������� ����
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

