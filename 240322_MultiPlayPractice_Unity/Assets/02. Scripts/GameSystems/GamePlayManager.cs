using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MP.GameSystems
{
    public class GamePlayManager : MonoBehaviour
    {
        private void Start()
        {
            PhotonNetwork.Instantiate("Characters/Dummy",
                                        Vector3.right * Random.Range(-5, 5) + Vector3.forward * Random.Range(-5f, 5f),
                                        Quaternion.identity);
        }
    }
}
