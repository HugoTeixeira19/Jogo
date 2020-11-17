using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelRankingController : MonoBehaviour
{
    public RectTransform painelColocacoes;
    public TextMeshProUGUI textoSemResultados;
    public TextMeshProUGUI[] textPontuacao = new TextMeshProUGUI[5];
    public GameObject[] textColocacao = new GameObject[5];

    public List<int> lista = new List<int>();
    public bool isReady;

    void Start()
    {
        ControlePaineis(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<RankingController>().GetLista != null)
        {
            lista = GetComponent<RankingController>().GetLista;

            if (lista.Count > 0)
            {
                ControlePaineis(true);
            }
        }

        if (painelColocacoes.gameObject.active && lista != null)
        {
            for (int i = 0; i < textPontuacao.Length; i++)
            {
                if (i < lista.Count)
                {
                    textPontuacao[i].SetText(lista[i].ToString());
                }
                else
                {
                    textColocacao[i].SetActive(false);
                }
            }
        }
    }

    private void ControlePaineis(bool input)
    {
        painelColocacoes.gameObject.SetActive(input);
        textoSemResultados.gameObject.SetActive(!input);
    }
}
