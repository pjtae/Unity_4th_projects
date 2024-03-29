using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using MP.Authentication;

namespace MP.UI
{
    public class UIProfileSettingWindow : UIBase
    {
        private TMP_InputField _nickname;
        private Button _confirm;


        protected override void Awake()
        {
            base.Awake();

            _nickname = transform.Find("Panel/InputField (TMP) - Nickname").GetComponent<TMP_InputField>();
            _confirm = transform.Find("Panel/Button - Confirm").GetComponent<Button>();
            onInputActionEnableChanged += value =>
            {
                _nickname.interactable = value;
                _confirm.interactable = value;
            };

            _confirm.onClick.AddListener(() =>
            {
                string nickname = _nickname.text;

                //웹서버에 있는 컬렉션에 접근 요청하고 반환할수있는 기능을 가지고 있는 객체 경로에 없으면 알아서 만들어줌
                CollectionReference usersCollectionRef = FirebaseFirestore.DefaultInstance.Collection("users");
                usersCollectionRef.GetSnapshotAsync()//비동기 구분
                                  .ContinueWithOnMainThread(task =>
                                  {
                                      // todo => profile의 닉네임 중복검사, 중복된거 잇으면 알림창띄어줌.
                                      // 없으면 프로필정보 새로 등록하고 로비씬으로 넘어가야함.? 계속 중복검사 해야하는거 아닌가

                                      //닉네임 중복검사
                                      foreach (DocumentSnapshot document in task.Result.Documents)
                                      {
                                          if (document.GetValue<string>("nickname").Equals(nickname))
                                          {
                                              UIManager.instance.Get<UIWarningWindow>()
                                                                .Show("The nickname is already exist.");

                                              return;
                                          }
                                      }

                                      FirebaseFirestore.DefaultInstance
                                      .Collection("users")
                                            .Document(LoginInformation.userKey)
                                                .SetAsync(new Dictionary<string, object>
                                                {
                                                    {"nickname", nickname}
                                                })
                                                .ContinueWithOnMainThread(task =>
                                                {
                                                    LoginInformation.nickname = nickname;    
                                                });
                                                
                                      
                                      //runtype dynamic
                                  });
            });

            _confirm.interactable = false;
            _nickname.onValueChanged.AddListener(value => _confirm.interactable = IsValidNickname(value));
        }

        private bool IsValidNickname(string nickname)
        {
            return (nickname.Length > 1) && (nickname.Length < 11);
        }

    }
}
