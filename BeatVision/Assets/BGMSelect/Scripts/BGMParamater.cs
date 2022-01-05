using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BGM
{
    Colosseo,
    Sengoku,
    Future,
    MainTheme,
    Exit
}

public class BGMParamater : MonoBehaviour
{
    public BGM bgm;
    public string sceneName;
}
