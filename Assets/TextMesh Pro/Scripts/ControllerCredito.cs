using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerCredito : MonoBehaviour
{
    public void onClickMenu()
    {
        SceneManager.LoadScene("Scenes/Menu");
    }

    public void onClickFacePage()
    {
        Application.OpenURL("https://pt-br.facebook.com/unicluster.face");
    }

    public void onClickTwitterPage()
    {
        Application.OpenURL("https://twitter.com/unicluster");
    }
}
