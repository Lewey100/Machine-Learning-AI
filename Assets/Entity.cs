using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{

    Brain testBrain;
    private List<Brain> brains;
    public float currentBrainFitness;
    public float bestFitness;
    private float currentTimer;
    private int checkPointsHit;

    public NNet neuralNet;

    public GA genAlg;
    public int checkpoints;
    public GameObject[] CPs;
    public Material normal;

    private Vector3 defaultpos;
    private Quaternion defaultrot;

    hit hit;

    public Text currentFitness;
    public Text bestFit;
    public Text currentGenom;
    public Text totalPop;
    public Text generation;


    // Use this for initialization
    void Start()
    {

        genAlg = new GA();
        int totalWeights = 5 * 8 + 8 * 2 + 8 + 2;
        genAlg.GenerateNewPopulation(15, totalWeights);
        currentBrainFitness = 0.0f;
        bestFitness = 0.0f;

        neuralNet = new NNet();
        neuralNet.CreateNet(1, 5, 8, 2);
        Genome genome = genAlg.GetNextGenome();
        neuralNet.FromGenome(genome, 5, 8, 2);

        testBrain = gameObject.GetComponent<Brain>();
        testBrain.Attach(neuralNet);

        hit = gameObject.GetComponent<hit>();

        defaultpos = transform.position;
        defaultrot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (testBrain.hasFailed)
        {
            if (genAlg.GetCurrentGenomeIndex() == 15 - 1) //If all the genomes have been used in this generation.
            {
                EvolveGenomes(); //Create new generation of genomes.
                return;
            }
            NextTestSubject(); //Begin a new test.
        }
        currentBrainFitness = testBrain.dist; //Current fitness is how far the car has travelled.
        if (currentBrainFitness > bestFitness)
        {
            bestFitness = currentBrainFitness; //Update the best fitness.
        }
        currentFitness.text = currentBrainFitness.ToString(); //Display data on screen.
        bestFit.text = bestFitness.ToString();
        currentGenom.text = genAlg.currentGenome.ToString();
        totalPop.text = genAlg.totalPopulation.ToString();
        generation.text = genAlg.generation.ToString();
    }

    public void NextTestSubject()
    {
        genAlg.SetGenomeFitness(currentBrainFitness, genAlg.GetCurrentGenomeIndex()); //Set genome's fitness before moving onto the next.
        currentBrainFitness = 0.0f; //reset fitness of new test subject.
        Genome genome = genAlg.GetNextGenome(); //Get next genome in generation

        neuralNet.FromGenome(genome, 5, 8, 2); //5 inputs, 8 neurons in hidden layer, 2 outputs

        transform.position = defaultpos; //Return car to original position
        transform.rotation = defaultrot;

        testBrain.Attach(neuralNet); //Attach the NNet to this subject
        testBrain.ClearFailure();    //Clear the fail checks

    }

    public void BreedNewPopulation()
    {
        genAlg.ClearPopulation();
        int totalweights = 5 * 8 + 8 * 2 + 8 + 2; //5*8 = amount of weights needed from input layer to hidden. 8*2 = weights needed from hidden to output. 8+2 bias weight needed on hidden and output layers.
        genAlg.GenerateNewPopulation(15, totalweights); //Generate new generation of genomes.
    }

    public void EvolveGenomes()
    {
        genAlg.BreedPopulation();
        NextTestSubject();
    }

    public int GetCurrentMemberOfPopulation()
    {
        return genAlg.GetCurrentGenomeIndex();
    }

  

}
