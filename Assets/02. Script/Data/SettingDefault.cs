using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultSettingData", menuName = "Scriptable Object/Default Setting Data")]
public class SettingDefault : ScriptableObject
{
    [SerializeField] string path;

    [SerializeField] SoundData sound;

    public string Path => path;

    public SoundData Sound => sound;
}
