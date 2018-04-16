using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GA //Genetic Algorithm
{

    public int currentGenome;
    public int totalPopulation;
    private int genomeID;
    public int generation;
    private int totalGenomeWeights;

    public float MUTATION_RATE;
    public float MAX_PERBETUATION;

    public List<Genome> population = new List<Genome>();
    public List<int> crossoverSplits = new List<int>();

    public GA()
    {
        this.currentGenome = -1;
        this.totalPopulation = 0;
        genomeID = 0;
        generation = 1;
        MUTATION_RATE = 0.15f; //Probability of a neuron being mutated
        MAX_PERBETUATION = 0.3f; //Max amount of mutation that can happen on a single neuron evolution
    }

    public Genome GetNextGenome()
    {
        currentGenome++;
        if (currentGenome >= population.Count)
            return null;

        return population[this.currentGenome];
    }

    public Genome GetBestGenome()
    {
        int bestGenome = -1;
        float fitness = 0;
        for (int i = 0; i < population.Count; i++)
        {
            if (population[i].fitness > fitness)
            {
                fitness = population[i].fitness; //Get the genome with the highest fitness value
                bestGenome = i;
            }
        }

        return population[bestGenome];
    }

    public Genome GetWorstGenome()
    {
        int worstGenome = -1;
        float fitness = 1000000.0f;
        for (int i = 0; i < population.Count; i++)
        {
            if (population[i].fitness < fitness)
            {
                fitness = population[i].fitness; //Get the genome with the worst fitness value.
                worstGenome = i;
            }
        }

        return population[worstGenome];
    }

    public Genome GetGenome(int index) //Get the genome at index.
    {
        if (index >= totalPopulation)
            return null;
        return population[index];
    }

    public int GetCurrentGenomeIndex()
    {
        return currentGenome;
    }

    public int GetCurrentGenomeID()
    {
        return population[currentGenome].ID;
    }

    public int GetCurrentGeneration()
    {
        return generation;
    }

    public int GetTotalPopulation()
    {
        return totalPopulation;
    }


    public void GetBestCases(int totalGenomes, ref List<Genome> output)
    {
        int genomeCount = 0;
        int runCount = 0;

        while (genomeCount < totalGenomes)
        {
            if (runCount > 10)
                return;

            runCount++;

            //Find the best cases for cross breeding based on fitness score
            float bestFitness = 0;
            int bestIndex = -1;
            for (int i = 0; i < this.totalPopulation; i++)
            {
                if (population[i].fitness > bestFitness)
                {
                    bool isUsed = false;

                    for (int j = 0; j < output.Count; j++)
                    {
                        if (output[j].ID == population[i].ID)
                        {
                            isUsed = true; //Block a genome being used more than once during best case selection
                        }
                    }

                    if (isUsed == false)
                    {
                        bestIndex = i;
                        bestFitness = population[bestIndex].fitness; //Set the best genome based on fitness
                    }
                }
            }

            if (bestIndex != -1)
            {
                genomeCount++;
                output.Add(population[bestIndex]); //Add the best genome to the output layer to be used by the next iteration
            }
        }
    }

    public void CrossBreed(Genome g1, Genome g2, ref Genome baby1, ref Genome baby2) //Gives the parent genome's weight values to it's babies
    {
        int totalWeights = g1.weights.Count;

        int crossover = (int)Random.Range(0, totalWeights - 1);

        baby1 = new Genome();
        baby1.ID = genomeID;
        baby1.weights = new List<float>();
        //resize
        for (int i = 0; i < totalWeights; i++)
        {
            baby1.weights.Add(0.0f);
        }
        genomeID++;

        baby2 = new Genome();
        baby2.ID = genomeID;
        baby2.weights = new List<float>();
        //resize
        for (int i = 0; i < totalWeights; i++)
        {
            baby2.weights.Add(0.0f);
        }
        genomeID++;

        //Go from start to crossover point, copying the weights from parent genomes
        for (int i = 0; i < crossover; i++)
        {
            baby1.weights[i] = g1.weights[i];
            baby2.weights[i] = g2.weights[i];
        }

        for (int i = crossover; i < totalWeights; i++)
        {
            baby1.weights[i] = g2.weights[i];
            baby2.weights[i] = g1.weights[i];
        }
    }

    public Genome CreateNewGenome(int totalWeights)
    {
        Genome genome = new Genome();
        genome.ID = genomeID;
        genome.fitness = 0.0f;
        genome.weights = new List<float>();
        //resize
        for (int i = 0; i < totalWeights; i++)
        {
            genome.weights.Add(0.0f);
        }

        for (int j = 0; j < totalWeights; j++)
        {
            genome.weights[j] = RandomClamped(); //Give each new genome randomised weighting which will have more priority during mutation
        }

        genomeID++;
        return genome;
    }

    public void GenerateNewPopulation(int totalPop, int totalWeights)
    {
        generation = 1;
        ClearPopulation();
        currentGenome = -1;
        totalPopulation = totalPop;
        //resize
        if (population.Count < totalPop)
        {
            for (int i = population.Count; i < totalPop; i++) //Create a new generation of 15 genomes
            {
                population.Add(new Genome());
            }
        }

        for (int i = 0; i < population.Count; i++)
        {
            Genome genome = new Genome();
            genome.ID = genomeID;
            genome.fitness = 0.0f;
            genome.weights = new List<float>();
            //resize
            for (int k = 0; k < totalWeights; k++)
            {
                genome.weights.Add(RandomClamped());
            }

            genomeID++;
            population[i] = genome;
        }
    }

    public void BreedPopulation()
    {
        List<Genome> bestGenomes = new List<Genome>();

        //find the 4 best genomes
        this.GetBestCases(4, ref bestGenomes);

        //Breed them with each other twice to form 3*2 + 2*2 + 1*2 = 12 children
        List<Genome> children = new List<Genome>();

        //Carry on the best
        Genome best = new Genome();
        best.fitness = 0.0f;
        best.ID = bestGenomes[0].ID;
        best.weights = bestGenomes[0].weights;
        //Mutate(best);
        children.Add(best);

        //Child genomes
        Genome baby1 = new Genome();
        Genome baby2 = new Genome();

        // Breed with genome 0.
        CrossBreed(bestGenomes[0], bestGenomes[1], ref baby1, ref baby2);
        Mutate(baby1);
        Mutate(baby2);
        children.Add(baby1);
        children.Add(baby2);
        CrossBreed(bestGenomes[0], bestGenomes[2], ref baby1, ref baby2);
        Mutate(baby1);
        Mutate(baby2);
        children.Add(baby1);
        children.Add(baby2);
        CrossBreed(bestGenomes[0], bestGenomes[3], ref baby1, ref baby2);
        Mutate(baby1);
        Mutate(baby2);
        children.Add(baby1);
        children.Add(baby2);

        // Breed with genome 1.
        CrossBreed(bestGenomes[1], bestGenomes[2], ref baby1, ref baby2);
        Mutate(baby1);
        Mutate(baby2);
        children.Add(baby1);
        children.Add(baby2);
        CrossBreed(bestGenomes[1], bestGenomes[3], ref baby1, ref baby2);
        Mutate(baby1);
        Mutate(baby2);
        children.Add(baby1);
        children.Add(baby2);

        //For the remainder population, add some random children
        int remainingChildren = (totalPopulation - children.Count);
        for (int i = 0; i < remainingChildren; i++)
        {
            children.Add(this.CreateNewGenome(bestGenomes[0].weights.Count));
        }

        ClearPopulation();
        population = children; //New generation is created of children only. This way the best 4 genomes have their weightings carried through generations until a genome has the input values needed to not fail.

        currentGenome = 0;
        generation++;
    }

    public void ClearPopulation()
    {
        for (int i = 0; i < population.Count; i++)
        {
            if (population[i] != null)
            {
                population[i] = null;
            }
        }
        population.Clear();
    }

    public void Mutate(Genome genome)
    {
        for (int i = 0; i < genome.weights.Count; i++)
        {
            if (RandomClamped() < MUTATION_RATE) //15% chance of mutation
            {
                genome.weights[i] += (RandomClamped() * MAX_PERBETUATION); //add random value * max amount of mutation to the genome's weight 
            }
        }
    }

    public void SetGenomeFitness(float fitness, int index)
    {
        if (index >= population.Count)
        {
            return;
        }
        else
        {
            population[index].fitness = fitness;
        }
    }

    public float RandomFloat()
    {
        float rand = (float)Random.Range(0.0f, 32767.0f);
        return rand / 32767.0f/*32767*/ + 1.0f;
    }

    public float RandomClamped()
    {
        return RandomFloat() - RandomFloat();
    }
}
