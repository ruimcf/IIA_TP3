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
        string angulos = "";

        foreach(float x in angles)
        {
            angulos += "| " + x;
        }
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
            if(y >= MaxY)
            {
                y = MaxY-0.0001f;
            }
            else if (y <= MinY)
            {
                y = MinY+0.0001f;
            }

            trackPoints.Add(info.startPointX + (i+1) * step, y);
            last_y = y;
        }
        trackPoints.Add(info.endPointX, info.endPointY); //endpoint
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
        newobj.angles = new List<float>(this.angles);
        return newobj;
    }


    //cria uma nova linha, totalmente random acho eu, com limites MaxY e MinY
    void AngleInitialization()
    {
        angles = new List<float>();
        float y = 0;

        float minimum_angle, maximum_angle;
        float step = (info.endPointX - info.startPointX) / (info.numTrackPoints - 1);
        float alpha = Mathf.Atan(step / (MaxY - MinY)) * Mathf.Rad2Deg;
        minimum_angle = alpha;
        maximum_angle = 180 - alpha;

        for (int i = 0; i < info.numTrackPoints-2; i++)
        {
            y = NormalizedRandom(minimum_angle, maximum_angle);
            angles.Add(y);
        }
    }


    void GaussianMutation(float probability)
    {
        for(int i = 0; i < angles.Count; i++)
        {
            if (UnityEngine.Random.Range(0f, 1f) < probability)
            {
                float minimum_angle, maximum_angle;
                float step = (info.endPointX - info.startPointX) / (info.numTrackPoints - 1);
                float alpha = Mathf.Atan(step / (MaxY - MinY)) * Mathf.Rad2Deg;
                minimum_angle = alpha;
                maximum_angle = 180 - alpha;

                float mean = angles[i];
                float sigma = (maximum_angle - mean) / 3;
                float value = (NextGaussianDouble() * sigma + mean);
                if (value < minimum_angle)
                {
                    value = minimum_angle;
                }
                else if (value > maximum_angle)
                {
                    value = maximum_angle;
                }

                angles[i] = value;
            }
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
        mean -= Mathf.Rad2Deg * Mathf.Atan((info.startPointY - info.endPointY) / (info.endPointX - info.startPointX));
        //Debug.Log(Mathf.Rad2Deg * Mathf.Atan((info.startPointY - info.endPointY) / (info.endPointX - info.startPointX)));


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