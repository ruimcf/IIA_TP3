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


	public StatisticsLogger(string name) {
		filename = name;
		bestFitness = new Dictionary<int,float> ();
		meanFitness = new Dictionary<int,float> ();
        worstFitness = new Dictionary<int,float>();
        desvioPadraoFitness = new Dictionary<int, float>();

    }

	//saves fitness info and writes to console
	public void PostGenLog(List<Individual> pop, int currentGen) {
		pop.Sort((x, y) => x.fitness.CompareTo(y.fitness));
	
		bestFitness.Add (currentGen, pop[0].fitness);
		meanFitness.Add (currentGen, 0f);
        worstFitness.Add( currentGen, pop[pop.Count-1].fitness);

        foreach (Individual ind in pop) {
			meanFitness[currentGen]+=ind.fitness;
		}
		meanFitness [currentGen] /= pop.Count;

        float media = meanFitness[currentGen];
        float acum = 0;
        foreach (Individual ind in pop)
        {
            acum += Mathf.Pow(ind.fitness-media, 2);
        }
        desvioPadraoFitness[currentGen] = Mathf.Sqrt(acum / (pop.Count-1));


        Debug.Log ("generation: "+currentGen+"\tbest: " + bestFitness [currentGen] + "\tmean: " + meanFitness [currentGen]+"\tworst: "+worstFitness[currentGen]+"\tdesvio padrao: "+desvioPadraoFitness[currentGen]+"\n");
	}

	//writes to file
	public void finalLog() {
		logger = File.CreateText (filename);

		//writes with the following format: generation, bestfitness, meanfitness
		for (int i=0; i<bestFitness.Count; i++) {
			logger.WriteLine(i+" "+bestFitness[i]+" "+meanFitness[i] + " " + worstFitness[i]+" " + desvioPadraoFitness[i]);
		}

		logger.Close ();
	}
}
