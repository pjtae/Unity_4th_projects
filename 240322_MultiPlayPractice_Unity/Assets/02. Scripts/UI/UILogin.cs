using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase; //의존성 체크를 위한 연결
using Firebase.Auth;
using Firebase.Extensions; // 확장기능
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
            //git hub를 사용하고 메타파일을 겹치지 않게 하기 위해서 직접 연결해준다.

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
                                                    // todo -> 로그인 취소에 대한 알림창 팝업
                                                    UIManager.instance.Get<UIWarringWindow>()
                                                                      .Show("Canceld login");            
                                                    return;
                                                }

                                                if (task.IsFaulted)
                                                {
                                                    // todo -> 로그인 실패에 대한 알림창 팝업
                                                    UIManager.instance.Get<UIWarringWindow>()
                                                                      .Show("Faulted login");
                                                    return;
                                                }
                                                // todo -> 로그인 성공에 대한 알림창 팝업
                                                // todo -> 로그인 성공 후에 실행할 추가내용 (씬 전화, 리소스 로드...)
                                            });
            });


            _register.onClick.AddListener(() =>
            {
                FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(_id.text, _pw.text)
                                            //.ContinueWith//체이닝을 할 수 있음 주의사항 유니티의 하이라이키를 접근하려면 다른 걸 써야한다
                                            .ContinueWithOnMainThread(task =>
                                            {
                                                if (task.IsCanceled)
                                                {
                                                    // todo -> 회원가입 취소에 대한 알림창 팝업    TODO작성시 통일화
                                                    UIManager.instance.Get<UIWarringWindow>()
                                                                      .Show("Canceld registeration");
                                                    return;
                                                }

                                                if(task.IsFaulted)
                                                {
                                                    // todo -> 회원가입 실패에 대한 알림창 팝업
                                                    UIManager.instance.Get<UIWarringWindow>()
                                                                      .Show("Faulted registeration");
                                                    return;
                                                }

                                                //task.Result 가 필요하다면 받으면됨

                                                // todo -> 회원가입 성공에 대한 알림창 팝업
                                            });
                
            });
            
        }
    }
}