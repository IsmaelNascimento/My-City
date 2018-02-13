using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidationInternet : MonoBehaviour
{
    [SerializeField] private GameObject m_CanvasWarning;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            m_CanvasWarning.SetActive(true);
        }
        else
        {
            m_CanvasWarning.SetActive(false);
        }
    }
}
