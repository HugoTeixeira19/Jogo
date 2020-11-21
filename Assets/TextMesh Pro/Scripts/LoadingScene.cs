using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    [Header("Nome da cena a ser carregada")]
    public string cenaACarregar;
    [Space(20)]
    public Texture textureFundo;
    public Texture barraProgresso;
    public string textoProgresso = "Carregando: ";
    public Color corTexto = Color.white;
    public Font fonteTexto;
    [Space(20)]
    [Range(0.5f, 3.0f)]
    public float tamanhoTexto = 3.0f;
    [Range(1, 10)]
    public int larguraBarra = 8;
    [Range(1, 10)]
    public int alturaBarra = 2;
    [Range(-4.5f, 4.5f)]
    public float deslocarBarra = 4;
    [Range(-8, 4)]
    public float deslocarTextoX = 4;
    [Range(-4.5f, 4.5f)]
    public float deslocarTextoY = 3;

    private bool iniciarCarregamento = false;
    private int progresso = 0;


    public void Play(string cena)
    {
        StartCoroutine(CenaDeCarregamento(cena));
    }

    IEnumerator CenaDeCarregamento (string cena)
    {
        iniciarCarregamento = true;

        AsyncOperation carregamento = Application.LoadLevelAsync(cena);
        while(!carregamento.isDone)
        {
            progresso = (int)(carregamento.progress * 100);
            yield return null;
        }
    }

    void OnGUI()
    {
        if(iniciarCarregamento)
        {
            GUI.contentColor = corTexto;
            GUI.skin.font = fonteTexto;
            GUI.skin.label.fontSize = (int) (Screen.height / 50 * tamanhoTexto);

            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), textureFundo);

            // Texto de carregamento
            float deslocXTexto = (Screen.height / 10) * deslocarTextoX;
            float deslocYTexto = (Screen.height / 10) * deslocarTextoY;
            GUI.Label(new Rect(Screen.width / 2 + deslocXTexto, Screen.height / 2 + deslocYTexto, Screen.width, Screen.height),
                textoProgresso + progresso + "%");

            // Barra de progresso
            float largura = Screen.width * (larguraBarra / 10.0f);
            float altura = (Screen.height / 50) * alturaBarra;
            float deslocYBar = (Screen.height / 10) * deslocarBarra;
            GUI.DrawTexture(new Rect(Screen.width / 2 - largura / 2, Screen.height / 2 - (altura / 2) + deslocYBar, largura * (progresso / 100.0f),
                altura), barraProgresso);
        }    
    }
}
