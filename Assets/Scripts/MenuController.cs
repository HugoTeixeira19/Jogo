using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayScene()
    {
        SceneManager.LoadScene("Scenes/ListaFasesScene");
    }

    public void CreditoScene()
    {
        SceneManager.LoadScene("Scenes/CreditoScene");
    }
    public void TutorialScene()
    {
        SceneManager.LoadScene("Scenes/TutorialScene");
    }
    public void RankingScene()
    {
        SceneManager.LoadScene("Scenes/RankingScene");
    }
    public void Sair()
    {
        Application.Quit();
    }
}
