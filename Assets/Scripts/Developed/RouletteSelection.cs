using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RouletteSelection : SelectionMethod {

    public RouletteSelection(): base()
    {

    }

    public override List<Individual> selectIndividuals(List<Individual> oldpop, int num)
    {
        return rouletteSelection(oldpop, num);
    }
    
    List<Individual> rouletteSelection(List<Individual> oldpop,int num){
        float t =0;
        List<Individual> selectedInds = new List<Individual>();

        for(int i=0;i<oldpop.Count;i++){
            t+=oldpop[i].fitness;
        }

        for(int i=0;i<num;i++){
            float r = Random.Range(0, t);
            float s = 0;
            for(int j=0;j<oldpop.Count;j++){
                s+=oldpop[i].fitness;
                if(s>=r && !selectedInds.Contains(oldpop[j])){
                    selectedInds.Add(oldpop[j].Clone());
                    break;
                }
            }

        }
        return selectedInds;
    }
}
