using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class singleNeuron
{

    public int numInputs;
    public List<double> vecWeights = new List<double>();

    public singleNeuron(int f_NumInputs)
    {
        for (int i = 0; i < f_NumInputs + 1; i++)
        {
            vecWeights.Add(Random.Range(-1f, 1f));
        }
        numInputs = f_NumInputs;
    }
}

public class NeuralNetworkLayer
{
    public int nodeCount;
    public int childNodeCount;
    public int parentNodeCount;

    public NeuralNetworkLayer parentLayer;
    public NeuralNetworkLayer childLayer;

    public float learningRate;
    public bool linearOutput = false;
    public bool useMomentum = false;
    public float momentumFactor = 0.9f;

    public float[][] weights;
    public float[][] weightChanges;
    public float[] neuronValues;
    public float[] desiredValues;
    public float[] errors;
    public float[] biasWeights;
    public float[] biasValues;

    public void Initialize(int NumNodes, NeuralNetworkLayer parent, NeuralNetworkLayer child) {

        int i, j;
        // Allocate memory
        neuronValues = new float[nodeCount];
        desiredValues = new float[nodeCount];
        errors = new float[nodeCount];

        if (parent != null)
        {
            parentLayer = parent;
        }
        if (child != null)
        {
            childLayer = child;
            weights = new float[nodeCount][];
            weightChanges = new float[nodeCount][];

            for (i = 0; i < nodeCount; i++)
            {
                weights[i] = new float[nodeCount];
                weightChanges[i] = new float[nodeCount];
            }

            biasValues = new float[nodeCount];
            biasWeights = new float[nodeCount];
        }
        else
        {
            weights = null;
            weightChanges = null;
            biasValues = null;
            biasWeights = null;
        }
        // Make sure everything contains 0s
        for (i = 0; i < nodeCount; i++)
        {
            neuronValues[i] = 0;
            desiredValues[i] = 0;
            errors[i] = 0;
            if (childLayer != null)
                for (j = 0; j < childNodeCount; j++)
                {
                    weights[i][j] = 0;
                    weightChanges[i][j] = 0;
                }
        }
        // Initialize the bias values and weights
        if (childLayer != null)
            for (j = 0; j < childNodeCount; j++)
            {
                biasValues[j] = -1;
                biasWeights[j] = 0;
            }
    }

    public void RandomizeWeights()
    {
        int i, j;
        int min = 0;
        int max = 200;
        int number;

        //srand((unsigned)time(NULL));
        for (i = 0; i < nodeCount; i++)
        {
            for (j = 0; j < childNodeCount; j++)
            {
                number = Random.Range(min, max);
                weights[i][j] = number / 100.0f - 1;
            }
        }
        for (j = 0; j < childNodeCount; j++)
        {
            number = Random.Range(min, max);
            biasWeights[j] = number / 100.0f - 1;
        }
    }

    public void CalculateNeuronValues()
    {
        int i, j;
        float x;
        if (parentLayer != null)
        {
            for (j = 0; j < nodeCount; j++)
            {
                x = 0;
                for (i = 0; i < parentNodeCount; i++)
                {
                    x += parentLayer.neuronValues[i] * parentLayer.weights[i][j];
                }
                x += parentLayer.biasValues[j] * parentLayer.biasWeights[j];

                if ((childLayer == null) && linearOutput)
                    neuronValues[j] = x;
                else
                    neuronValues[j] = 1.0f / (1 + Mathf.Exp(-x));
            }
        }
    }

    public void CalculateErrors()
    {
        int i, j;
        float sum;

        if (childLayer == null) // output layer
        {
            for (i = 0; i < nodeCount; i++)
            {
                errors[i] = (desiredValues[i] - neuronValues[i]) *
                neuronValues[i] * (1.0f - neuronValues[i]);
            }
        }
        else if (parentLayer == null)
        { // input layer
            for (i = 0; i < nodeCount; i++)
            {
                errors[i] = 0.0f;
            }
        }
        else
        { // hidden layer
            for (i = 0; i < nodeCount; i++)
            {
                sum = 0;
                for (j = 0; j < nodeCount; j++)
                {
                    sum += childLayer.errors[j] * weights[i][j];
                }
                errors[i] = sum * neuronValues[i] *
                (1.0f - neuronValues[i]);
            }
        }
    }

