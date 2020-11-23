using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ControllerFases : MonoBehaviour
{
    public static ControllerFases instance;
    public GameObject[] listaIconesFases;

    private static string path = ".\\Assets\\Configure\\info.txt";
    private static List<int> fases = new List<int>();
    private static StreamWriter texto;
    private string[] leitura;
    private static string[] aux;


    void Start()
    {
        if (!File.Exists(path))
        {
            File.CreateText(path);
        }
        else if (File.Exists(path))
        {
            leitura = File.ReadAllLines(path);
            aux = leitura;
            if (leitura.Length > 0)
            {
                LeituraRank(path);
            }
        }

        listaIconesFases[0].SetActive(true);
        for(int i = 1; i < listaIconesFases.Length; i++)
        {
            listaIconesFases[i].SetActive(false);
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
        List<int> listaDecrescente = numeros.OrderBy(i => i).ToList<int>();

        fases = listaDecrescente;
        Debug.Log("Lista quantidade: " + fases.Count);
    }

    public static void CriarArquivo(int faseSucesso)
    {
        if (!File.Exists(path))
        {
            texto = File.CreateText(path);
            texto.Close();
            EscritaRank(faseSucesso);
        }
        else
        {
            EscritaRank(faseSucesso);
        }

    }

    public static void EscritaRank(int faseSucesso)
    {
        texto = new StreamWriter(path, true);

        if (aux.Length < 1)
        {
            texto.Write(faseSucesso);
            texto.Close();
        }
        else
        {
            texto.WriteLine();
            texto.Write(faseSucesso);
            texto.Close();
        }
    }

    void Update()
    {
        if (fases != null)
        {
            for (int i = 0; i <= fases.Count; i++)
            {
                if (i < 10)
                {
                    listaIconesFases[i].SetActive(true);
                }
            }
        }
    }

    public List<int> Fases
    {
        set
        {
            fases = value;
        }
        get
        {
            return fases;
        }
    }
}
