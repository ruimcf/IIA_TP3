using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class AngleIndividual : Individual {


    private float MinX;
    private float MaxX;
    private float MinY;
    private float MaxY;

    public List<float> angles;

    public AngleIndividual(ProblemInfo info) : base(info) {

        MinX = info.startPointX;
        MaxX = info.endPointX;
        MaxY = info.startPointY > info.endPointY ? info.startPointY : info.endPointY;

        MinY = MaxY - 2 * (Mathf.Abs(info.startPointY - info.endPointY));
    }

    public override void Initialize()
    {
        AngleInitialization();
    }

    public override void Mutate(float probability)
    {
        NewValueMutation(probability);
    }

    public override void Crossover(Individual partner, float probability, int nPoints)
    {
        if (nPoints > 2)
            N_PointCrossover(partner, probability, nPoints);
        else
            HalfCrossover((AngleIndividual)partner, probability);
    }

    public override void CalcTrackPoints()
    {
        float last_y, step, y;
        step = (info.endPointX - info.startPointX) / (info.numTrackPoints - 1);
        trackPoints.Clear();
        trackPoints.Add(info.startPointX, info.startPointY);//startpoint
        last_y = info.startPointY;
        for (int i = 0; i < angles.Count; i++)
        {
            y = step / Mathf.Tan(angles[i] * Mathf.Deg2Rad);
            y = last_y - y;

            //proteçao
            if(y > MaxY)
            {
                y = MaxY;
            }
            else if (y < MinY)
            {
                y = MinY;
            }

            trackPoints.Add(info.startPointX + (i+1) * step, y);
            last_y = y;
        }
        trackPoints.Add(info.endPointX, info.endPointY); //endpoint
        //the representation used in the example individual is a list of trackpoints, no need to convert
    }

    public override void CalcFitness()
    {
        fitness = eval.time; //in this case we only consider time
    }


    public override Individual Clone()
    {
        AngleIndividual newobj = (AngleIndividual)this.MemberwiseClone();
        newobj.fitness = 0f;
        newobj.trackPoints = new Dictionary<float, float>(this.trackPoints);
        return newobj;
    }


    //cria uma nova linha, totalmente random acho eu, com limites MaxY e MinY
    void AngleInitialization()
    {
        angles = new List<float>();
        float y = 0;

        for (int i = 0; i < info.numTrackPoints - 3; i++)
        {
            y = NormalizedRandom(0, 180);
            angles.Add(y);
        }
    }

    void NewValueMutation(float probability)
    {
        int i = 0;
        foreach (float x in angles)
        {
            if (UnityEngine.Random.Range(0f, 1f) < probability)
            {
                angles[i] = NormalizedRandom(0, 180);
            }
            i++;
        }
    }
    //recombinaçao de metade entre dois individuos
    void HalfCrossover(AngleIndividual partner, float probability)
    {

        if (UnityEngine.Random.Range(0f, 1f) > probability)
        {
            return;
        }
        //this example always splits the chromosome in half
        int crossoverPoint = Mathf.FloorToInt(angles.Count / 2f);
        for (int i = 0; i < crossoverPoint; i++)
        {
            float tmp = angles[i];
            angles[i] = partner.angles[i];
            partner.angles[i] = tmp;
        }

    }


    void N_PointCrossover(Individual partner, float probability, int nPoints)
    {
        if (UnityEngine.Random.Range(0f, 1f) > probability)
        {
            return;
        }
        List<float> keys = new List<float>(trackPoints.Keys);
        for (int i = 0; i < info.numTrackPoints; i += (nPoints * 2))
        {
            for (int j = 0; j < nPoints && j < info.numTrackPoints; j++)
            {
                float tmp = trackPoints[keys[i + j]];
                trackPoints[keys[i + j]] = partner.trackPoints[keys[i + j]];
                partner.trackPoints[keys[i + j]] = tmp;
            }
        }
    }
    /*http://answers.unity3d.com/questions/421968/normal-distribution-random.html */
    public float NormalizedRandom(float minimum, float maximum)
    {
        float mean = (maximum + minimum) / 2;
        float sigma = (maximum - mean) / 3;
        float value = (NextGaussianDouble() * sigma + mean);
        if(value < minimum)
        {
            return minimum;
        }
        else if(value > maximum)
        {
            return maximum;
        }
        else
        {
            return value;
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