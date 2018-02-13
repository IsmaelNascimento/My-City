using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundHelper : MonoBehaviour
{
    public TypeSound typeSound;

    public enum TypeSound
    {
        Music, 
        Fx
    }
}
