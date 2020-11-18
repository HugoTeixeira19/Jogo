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
    private static string[] aux;

    

    // Start is called before the first frame update
    void Start()
    {
        if (!File.Exists(path))
        {
            File.CreateText(path);
        }

        leitura = File.ReadAllLines(path);
        aux = leitura;
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
        
        if(aux.Length < 1)
        {
            texto.Write(pontuacaoAdquirida);
            texto.Close();
        } else
        {
            texto.WriteLine();
            texto.Write(pontuacaoAdquirida);
            texto.Close();
        }
    }

    public List<int> GetLista
    {
        get
        {
            return lista;
        }
    }

}
