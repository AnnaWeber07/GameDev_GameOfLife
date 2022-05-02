using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberVirus : MonoBehaviour
{
    Text numberVirus;

    private void Awake()
    {
        numberVirus = GetComponent<Text>();
    }

    private void Update()
    {
        GameObject[] virus = GameObject.FindGameObjectsWithTag("Virus");
        int Total = virus.Length;

        numberVirus.text = "Total viruses: " + Total;
    }
}
