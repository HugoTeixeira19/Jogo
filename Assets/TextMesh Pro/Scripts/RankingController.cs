using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class RankingController : MonoBehaviour
{
    private static List<int> lista = new List<int>();
    private static string path = ".\\Assets\\Configure\\rank.txt";
    private static StreamWriter texto;
    private string[] leitura;

    

    // Start is called before the first frame update
    void Start()
    {
        leitura = File.ReadAllLines(path);

        if (leitura.Length > 0)
        {
            LeituraRank(path);
        }
    }

    public void LeituraRank(string path)
    {
        int[] numeros = new int[leitura.Length];
        Debug.Log("Textos lidos: " + leitura.Length);
        for (int i = 0; i < numeros.Length; i++)
        {
            numeros[i] = int.Parse(leitura[i]);
        }

        // Ordena a lista para decrescente
        List<int> listaDecrescente = numeros.OrderByDescending(i => i).ToList<int>();
        
        lista = listaDecrescente;
        Debug.Log("Lista quantidade: " + lista.Count);
    }

    public static void CriarArquivo(int pontuacaoAdquirida)
    {
        if (!File.Exists(path))
        {
            texto = File.CreateText(path);
            texto.Close();
            EscritaRank(pontuacaoAdquirida);
        }
        else
        {
            EscritaRank(pontuacaoAdquirida);
        }
        
    }

    public static void EscritaRank(int pontuacaoAdquirida)
    {
        texto = new StreamWriter(path, true);

        lista.Add(pontuacaoAdquirida);

        for (int i = 0; i < lista.Count; i++)
        {
            texto.WriteLine(path, File.ReadAllLines(path)[i]);
        }
        texto.Close();


        foreach (int i in lista)
        {
            Debug.Log("VALOR DO RANK: " + i);
        }
        Debug.Log("TAMANHO DE N NA LISTA: " + lista.Count);
    }

    public List<int> GetLista
    {
        get
        {
            return lista;
        }
    }

}
