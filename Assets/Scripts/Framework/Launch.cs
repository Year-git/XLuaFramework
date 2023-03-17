using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

public class Launch : MonoBehaviour
{
    public GameMode GameMode = GameMode.EditorMode;
    void Awake()
    {
        ConstDefine.GameMode = GameMode;
    }
}
