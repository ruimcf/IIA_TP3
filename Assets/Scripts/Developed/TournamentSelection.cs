using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TournamentSelection : SelectionMethod {
    int tourn_size;
    float tourn_prob;

	public TournamentSelection(): base()
    {

    }

    public TournamentSelection(int tournament_size, float tournament_prob)
    {
        tourn_size = tournament_size;
        tourn_prob = tournament_prob;
    }

    public override List<Individual> selectIndividuals(List<Individual> oldpop, int num)
    {
        return tournamentSelection(oldpop, num);
    }

    List<Individual> tournamentSelection(List<Individual> oldpop, int num)
    {

        List<Individual> selectedInds = new List<Individual>();
        int popsize = oldpop.Count;
        for (int j = 0; j < num; j++)
        {

            //Escolher um grupo de tourn_size Individuous para o tourneio
            List<Individual> tournament = new List<Individual>();
            for(int i = 0; i < tourn_size; i++)
            {
                Individual ind = oldpop[Random.Range(0, popsize)];
                while (tournament.Contains(ind))
                {
                    ind = oldpop[Random.Range(0, popsize)];
                }
                tournament.Add(ind.Clone());
            }

            //ordenar a lista de elementos do torneio pelo seu time
            tournament.Sort(SortByEval);

            //escolher o individuo mais forte primeiro, caso nao ganhei, passa ao proximo com menos prob.
            for(int k =0 ; k <tourn_size;k++)
            {
                if (Random.Range(0, 1) < tourn_prob * Mathf.Pow((float)(1 - tourn_prob), k) || k == tourn_size - 1)
                {
                    selectedInds.Add(tournament[k].Clone());
                    break;
                }
            }

        }

        for (int i = 0; i < num; i++)
        {
            //make sure selected individuals are different
            Individual ind = oldpop[Random.Range(0, popsize)];
            while (selectedInds.Contains(ind))
            {
                ind = oldpop[Random.Range(0, popsize)];
            }
            selectedInds.Add(ind.Clone()); //we return copys of the selected individuals
        }

        return selectedInds;
    }


    static int SortByEval(Individual inv1, Individual inv2)
    {
        return inv1.fitness.CompareTo(inv2.fitness);
    }
}
