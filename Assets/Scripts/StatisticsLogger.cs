using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class StatisticsLogger {
	
	public Dictionary<int,float> bestFitness;
	public Dictionary<int,float> meanFitness;
    public Dictionary<int,float> worstFitness;
    public Dictionary<int, float> desvioPadraoFitness;

    private string filename;
	private StreamWriter logger;
    private int num_gen;
    private int num_test;




    public void setNumTest(int num)
    {
        num_test = num;
    }

	public StatisticsLogger(string name, int num_gene) {
		filename = name;
		bestFitness = new Dictionary<int,float> ();
		meanFitness = new Dictionary<int,float> ();
        worstFitness = new Dictionary<int,float>();
        desvioPadraoFitness = new Dictionary<int, float>();
        this.num_gen = num_gene;
        for(int i =0; i < num_gen; i++)
        {
            bestFitness[i] = 0;
            meanFitness[i] = 0;
            worstFitness[i] = 0;
            desvioPadraoFitness[i] = 0;
        }

    }

	//saves fitness info and writes to console
	public void PostGenLog(List<Individual> pop, int currentGen) {
		pop.Sort((x, y) => x.fitness.CompareTo(y.fitness));

        float media = 0f;
	
		bestFitness[currentGen] += pop[0].fitness;
        worstFitness[currentGen] += pop[pop.Count-1].fitness;

        foreach (Individual ind in pop) {
			media+=ind.fitness;
		}
		media /= pop.Count;
        meanFitness[currentGen] += media;
        
        float acum = 0;
        foreach (Individual ind in pop)
        {
            acum += Mathf.Pow(ind.fitness-media, 2);
        }
        desvioPadraoFitness[currentGen] += Mathf.Sqrt(acum / (pop.Count-1));

        //ja nao vamos ter estatisticas do teste currente
        //Debug.Log ("generation: "+currentGen+"\tbest: " + bestFitness [currentGen] + "\tmean: " + meanFitness [currentGen]+"\tworst: "+worstFitness[currentGen]+"\tdesvio padrao: "+desvioPadraoFitness[currentGen]+"\n");
	}

	//writes to file
	public void finalLog() {
		logger = File.CreateText (filename);

		//writes with the following format: generation, bestfitness, meanfitness
		for (int i=0; i<bestFitness.Count; i++) {
            bestFitness[i] /= num_test;
            meanFitness[i] /= num_test;
            worstFitness[i] /= num_test;
            desvioPadraoFitness[i] /= num_test;
            logger.WriteLine(i+"\t"+bestFitness[i]+"\t"+meanFitness[i] + "\t" + worstFitness[i]+"\t" + desvioPadraoFitness[i]);
		}

		logger.Close ();
	}
}
