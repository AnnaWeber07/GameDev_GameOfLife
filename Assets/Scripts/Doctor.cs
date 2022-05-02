using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : Unit
{

    private void OnTriggerEnter2D(Collider2D collider)
    {

        Cemetery cemetery = collider.GetComponent<Cemetery>();

        if (cemetery)
        {
            Destroy(gameObject);
        }

        Regeneration regeneration = collider.GetComponent<Regeneration>();

        if (regeneration)
        {
            int temp = Random.Range(1, 5);
            if (temp % 4 == 1)
                Instantiate(gameObject);
        }
    }
}
