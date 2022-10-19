using UnityEngine;
using System.Collections;

public class BGMControler : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void play()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    public void stop()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
    }

    // Update is called once per frame
    void Update()
    {

    }
}