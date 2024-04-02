using MP.Authentication;
using MP.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MP.GameSystems
{
    public enum GameState
    {
        None,
        WaitUntilUserDataVerifed,
        InLobby,
        InGamePlay,

    }

    public class GameManager : SingletonMonoBase<GameManager>
    {
        [field: SerializeField] public GameState state {  get; private set; }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
        private void Update()
        {
            Workflow();
        }

        private void Workflow()
        {
            switch (state)
            {
                case GameState.None:
                    break;
                case GameState.WaitUntilUserDataVerifed:
                    {
                        if (string.IsNullOrEmpty(LoginInformation.nickname))
                            return;

                        SceneManager.LoadScene("Lobby");
                        state = GameState.InLobby;
                    }
                    break;
                case GameState.InLobby:
                    break;
                case GameState.InGamePlay:
                    break;
                default:
                    break;
            }
        }

    }
}