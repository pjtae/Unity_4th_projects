using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase; //의존성 체크를 위한 연결
using Firebase.Auth;
using Firebase.Extensions; // 확장기능
using System.Text.RegularExpressions;
using MP.Authentication;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEditor.VersionControl;
using UnityEngine.SceneManagement;

namespace MP.UI
{
    public class UILogin : MonoBehaviour
    {
        //NameingConvention
        private TMP_InputField _id;
        private TMP_InputField _pw;
        private Button _tryLogin;
        private Button _register;

        private async void Awake()
        {
            _id = transform.Find("Panel/InputField (TMP) - ID").GetComponent<TMP_InputField>();
            _pw = transform.Find("Panel/InputField (TMP) - PW").GetComponent<TMP_InputField>();
            _tryLogin = transform.Find("Panel/Button - TryLogin").GetComponent<Button>();
            _register = transform.Find("Panel/Button - Register").GetComponent<Button>();
            //git hub를 사용하고 메타파일을 겹치지 않게 하기 위해서 직접 연결해준다.

            var dependecnyState = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (dependecnyState != DependencyStatus.Available)
                throw new Exception();

            _tryLogin.onClick.AddListener(() =>
            {
                string id = _id.text;
                string pw = _pw.text;
                FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(_id.text, _pw.text)
                                            .ContinueWithOnMainThread(async task =>
                                            {
                                                if (task.IsCanceled)
                                                {
                                                    // todo -> 로그인 취소에 대한 알림창 팝업
                                                    UIManager.instance.Get<UIWarningWindow>()
                                                                      .Show("Canceld login");
                                                    return;
                                                }

                                                if (task.IsFaulted)
                                                {
                                                    // todo -> 로그인 실패에 대한 알림창 팝업
                                                    UIManager.instance.Get<UIWarningWindow>()
                                                                      .Show("Faulted login");
                                                    return;
                                                }

                                                LoginInformation.Refresh(id);

                                                Debug.Log("Successfully logged in.");

                                                await FirebaseFirestore.DefaultInstance
                                                     .Collection("users")
                                                         .Document(LoginInformation.userKey)
                                                                .GetSnapshotAsync()
                                                                .ContinueWithOnMainThread(task =>
                                                                {
                                                                    Dictionary<string, object> documentDictionary = task.Result.ToDictionary();

                                                                    Debug.Log("Finished Get profile document");

                                                                    if (documentDictionary?.TryGetValue("nickname", out object value) ?? false)
                                                                        SceneManager.LoadScene("Lobby");
                                                                    else
                                                                    {
                                                                        UIManager.instance.Get<UIProfileSettingWindow>().Show();
                                                                    }
                                                                });
                                                //CheckProfile(id);

                                                // todo -> 로그인 성공에 대한 알림창 팝업
                                                // todo -> 로그인 성공 후에 실행할 추가내용 (씬 전환, 리소스 로드...)
                                            });
            });


            _register.onClick.AddListener(() =>
            {
                string id = _id.text;
                string pw = _pw.text;

                if (IsValidID(id) == false)
                {
                    UIManager.instance.Get<UIWarningWindow>()
                                      .Show("Wrong email format");
                    return;
                }

                if (IsValidPW(pw) == false)
                {
                    UIManager.instance.Get<UIWarningWindow>()
                                      .Show($"Wrong password format.");
                    return;
                }

                FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(id, pw)
                                            //.ContinueWith//체이닝을 할 수 있음 주의사항 유니티의 하이라이키를 접근하려면 다른 걸 써야한다
                                            .ContinueWithOnMainThread(task =>
                                            {
                                                if (task.IsCanceled)
                                                {
                                                    // todo -> 회원가입 취소에 대한 알림창 팝업    TODO작성시 통일화
                                                    UIManager.instance.Get<UIWarningWindow>()
                                                                      .Show(task.Exception.Message);
                                                    return;
                                                }

                                                if (task.IsFaulted)
                                                {
                                                    // todo -> 회원가입 실패에 대한 알림창 팝업
                                                    UIManager.instance.Get<UIWarningWindow>()
                                                                      .Show(task.Exception.Message);
                                                    return;
                                                }

                                                //task.Result 가 필요하다면 받으면됨

                                                // todo -> 회원가입 성공에 대한 알림창 팝업
                                            });

            });
        }

        private async Task<string> GetNicknameAsync(string userKey) //비동기 결과를 받아야하기 때문에 
        {
            string nickname = string.Empty;

            await FirebaseFirestore.DefaultInstance
                    .Collection("users")
                        .Document(userKey)
                            .GetSnapshotAsync()
                            .ContinueWithOnMainThread(task =>
                            {
                                Dictionary<string, object> documentDicionary = task.Result.ToDictionary();
                                if (documentDicionary.TryGetValue("nickname", out object value))
                                    nickname = (string)value;
                            });
            
            return nickname;
        }

        private bool IsValidID(string id)
        {
            return Regex.IsMatch(id,
                         @"^[^@\s]+@[^@\s]+\.[^@\s]+$"); //[^@\s] 엣 기호 $끝나는부분
        }

        private bool IsValidPW(string pw)
        {
            return pw.Length >= 6;
        }

    }
}