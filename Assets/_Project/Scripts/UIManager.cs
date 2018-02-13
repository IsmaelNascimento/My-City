using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PushStart.MyCity
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private AudioSource m_FxClick;

        public void OnButtonPauseClicked(GameObject panelPause)
        {
            m_FxClick.Play();
            GameMaster.Instance.Pause = true;
            panelPause.SetActive(true);
        }

        public void OnButtonBackClicked(GameObject panelPause)
        {
            m_FxClick.Play();
            GameMaster.Instance.Pause = false;
            panelPause.SetActive(false);
        }

        public void OnButtonExitClicked()
        {
            m_FxClick.Play();
            DataSave.SaveAll();
            Application.Quit();
        }

        public void ControllerAllMusicGame(bool mute)
        {
            m_FxClick.Play();

            var musics = FindObjectsOfType(typeof(SoundHelper)) as SoundHelper[];

            for (var i = 0; i < musics.Length; i++)
            {
                if (musics[i].typeSound == SoundHelper.TypeSound.Music)
                {
                    musics[i].GetComponent<AudioSource>().mute = mute;
                }
            }
        }

        public void ControllerAllFxGame(bool mute)
        {
            m_FxClick.Play();

            var fxs = FindObjectsOfType(typeof(SoundHelper)) as SoundHelper[];

            for (var i = 0; i < fxs.Length; i++)
            {
                if (fxs[i].typeSound == SoundHelper.TypeSound.Fx)
                {
                    fxs[i].GetComponent<AudioSource>().mute = mute;
                }
            }
        }
    }
}