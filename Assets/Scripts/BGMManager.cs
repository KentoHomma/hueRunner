using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    static BGMControler prevaudiomaster = null;
    public BGMControler audiomaster;

    void Start()
    {
        if (prevaudiomaster == null)
        {
            prevaudiomaster = audiomaster;
            audiomaster.play();
        }
        else
        {
            audiomaster.stop();
            Destroy(audiomaster);
        }
    }
}
