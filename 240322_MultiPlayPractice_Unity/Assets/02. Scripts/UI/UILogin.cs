using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase; //������ üũ�� ���� ����
using Firebase.Auth;
using Firebase.Extensions; // Ȯ����
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
            //git hub�� ����ϰ� ��Ÿ������ ��ġ�� �ʰ� �ϱ� ���ؼ� ���� �������ش�.

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
                                                    // todo -> �α��� ��ҿ� ���� �˸�â �˾�
                                                    UIManager.instance.Get<UIWarningWindow>()
                                                                      .Show("Canceld login");
                                                    return;
                                                }

                                                if (task.IsFaulted)
                                                {
                                                    // todo -> �α��� ���п� ���� �˸�â �˾�
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

                                                // todo -> �α��� ������ ���� �˸�â �˾�
                                                // todo -> �α��� ���� �Ŀ� ������ �߰����� (�� ��ȯ, ���ҽ� �ε�...)
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
                                            //.ContinueWith//ü�̴��� �� �� ���� ���ǻ��� ����Ƽ�� ���̶���Ű�� �����Ϸ��� �ٸ� �� ����Ѵ�
                                            .ContinueWithOnMainThread(task =>
                                            {
                                                if (task.IsCanceled)
                                                {
                                                    // todo -> ȸ������ ��ҿ� ���� �˸�â �˾�    TODO�ۼ��� ����ȭ
                                                    UIManager.instance.Get<UIWarningWindow>()
                                                                      .Show(task.Exception.Message);
                                                    return;
                                                }

                                                if (task.IsFaulted)
                                                {
                                                    // todo -> ȸ������ ���п� ���� �˸�â �˾�
                                                    UIManager.instance.Get<UIWarningWindow>()
                                                                      .Show(task.Exception.Message);
                                                    return;
                                                }

                                                //task.Result �� �ʿ��ϴٸ� �������

                                                // todo -> ȸ������ ������ ���� �˸�â �˾�
                                            });

            });
        }

        private async Task<string> GetNicknameAsync(string userKey) //�񵿱� ����� �޾ƾ��ϱ� ������ 
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
                         @"^[^@\s]+@[^@\s]+\.[^@\s]+$"); //[^@\s] �� ��ȣ $�����ºκ�
        }

        private bool IsValidPW(string pw)
        {
            return pw.Length >= 6;
        }

    }
}