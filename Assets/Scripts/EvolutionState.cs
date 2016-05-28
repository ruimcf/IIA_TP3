using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EvolutionState : MonoBehaviour {

	public float startPointX;
	public float startPointY;
	public float endPointX;
	public float endPointY;
	public float g;
	public float startVelocity;
	public int numTrackPoints;
	private ProblemInfo info;

    public int tournament_size;
    public float tournament_prob;

    public int IndividualType;
    public int SelectionType;
    public int MutationType;
    public int CrossoverType;

	public int numGenerations;
	public int populationSize;
	public float mutationProbability;
	public float crossoverProbability;

	public int numberOfCrossoverPoints;
    public int ElitismNumber;
	private List<Individual> population;
	private SelectionMethod randomSelection;

	private int evaluatedIndividuals;
	private int currentGeneration;
	public int EvaluationsPerStep;


	public string statsFilename;

	private StatisticsLogger stats;
	private PolygonGenerator drawer;

    public int NumTestes;
    private int contador=0;



	bool evolving;
	bool drawing;
    

	// Use this for initialization
	void Start () {
		info = new ProblemInfo ();
		info.startPointX = startPointX;
		info.startPointY = startPointY;
		info.endPointX = endPointX;
		info.endPointY = endPointY;
		info.g = g;
		info.startVelocity = startVelocity;
		info.numTrackPoints = numTrackPoints;

        switch (SelectionType)
        {
            case 1:
                randomSelection = new RandomSelection();
                break;
            case 2:
                randomSelection = new TournamentSelection(tournament_size, tournament_prob);
                break;
            case 3:
                randomSelection = new RouletteSelection();
                break;
            default:
                randomSelection = null;
                throw new System.Exception("Inserir Seletion Type valido:\t1-Random\t2-Tournament\t3-Roulette");
        }
       
        //iniciar as estatisticas
		if(contador == 0)
        {
            Debug.Log("A inicializar estatisticas");
            stats = new StatisticsLogger(statsFilename, numGenerations);
            stats.setNumTest(NumTestes);
        }

		drawer = new PolygonGenerator ();

		InitPopulation ();
		evaluatedIndividuals = 0;
		currentGeneration = 0;
		evolving = true;
		drawing = false;
	}
	

	void FixedUpdate () {
		if (evolving) {
			EvolStep ();
        } else if(drawing) {
            float min=100000;
            int index=0;
            for(int i =0; i < population.Count; i++)
            {
                population[i].evaluate();
                if (population[i].fitness < min)
                {
                    min = population[i].fitness;
                    index = i;
                }
            }
			//population.Sort((x, y) => x.fitness.CompareTo(y.fitness));
			drawer.drawCurve(population[index].trackPoints,info);
			drawing=false;

            contador++;
            if(contador >= NumTestes)
            {
                stats.finalLog();
                GameObject.Find("New Game Object").GetComponent<Renderer>().sharedMaterial.SetFloat("_Mode", 3f);
                Debug.Log("Testing done.");
            }
            else
            {
                Start();
            }
        }
	}

	void EvolStep() {
		//do for a given number of generations
		if (currentGeneration < numGenerations) {
			
			//if there are individuals to evaluate on the current generation
			int evalsThisStep = EvaluationsPerStep < (populationSize - evaluatedIndividuals) ? EvaluationsPerStep : (populationSize - evaluatedIndividuals);
			for (int ind = evaluatedIndividuals; ind<evaluatedIndividuals+evalsThisStep; ind++) {
				population[ind].evaluate();
			}
			evaluatedIndividuals += evalsThisStep;
			
			//if all individuals have been evaluated on the current generation, breed a new population
			if(evaluatedIndividuals==populationSize) {
                population.Sort((x, y) => x.fitness.CompareTo(y.fitness));
                stats.PostGenLog(population,currentGeneration);
				
				population = BreedPopulation();
				evaluatedIndividuals=0;
				currentGeneration++;
                for (int i = 0; i < info.numTrackPoints; i++)
                {
                    if (population[0].trackPoints.ContainsKey(i) && population[0].trackPoints.ContainsKey(i + 1))
                        Debug.DrawLine(new Vector2(i, population[0].trackPoints[i]), new Vector2(i + 1, population[0].trackPoints[i + 1]));
                }
            }

			
		} else {
			evolving=false;
			drawing = true;
			print ("Test "+contador+" done. Evolution stopped");
		}
		
		

	
	}

    List<Individual> Elitism()
    {
        List<Individual> best_individual = new List<Individual>();
        population.Sort((x, y) => x.fitness.CompareTo(y.fitness));
        for(int i = 0; i < ElitismNumber; i++)
        {
            best_individual.Add(population[i].Clone());
        }

        return best_individual;
    }


	void InitPopulation () {
		population = new List<Individual>();
		while (population.Count<populationSize) {
            Individual newind = null;
            switch (IndividualType)
            {
                case 1:
                    newind = new ExampleIndividual(info);
                    break;
                case 2:
                    newind = new AngleIndividual(info);
                    break;
                default:
                    throw new System.Exception("Inserir Individual Type valido:\t1-Example Individual\t2-Angle Individual");
            }
			 //change accordingly
			newind.Initialize();
			population.Add (newind);
		}
	}


	List<Individual> BreedPopulation() {
		List<Individual> newpop = new List<Individual>();

        //vai buscar os X melhores elementos da pop atual para a nova
        newpop = Elitism();

        //breed individuals and place them on new population. We'll apply crossover and mutation later 
        while (newpop.Count<populationSize) {
			List<Individual> selectedInds = randomSelection.selectIndividuals(population,2); //we should propably always select pairs of individuals
			for(int i =0; i< selectedInds.Count;i++) {
				if(newpop.Count<populationSize) {
					newpop.Add(selectedInds[i]); //added individuals are already copys, so we can apply crossover and mutation directly
				}
				else{ //discard any excess individuals
					selectedInds.RemoveAt(i);	
				}
			}

			//apply crossover between pairs of individuals and mutation to each one
			while(selectedInds.Count>1) {
				selectedInds[0].Crossover(selectedInds[1],crossoverProbability,numberOfCrossoverPoints, CrossoverType);
				selectedInds[0].Mutate(mutationProbability,MutationType);
				selectedInds[1].Mutate(mutationProbability,MutationType);
				selectedInds.RemoveRange(0,2);
			}
			if(selectedInds.Count==1) {
				selectedInds[0].Mutate(mutationProbability,MutationType);
				selectedInds.RemoveAt(0);
			}
		}

		return newpop;
	}



}


