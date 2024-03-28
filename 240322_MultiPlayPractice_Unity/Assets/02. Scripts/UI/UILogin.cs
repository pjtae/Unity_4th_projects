using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase; //������ üũ�� ���� ����
using Firebase.Auth;
using Firebase.Extensions; // Ȯ����
namespace MP.UI
{
    public class UILogin : MonoBehaviour
    {
        //NameingConvention
        private InputField _id;
        private InputField _pw;
        private Button _tryLogin;
        private Button _register;

        private async void Awake()
        {
            _id = transform.Find("Panel/InputField (TMP) - ID").GetComponent<InputField>();
            _pw = transform.Find("Panel/InputField (TMP) - PW").GetComponent<InputField>();
            _tryLogin = transform.Find("Panel/Button - TryLogin").GetComponent<Button>();
            _register = transform.Find("Panel/Button - Register").GetComponent<Button>();
            //git hub�� ����ϰ� ��Ÿ������ ��ġ�� �ʰ� �ϱ� ���ؼ� ���� �������ش�.

            var dependecnyState = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (dependecnyState != DependencyStatus.Available)
                throw new Exception();

            _tryLogin.onClick.AddListener(() =>
            {
                FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(_id.text, _pw.text)
                                            .ContinueWithOnMainThread(task =>
                                            {
                                                if (task.IsCanceled)
                                                {
                                                    // todo -> �α��� ��ҿ� ���� �˸�â �˾�
                                                    UIManager.instance.Get<UIWarringWindow>()
                                                                      .Show("Canceld login");            
                                                    return;
                                                }

                                                if (task.IsFaulted)
                                                {
                                                    // todo -> �α��� ���п� ���� �˸�â �˾�
                                                    UIManager.instance.Get<UIWarringWindow>()
                                                                      .Show("Faulted login");
                                                    return;
                                                }
                                                // todo -> �α��� ������ ���� �˸�â �˾�
                                                // todo -> �α��� ���� �Ŀ� ������ �߰����� (�� ��ȭ, ���ҽ� �ε�...)
                                            });
            });


            _register.onClick.AddListener(() =>
            {
                FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(_id.text, _pw.text)
                                            //.ContinueWith//ü�̴��� �� �� ���� ���ǻ��� ����Ƽ�� ���̶���Ű�� �����Ϸ��� �ٸ� �� ����Ѵ�
                                            .ContinueWithOnMainThread(task =>
                                            {
                                                if (task.IsCanceled)
                                                {
                                                    // todo -> ȸ������ ��ҿ� ���� �˸�â �˾�    TODO�ۼ��� ����ȭ
                                                    UIManager.instance.Get<UIWarringWindow>()
                                                                      .Show("Canceld registeration");
                                                    return;
                                                }

                                                if(task.IsFaulted)
                                                {
                                                    // todo -> ȸ������ ���п� ���� �˸�â �˾�
                                                    UIManager.instance.Get<UIWarringWindow>()
                                                                      .Show("Faulted registeration");
                                                    return;
                                                }

                                                //task.Result �� �ʿ��ϴٸ� �������

                                                // todo -> ȸ������ ������ ���� �˸�â �˾�
                                            });
                
            });
            
        }
    }
}