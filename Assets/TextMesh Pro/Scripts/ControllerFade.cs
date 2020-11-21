using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerFade : MonoBehaviour
{
    public Animator animatorLogo;
    public string scene;

    public void Start()
    {
        Invoke("FadeIn", 3);
        Invoke("ChangeScene", 5);
    }

    public void FadeIn()
    {
        animatorLogo.Play("FadeInFundoComeco");
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("Scenes/" + scene);
    }
}
