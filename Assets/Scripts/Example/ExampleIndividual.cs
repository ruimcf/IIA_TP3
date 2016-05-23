using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ExampleIndividual : Individual {


	private float MinX;
	private float MaxX;
	private float MinY;
	private float MaxY;

	public ExampleIndividual(ProblemInfo info) : base(info) {

		MinX = info.startPointX;
		MaxX = info.endPointX;
		MaxY = info.startPointY > info.endPointY ? info.startPointY : info.endPointY;

		MinY = MaxY - 2 * (Mathf.Abs (info.startPointY - info.endPointY));
	}

	public override void Initialize() {
		RandomInitialization();
	}

	public override void Mutate(float probability, int MutationType) {
        switch (MutationType)
        {
            case 1:
                NewValueMutation(probability);
                break;
            case 2:
                GaussianMutation(probability);
                break;
            default:
                throw new System.Exception("Inserir Mutation Type valido:\t1-Random\t2-Gaussian Mutation");
        }
    }

	public override void Crossover(Individual partner, float probability,int nPoints) {
		if(nPoints>2)
			N_PointCrossover(partner,probability,nPoints);
		else
			HalfCrossover (partner, probability);
	}

	public override void CalcTrackPoints() {
		//the representation used in the example individual is a list of trackpoints, no need to convert
	}

	public override void CalcFitness() {
		fitness = eval.time; //in this case we only consider time
	}


	public override Individual Clone() {
		ExampleIndividual newobj = (ExampleIndividual)this.MemberwiseClone ();
		newobj.fitness = 0f;
		newobj.trackPoints = new Dictionary<float,float> (this.trackPoints);
		return newobj;
	}


	//cria uma nova linha, totalmente random acho eu, com limites MaxY e MinY
	void RandomInitialization() {
		float step = (info.endPointX - info.startPointX ) / (info.numTrackPoints - 1);
		float y = 0;

		trackPoints.Add (info.startPointX, info.startPointY);//startpoint
		for(int i = 1; i < info.numTrackPoints - 1; i++) {
			y = UnityEngine.Random.Range(MinY, MaxY);
			trackPoints.Add(info.startPointX + i * step, y);
		}
		trackPoints.Add (info.endPointX, info.endPointY); //endpoint
	}

	void NewValueMutation(float probability) {
		List<float> keys = new List<float>(trackPoints.Keys);
		foreach (float x in keys) {
			//make sure that the startpoint and the endpoint are not mutated 
			if(Math.Abs (x-info.startPointX)<0.01 || Math.Abs (x-info.endPointX)<0.01) {
				continue;
			}
			if(UnityEngine.Random.Range (0f, 1f) < probability) {
				trackPoints[x] = UnityEngine.Random.Range(MinY,MaxY);
			}
		}
	}

    void GaussianMutation(float probability)
    {
        List<float> keys = new List<float>(trackPoints.Keys);
        foreach (float x in keys)
        {
            //make sure that the startpoint and the endpoint are not mutated 
            if (Math.Abs(x - info.startPointX) < 0.01 || Math.Abs(x - info.endPointX) < 0.01)
            {
                continue;
            }
            if (UnityEngine.Random.Range(0f, 1f) < probability)
            {
                float mean = trackPoints[x];

                float sigma = (MaxY - mean) / 3;
                float value = (NextGaussianDouble() * sigma + mean);
                if (value < MinY)
                {
                    value = MinY;
                }
                else if (value > MaxY)
                {
                    value = MaxY;
                }

                trackPoints[x] = value;
            }
        }
    }

    //recombinaçao de metade entre dois individuos
    void HalfCrossover(Individual partner, float probability) {

		if (UnityEngine.Random.Range (0f, 1f) > probability) {
			return;
		}
		//this example always splits the chromosome in half
		int crossoverPoint = Mathf.FloorToInt (info.numTrackPoints / 2f);
		List<float> keys = new List<float>(trackPoints.Keys);
		for (int i=0; i<crossoverPoint; i++) {
			float tmp = trackPoints[keys[i]];
			trackPoints[keys[i]] = partner.trackPoints[keys[i]];
			partner.trackPoints[keys[i]]=tmp;
		}

	}


	void N_PointCrossover(Individual partner, float probability,int nPoints){
		if (UnityEngine.Random.Range (0f, 1f) > probability) {
			return;
		}
		List<float> keys = new List<float>(trackPoints.Keys);
		for(int i=0;i<info.numTrackPoints;i+=(nPoints*2)){
			for(int j=0;j<nPoints && j<info.numTrackPoints;j++){
				float tmp = trackPoints[keys[i+j]];
				trackPoints[keys[i+j]] = partner.trackPoints[keys[i+j]];
				partner.trackPoints[keys[i+j]]=tmp;
			}
		}
	}

    public float NextGaussianDouble()
    {
        double u, v, S;

        do
        {
            u = 2.0 * UnityEngine.Random.value - 1.0;
            v = 2.0 * UnityEngine.Random.value - 1.0;
            S = u * u + v * v;
        }
        while (S >= 1.0);

        double fac = Math.Sqrt(-2.0 * Math.Log(S) / S);
        return (float)(u * fac);
    }

}
