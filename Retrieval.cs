using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class Retrieval : MonoBehaviour
{
    //private string secretKey = "RitoLab$$";
    public string DeptosURL =
             "https://ritolab.cl/demounitydepartamentos/getdptos.php";
    public String[] textSplit;
    public List<Departamento> Departamentos = new List<Departamento>();
    public Dictionary<int, bool> deptosDisponibles = new Dictionary<int, bool>();
    public List<int> departamentosPorPiso;
    [HideInInspector]
    public bool Called = false;
    // Start is called before the first frame update
    public void GetDptoBtn()
    {
        StartCoroutine("GetDpto");
    }

    void Start()
    {
        InvokeRepeating("GetDptoBtn", 0, 3);
    }

    IEnumerator GetDpto()
    {
        UnityWebRequest hs_get = UnityWebRequest.Get(DeptosURL);
        yield return hs_get.SendWebRequest();
        if (hs_get.error != null)
        {
            Debug.Log("Hubo un error Bajando informacion de los departamentos: " + hs_get.error);
        }
        else
        {
            for (int i = 0; i < departamentosPorPiso.Count; i++)
            {
                departamentosPorPiso[i] = 0;
            }
            Departamentos.Clear();
            string dataText = System.Text.Encoding.UTF8.GetString(hs_get.downloadHandler.data);
            textSplit = dataText.Split('-');
            for (int i = 0; i < textSplit.Length-1; i++)
            {
                String[] textSplit2 = textSplit[i].Split('_');
                Departamento depa = new Departamento();
                depa.piso = Regex.Replace(textSplit2[0],"[^\\w\\._]", "");
                int pisoDepaActual = 1;
                bool result = int.TryParse(depa.piso, out pisoDepaActual);
                if (result)
                {
                }
                else
                {
                    Debug.Log(i);
                }
                depa.deptoid = Regex.Replace(textSplit2[1],"[^\\w\\._]", "");
                depa.disponible = Regex.Replace(textSplit2[2],"[^\\w\\._]", "");
                if(deptosDisponibles.ContainsKey(int.Parse(depa.deptoid)))
                {
                    if(depa.disponible == "1")
                    {
                        deptosDisponibles[int.Parse(depa.deptoid)] = true;
                    }
                    else
                    {
                        deptosDisponibles[int.Parse(depa.deptoid)] = false;
                    }
                }
                else
                {
                    if(depa.disponible == "1")
                    { 
                        deptosDisponibles.Add(int.Parse(depa.deptoid),true);
                    }
                    else
                    {
                        deptosDisponibles.Add(int.Parse(depa.deptoid), false);
                    }
                }
                if (depa.disponible == "1")
                {
                    departamentosPorPiso[pisoDepaActual-1]++;
                }
                Departamentos.Add(depa);
            }
        }
    }

    public class Departamento
    {
        public string deptoid { get; set; }
        public string piso { get; set; }
        public string disponible { get; set; }
    }

}