    public void AdjustWeights()
    {
        int i, j;
        float dw;
        if (childLayer != null)
        {
            for (i = 0; i < nodeCount; i++)
            {
                for (j = 0; j < childNodeCount; j++)
                {
                    dw = learningRate * childLayer.errors[j] *
                    neuronValues[i];
                    if (useMomentum)
                    {
                        weights[i][j] += dw + momentumFactor *
                        weightChanges[i][j];
                        weightChanges[i][j] = dw;
                    }
                    else
                    {
                        weights[i][j] += dw;
                    }
                }
            }
            for (j = 0; j < nodeCount; j++)
            {
                biasWeights[j] += learningRate *
                childLayer.errors[j] *
                biasValues[j];
            }
        }
    }
}

public class NeuralNetwork
{
    public NeuralNetworkLayer InputLayer, HiddenLayer, OutputLayer;

    public void Initialize(int nNodesInput, int nNodesHidden, int nNodesOutput)
    {
        InputLayer.nodeCount = nNodesInput;
        InputLayer.childNodeCount = nNodesHidden;
        InputLayer.parentNodeCount = 0;
        InputLayer.Initialize(nNodesInput, null, HiddenLayer);
        InputLayer.RandomizeWeights();
        HiddenLayer.nodeCount = nNodesHidden;
        HiddenLayer.childNodeCount = nNodesOutput;
        HiddenLayer.parentNodeCount = nNodesInput;
        HiddenLayer.Initialize(nNodesHidden, InputLayer, OutputLayer);
        HiddenLayer.RandomizeWeights();
        OutputLayer.nodeCount = nNodesOutput;
        OutputLayer.childNodeCount = 0;
        OutputLayer.parentNodeCount = nNodesHidden;
        OutputLayer.Initialize(nNodesOutput, HiddenLayer, null);
    }

    public void SetInput(int i, float value)
    {
        if ((i >= 0) && (i < InputLayer.nodeCount))
        {
            InputLayer.neuronValues[i] = value;
        }
    }

    public float GetOutput(int i)
    {
        if ((i >= 0) && (i < OutputLayer.nodeCount))
        {
            return OutputLayer.neuronValues[i];
        }
        return (float)int.MaxValue; // to indicate an error
    }

    public void SetDesiredOutput(int i, float value)
    {
        if ((i >= 0) && (i < OutputLayer.nodeCount))
        {
            OutputLayer.desiredValues[i] = value;
        }
    }

    public void FeedForward()
    {
        InputLayer.CalculateNeuronValues();
        HiddenLayer.CalculateNeuronValues();
        OutputLayer.CalculateNeuronValues();
    }
    public void BackPropagate()
    {
        OutputLayer.CalculateErrors();
        HiddenLayer.CalculateErrors();
        HiddenLayer.AdjustWeights();
        InputLayer.AdjustWeights();
    }
    int GetMaxOutputID()
    {
        int i, id;
        double maxval;
        maxval = OutputLayer.neuronValues[0];
        id = 0;
        for (i = 1; i < OutputLayer.nodeCount; i++)
        {
            if (OutputLayer.neuronValues[i] > maxval)
            {
                maxval = OutputLayer.neuronValues[i];
                id = i;
            }
        }
        return id;
    }

    float CalculateError()
    {
        int i;
        float error = 0;
        for (i = 0; i < OutputLayer.nodeCount; i++)
        {
            error += Mathf.Pow(OutputLayer.neuronValues[i] - OutputLayer.desiredValues[i], 2);
        }
        error = error / OutputLayer.nodeCount;
        return error;
    }

    public void SetLearningRate(float rate)
    {
        InputLayer.learningRate = rate;
        HiddenLayer.learningRate = rate;
        OutputLayer.learningRate = rate;
    }

    void SetLinearOutput(bool useLinear)
    {
        InputLayer.linearOutput = useLinear;
        HiddenLayer.linearOutput = useLinear;
        OutputLayer.linearOutput = useLinear;
    }
    public void SetMomentum(bool useMomentum, float factor)
    {
        InputLayer.useMomentum = useMomentum;
        HiddenLayer.useMomentum = useMomentum;
        OutputLayer.useMomentum = useMomentum;
        InputLayer.momentumFactor = factor;
        HiddenLayer.momentumFactor = factor;
        OutputLayer.momentumFactor = factor;
    }
}

public class Rocket : MonoBehaviour
{
    public Rigidbody afterburner, frontboosters;

    NeuralNetwork network;

    float leftThrust, rightThrusht, forwardThrust, backThrust, upThrust;
    float x,y,z;


    void Start()
    {
        network.Initialize(3,,5);
    }

    void Update()
    {
        
    }
}
