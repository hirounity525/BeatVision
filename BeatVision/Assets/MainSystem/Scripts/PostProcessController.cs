using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessController : MonoBehaviour
{
    [SerializeField] VolumeProfile prePostProcess;
    [SerializeField] VolumeProfile movementPostProcess;

    Volume volume;

    private void Awake()
    {
        volume = GetComponent<Volume>();
    }

    public void ChangePrePostProcess()
    {
        volume.profile = prePostProcess;
    }

    public void ChangeMovementPostProcess()
    {
        volume.profile = movementPostProcess;
    }
}
