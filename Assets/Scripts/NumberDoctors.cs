using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberDoctors : MonoBehaviour
{
    Text numberDoctors;

    private void Awake()
    {
        numberDoctors = GetComponent<Text>();
    }

    private void Update()
    {
        GameObject[] doctor = GameObject.FindGameObjectsWithTag("Doctor");
        int Total = doctor.Length;

        numberDoctors.text = "Total doctors: " + Total;
    }
}
