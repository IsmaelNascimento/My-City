using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PushStart.MyCity
{
    public class ScreenLogin : MonoBehaviour
    {
        [Header("Popup Notification")]
        [SerializeField] private GameObject m_NotificationError;
        [SerializeField] private GameObject m_NotificationSuccess;
        [Space(10)]

        [Header("User")]
        [SerializeField] private InputField m_FieldUsername;
        [SerializeField] private InputField m_FieldPassword;
        [Space(10)]

        [Header("Wait Login")]
        [SerializeField] private GameObject m_WaitLogin;
        [SerializeField] private GameObject m_ButtonLogin;
        [Space(10)]

        [SerializeField] private AudioSource m_FxClick;

        public void OnButtonConfirmLoginClicked()
        {
            m_FxClick.Play();
            m_WaitLogin.SetActive(true);
            m_ButtonLogin.SetActive(false);
            StartCoroutine(WaitRequest_Coroutine());
        }

        private IEnumerator WaitRequest_Coroutine()
        {
            yield return StartCoroutine(Request.RequestData_Coroutine(m_FieldUsername.text, m_FieldPassword.text));

            if (Request.callbackRequest.Equals("error"))
            {
                m_NotificationError.GetComponent<Animator>().Play("Fade Effect");

                yield return new WaitForSeconds(3f);

                m_WaitLogin.SetActive(false);
                m_ButtonLogin.SetActive(true);
                m_NotificationError.GetComponent<Animator>().Play("Fade Effect Exit");
            }
            else
            {
                m_NotificationSuccess.GetComponent<Animator>().Play("Fade Effect");

                yield return new WaitForSeconds(2f);

                m_WaitLogin.SetActive(false);

                yield return new WaitForSeconds(.5f);

                m_NotificationSuccess.GetComponent<Animator>().Play("Fade Effect Exit");
                LoadScene.ChangeToScene("Gameplay");
            }
        }
    }
}