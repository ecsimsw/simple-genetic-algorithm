using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GA : MonoBehaviour
{
   public float mutatePR=0.5f;

    Car[] cars;
    int[] childCarsValue;

    int geneCount = 10;
    float[] fitness;
    int[] selected;

    int generationCount = 0;
    int MAXIMUM_VALUE = 0xFFFFFF;

    public Text text_Fitness;
    public Text text_generation;

    // Start is called before the first frame update
    void Start()
    {
        cars = new Car[geneCount];

        for(int i=0; i<geneCount; i++)
        {
            cars[i] = transform.GetChild(i).gameObject.GetComponent<Car>();
        }

        fitness = new float[geneCount];
        selected = new int[geneCount];
        childCarsValue = new int[geneCount];

        for (int i = 0; i < geneCount; i++)
        {
            cars[i].value = Random.Range(0, MAXIMUM_VALUE+1);
            cars[i].Initialize();
        }

        text_Fitness.text = "Fitness : 00.00";
        text_generation.text = "Generation : 0";
    }

    // Update is called once per frame
    void Update()
    {

        if (IsAllDead())
        {
            CalculateFitness(); // fitness is definated (100-record)*1000 

            Selection();

            CrossOver();

            Mutate(mutatePR);// mutatePB : 0.5% means picking 1 in 200 total sample 

            NextGeneration();

            text_Fitness.text = "Fitness : " + getBiggestFitness();
            text_generation.text="Generation : "+generationCount++;
        }
    }

    bool IsAllDead()
    {
        bool result = true;

        foreach(Car c in cars)
        {
            result &= c.dead;
        }

        return result;
    }
    
    void CalculateFitness()
    {
        for (int i = 0; i < geneCount; i++)
        {
            fitness[i] = cars[i].record;
        }
    }
    
    void Selection()
    {
       for(int i=0; i<geneCount; i++)
       {
         float rand_roulette = Random.Range(0, 100);
         selected[i] = RouletteWheelTable(rand_roulette);
       }
    }

    int RouletteWheelTable(float rand)
    {
        float fitness_sum = 0;

        foreach (float f in fitness)
        {
            fitness_sum += f;
        }

        for (int i=0; i<geneCount; i++)
        {
            float PR = (fitness[i] / fitness_sum) * 100;
            if (PR<= rand)
            {
                rand -= PR;
            }

            else
            {
                return i;
            }
        }

        return -1;
    }

    void CrossOver()
    {
        int rand_selectedIndex1;
        int rand_selectedIndex2;
        FileStream FS = new FileStream(@"C:\Users\luraw\OneDrive\바탕 화면\crossOverData.txt", FileMode.Open, FileAccess.Write);
        StreamWriter wr = new StreamWriter(FS);

        rand_selectedIndex1 = Random.Range(0, geneCount);
        rand_selectedIndex2 = Random.Range(0, geneCount);

        childCarsValue[0] = cars[selected[rand_selectedIndex1]].value;   // to save two of parents
        childCarsValue[1] = cars[selected[rand_selectedIndex2]].value;

        wr.WriteLine("selected[" + rand_selectedIndex1 + "]");
        wr.WriteLine("childvalue" + 0 + "  :  " + cars[selected[rand_selectedIndex1]].value.ToString("x4"));
        wr.WriteLine("selected[" + rand_selectedIndex2 + "]");
        wr.WriteLine("childvalue" + 1 + "  :  " + cars[selected[rand_selectedIndex2]].value.ToString("x4"));
       
        for (int i = 2; i < geneCount; i++)
        {
            rand_selectedIndex1 = Random.Range(0, geneCount);
            rand_selectedIndex2 = Random.Range(0, geneCount);

            int cv = (cars[selected[rand_selectedIndex1]].value & 0xCF0F0C) + (cars[selected[rand_selectedIndex2]].value & 0x30F0F3);

            childCarsValue[i] = cv;

            wr.WriteLine("selected[" + rand_selectedIndex1 + "] crossOver "+"selected["+ rand_selectedIndex2+"]");
             wr.WriteLine("childvalue" + i + "  :  " + cv.ToString("x4"));

        }
        wr.Close();
        FS.Close();
        showingCrossOverData();
    }

    void Mutate(float mutatePR)
    {
        for(int i=0; i<geneCount; i++)
        {
            float mutateRand = Random.Range(0, 100.0f);
            if (mutateRand < mutatePR) 
            {
                Debug.Log("MUTATE");
                int mutate= Random.Range(0, 0xFFFFFF+1);

                childCarsValue[i] = mutate;
            }
        }
    }

    void NextGeneration()
    {
        showingGeneData();

        for (int i = 0; i < geneCount; i++)
        {
            cars[i].value = childCarsValue[i];
            cars[i].Initialize();
        }
    }

    float getBiggestFitness()
    {
        float max=fitness[0];

        foreach(float f in fitness)
        {
            if(f > max)
            {
                max = f;
            }
        }

        return max;
    }

    void showingCrossOverData()
    {
        using (StreamWriter wr = new StreamWriter(@"C:\Users\luraw\OneDrive\바탕 화면\selectedData.txt"))
        {
            for (int i = 0; i < geneCount; i++)
            {
                wr.WriteLine("car["+i+"] fitness : " + fitness[i]);
            }

            for (int i = 0; i < geneCount; i++)
            {
                wr.WriteLine("selected cars ["+i+"] : " + selected[i] + " || "+cars[selected[i]].value.ToString("x4"));
            }
        }
    }

    void showingGeneData()
    {
        using (StreamWriter wr = new StreamWriter(@"C:\Users\luraw\OneDrive\바탕 화면\geneData.txt"))
        {
            for (int i = 0; i < geneCount; i++)
            {
                wr.WriteLine(i + " geneData: " + cars[i].value.ToString("x4") +" -> "+childCarsValue[i].ToString("x4"));
            }

       
        }
    }
}
