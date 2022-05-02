using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiater : MonoBehaviour
{
    public static float generationInterval = 1F;

    int[,] virusArray;
    int[,] doctorArray;
    int[,] cemeteryArray;
    int[,] coffeeArray;

    public int gridHeight;
    private int gridWidth;

    private float cellSize;
    private float cemeterySize;
    private float coffeeSize;

    public Regeneration regeneration;
    public Cemetery cemetery;
    public Doctor doctor;
    public Virus virus;


    private void Start()
    {
        gridWidth = Mathf.RoundToInt(gridHeight * Camera.main.aspect);

        cellSize = (Camera.main.orthographicSize * 2) / gridHeight;
        cemeterySize = (Camera.main.orthographicSize * 2) / gridHeight;
        coffeeSize = (Camera.main.orthographicSize * 2) / gridHeight;

        virusArray = new int[gridHeight, gridWidth];
        doctorArray = new int[gridHeight, gridWidth];
        cemeteryArray = new int[gridHeight, gridWidth];
        coffeeArray = new int[gridHeight, gridWidth];

        GenerateDoughnuts();
        GeneratePrison();
        InvokeRepeating("NewGenerationUpdate", generationInterval, generationInterval);
    }

    private void NewGenerationUpdate()
    {
        ApplyRules();
        GenerateCells(virus, ref virusArray, "Virus");
        GenerateCells(doctor, ref doctorArray, "Doctor");
    }

    private bool CheckOverlapping(int[,] arr, int row, int col)
    {
        int start, start1, final, final1;

        if (row - 20 < 0)
            start = 0;
        else
            start = row - 20;
        if (row + 20 >= gridHeight)
            final = gridHeight - 1;
        else
            final = row + 20;


        if (col - 20 < 0)
            start1 = 0;
        else
            start1 = col - 20;
        if (col + 20 >= gridWidth)
            final1 = gridWidth - 1;
        else
            final1 = col + 20;


        for (int i = start; i <= final; i++)
            for (int j = start1; j <= final1; j++)
                if (arr[i, j] == 1)
                    return false;


        return true;
    }

    private void GenerateDoughnuts()
    {
        foreach (GameObject cell in GameObject.FindGameObjectsWithTag("Regeneration"))
        {
            Destroy(cell);
        }
        for (int i = 0; i < 3; i++)
        {
            coffeeArray[Random.Range(20, 70), Random.Range(30, 120)] = 1;
        }

        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                if (coffeeArray[i, j] == 0) continue;
                Vector3 Position = new Vector3(
                    j * coffeeSize + coffeeSize / 2,
                    (coffeeSize * gridHeight) - (i * coffeeSize + coffeeSize / 2),
                    0
                );


                Regeneration clone3 = Instantiate(regeneration, Position, Quaternion.identity) as Regeneration;
            }
        }
    }

    private void GeneratePrison()
    {
        foreach (GameObject cell in GameObject.FindGameObjectsWithTag("Prison"))
        {
            Destroy(cell);
        }

        int count = 0;
        while (count != 5)
        {
            int row = Random.Range(20, 70);
            int col = Random.Range(30, 120);

            if (!CheckOverlapping(coffeeArray, row, col))
            {
                cemeteryArray[row, col] = 1;
                count++;
            }
        }

        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                if (cemeteryArray[i, j] == 0) continue;
                Vector3 Position = new Vector3(
                    j * cemeterySize + cemeterySize / 2,
                    (cemeterySize * gridHeight) - (i * cemeterySize + cemeterySize / 2),
                    0
                );

                Cemetery clone1 = Instantiate(cemetery, Position, Quaternion.identity) as Cemetery;
            }
        }
    }

    private void GenerateCells<T>(T animal, ref int[,] arr, string tag) where T : Unit
    {
        foreach (GameObject cell in GameObject.FindGameObjectsWithTag(tag))
        {
            Destroy(cell);
        }

        for (int i = 0; i < 5; i++)
        {
            arr[Random.Range(20, 70), Random.Range(30, 120)] = 1;
        }

        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                if (arr[i, j] == 0) continue;
                Vector3 Position = new Vector3(
                    j * cellSize + cellSize / 2,
                    (cellSize * gridHeight) - (i * cellSize + cellSize / 2),
                    0
                );

                T clone = Instantiate(animal, Position, Quaternion.identity) as T;
            }
        }
    }


    private void ApplyRules()
    {
        CatchVirus();
        BeatDoctors();
        virusArray = Breed(virusArray);
        doctorArray = Breed(doctorArray);
    }

    private int[,] Breed(int[,] arr)
    {
        int[,] nextGenGrid = new int[gridHeight, gridWidth];
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                int livingNeighbours = CountLivingNeighbours(i, j, arr);
                if (livingNeighbours == 3)  // Reproduction, exactly 3 neighbours
                {
                    nextGenGrid[i, j] = 1;
                }
                else if (livingNeighbours == 2 && arr[i, j] == 1)   // exactly 2 neigh, the live cell survives
                {
                    nextGenGrid[i, j] = 1;
                }
                else if (livingNeighbours == 0)
                {
                    nextGenGrid[i, j] = 0;
                }

            }
        }

        return arr;	// GOING TO THE NEXT GEN!!! 
    }

    //thieves will get caught 
    private void CatchVirus()
    {
        for (int i = 1; i < gridHeight - 1; i++)
        {
            for (int j = 1; j < gridWidth - 1; j++)
            {
                if (doctorArray[i, j] == 0) continue;

                if (virusArray[i - 1, j] == 1)
                {
                    virusArray[i - 1, j] = 0;
                    doctorArray[i, j] = 0;
                    doctorArray[i - 1, j] = 1;
                    continue;
                }

                if (virusArray[i - 1, j - 1] == 1)
                {
                    virusArray[i - 1, j - 1] = 0;
                    doctorArray[i - 1, j - 1] = 1;
                    doctorArray[i, j] = 0;
                    continue;
                }

                if (virusArray[i - 1, j + 1] == 1)
                {
                    virusArray[i - 1, j + 1] = 0;
                    doctorArray[i - 1, j + 1] = 1;
                    doctorArray[i, j] = 0;
                    continue;
                }

                if (virusArray[i, j - 1] == 1)
                {
                    virusArray[i, j - 1] = 0;
                    doctorArray[i, j - 1] = 1;
                    doctorArray[i, j] = 0;
                    continue;
                }

                if (virusArray[i, j + 1] == 1)
                {
                    virusArray[i, j + 1] = 0;
                    doctorArray[i, j + 1] = 1;
                    doctorArray[i, j] = 0;
                    continue;
                }

                if (virusArray[i + 1, j - 1] == 1)
                {
                    virusArray[i + 1, j - 1] = 0;
                    doctorArray[i + 1, j - 1] = 1;
                    doctorArray[i, j] = 0;
                    continue;
                }

                if (virusArray[i + 1, j] == 1)
                {
                    virusArray[i + 1, j] = 0;
                    doctorArray[i + 1, j] = 1;
                    doctorArray[i, j] = 0;
                    continue;
                }

                if (virusArray[i + 1, j + 1] == 1)
                {
                    virusArray[i + 1, j + 1] = 0;
                    doctorArray[i + 1, j + 1] = 1;
                    doctorArray[i, j] = 0;
                }
            }
        }
    }

    //thieves will beat a policeman
    private void BeatDoctors()
    {
        for (int i = 1; i < gridHeight - 1; i++)
        {
            for (int j = 1; j < gridWidth - 1; j++)
            {
                if (doctorArray[i, j] == 1)
                {
                    if (virusArray[i - 1, j] == 1 && virusArray[i + 1, j] == 1 && virusArray[i, j - 1] == 1 && virusArray[i, j + 1] == 1)
                    {
                        doctorArray[i, j] = 0;
                        continue;
                    }

                    if (virusArray[i - 1, j - 1] == 1 && virusArray[i - 1, j + 1] == 1 && virusArray[i + 1, j - 1] == 1 && virusArray[i + 1, j + 1] == 1)
                    {
                        doctorArray[i, j] = 0;
                        continue;
                    }

                    if (virusArray[i + 1, j - 1] == 1 && virusArray[i, j - 1] == 1 && virusArray[i - 1, j - 1] == 1 && virusArray[i - 1, j] == 1)
                    {
                        doctorArray[i, j] = 0;
                        continue;
                    }

                    if (virusArray[i - 1, j] == 1 && virusArray[i - 1, j + 1] == 1 && virusArray[i, j + 1] == 1 && virusArray[i + 1, j + 1] == 1)
                    {
                        doctorArray[i, j] = 0;
                        continue;
                    }

                    if (virusArray[i + 1, j - 1] == 1 && virusArray[i, j - 1] == 1 && virusArray[i - 1, j - 1] == 1 && virusArray[i + 1, j] == 1)
                    {
                        doctorArray[i, j] = 0;
                        continue;
                    }

                    if (virusArray[i + 1, j] == 1 && virusArray[i - 1, j + 1] == 1 && virusArray[i, j + 1] == 1 && virusArray[i + 1, j + 1] == 1)
                    {
                        doctorArray[i, j] = 0;
                        continue;
                    }

                    if (virusArray[i - 1, j - 1] == 1 && virusArray[i - 1, j] == 1 && virusArray[i - 1, j + 1] == 1 && virusArray[i, j + 1] == 1)
                    {
                        doctorArray[i, j] = 0;
                        continue;
                    }

                    if (virusArray[i - 1, j - 1] == 1 && virusArray[i - 1, j] == 1 && virusArray[i - 1, j + 1] == 1 && virusArray[i, j - 1] == 1)
                    {
                        doctorArray[i, j] = 0;
                        continue;
                    }

                    if (virusArray[i, j - 1] == 1 && virusArray[i + 1, j - 1] == 1 && virusArray[i + 1, j] == 1 && virusArray[i + 1, j + 1] == 1)
                    {
                        doctorArray[i, j] = 0;
                        continue;
                    }

                    if (virusArray[i, j + 1] == 1 && virusArray[i + 1, j - 1] == 1 && virusArray[i + 1, j] == 1 && virusArray[i + 1, j + 1] == 1)
                    {
                        doctorArray[i, j] = 0;
                    }
                }
            }
        }
    }

    private int CountLivingNeighbours(int i, int j, int[,] arr)
    {
        int result = 0;
        for (int iNeigh = i - 1; iNeigh < i + 2; iNeigh++)
        {
            for (int jNeigh = j - 1; jNeigh < j + 2; jNeigh++)
            {
                if (iNeigh == i && jNeigh == j) continue;
                try
                {
                    result += arr[iNeigh, jNeigh];
                }
                catch { }
            }
        }
        return result;
    }
}

