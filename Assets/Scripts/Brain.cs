using System;

public class Brain
{
    // 2D Matrix for Input-to-Hidden weights (5 inputs mapped to 4 hidden neurons)
    public float[,] hiddenWeights;
    public float[] hiddenBiases;

    // 1D Vector for Hidden-to-Output weights (4 hidden outputs mapped to 1 final choice)
    public float[] outputWeights;
    public float outputBias;

    public float[] inputs;
    private System.Random rng;

    // Constructor for Gen 1 (Random initialization)
    public Brain(int numberOfInputs)
    {
        rng = new System.Random();
        inputs = new float[numberOfInputs];

        int hiddenNeurons = 4; // 4 hidden neurons is the sweet spot for Flappy Bird
        hiddenWeights = new float[numberOfInputs, hiddenNeurons];
        hiddenBiases = new float[hiddenNeurons];
        outputWeights = new float[hiddenNeurons];

        // Randomize Matrix A
        for (int i = 0; i < numberOfInputs; i++)
        {
            for (int j = 0; j < hiddenNeurons; j++)
            {
                hiddenWeights[i, j] = (float)(rng.NextDouble() * 2.0) - 1f;
            }
        }

        // Randomize Matrix B and Biases
        for (int i = 0; i < hiddenNeurons; i++)
        {
            hiddenBiases[i] = (float)(rng.NextDouble() * 2.0) - 1f;
            outputWeights[i] = (float)(rng.NextDouble() * 2.0) - 1f;
        }
        outputBias = (float)(rng.NextDouble() * 2.0) - 1f;
    }

    // CONSTRUCTOR FOR GENERATION 2+ (Cloning the winner)
    public Brain(float[,] hWeights, float[] hBiases, float[] oWeights, float oBias)
    {
        rng = new System.Random();

        // Deep clone everything so they have separate storage arrays
        this.hiddenWeights = (float[,])hWeights.Clone();
        this.hiddenBiases = (float[])hBiases.Clone();
        this.outputWeights = (float[])oWeights.Clone();
        this.outputBias = oBias;

        // FIXED: Force lock the array allocation size to exactly match your 5 environment inputs
        inputs = new float[5];
    }


    public void UpdateInputs(float[] newInputs)
    {
        inputs = newInputs;
    }

    private float Sigmoid(float input)
    {
        return 1f / (1f + (float)Math.Exp(-input));
    }

    public bool Think()
    {
        int hiddenCount = hiddenBiases.Length;
        float[] hiddenOutputs = new float[hiddenCount];

        // 1. Calculate Hidden Layer Neurons (Dot products for each hidden neuron)
        for (int j = 0; j < hiddenCount; j++)
        {
            float sum = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                sum += inputs[i] * hiddenWeights[i, j];
            }
            hiddenOutputs[j] = Sigmoid(sum + hiddenBiases[j]);
        }

        // 2. Calculate Final Output Neuron (Dot product of hidden outputs and final weights)
        float finalSum = 0;
        for (int i = 0; i < hiddenCount; i++)
        {
            finalSum += hiddenOutputs[i] * outputWeights[i];
        }

        float score = Sigmoid(finalSum + outputBias);
        return score > 0.5f;
    }
}
