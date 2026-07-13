using System;

public class Brain
{
    public float[,] hiddenWeights;
    public float[] hiddenBiases;

    public float[] outputWeights;
    public float outputBias;

    public float[] inputs;
    private System.Random rng;

    public Brain(int numberOfInputs)
    {
        rng = new System.Random();
        inputs = new float[numberOfInputs];

        int hiddenNeurons = 4;
        hiddenWeights = new float[numberOfInputs, hiddenNeurons];
        hiddenBiases = new float[hiddenNeurons];
        outputWeights = new float[hiddenNeurons];

        for (int i = 0; i < numberOfInputs; i++)
        {
            for (int j = 0; j < hiddenNeurons; j++)
            {
                hiddenWeights[i, j] = (float)(rng.NextDouble() * 2.0) - 1f;
            }
        }

        for (int i = 0; i < hiddenNeurons; i++)
        {
            hiddenBiases[i] = (float)(rng.NextDouble() * 2.0) - 1f;
            outputWeights[i] = (float)(rng.NextDouble() * 2.0) - 1f;
        }
        outputBias = (float)(rng.NextDouble() * 2.0) - 1f;
    }

    public Brain(float[,] hWeights, float[] hBiases, float[] oWeights, float oBias)
    {
        rng = new System.Random();

        this.hiddenWeights = (float[,])hWeights.Clone();
        this.hiddenBiases = (float[])hBiases.Clone();
        this.outputWeights = (float[])oWeights.Clone();
        this.outputBias = oBias;

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

        for (int j = 0; j < hiddenCount; j++)
        {
            float sum = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                sum += inputs[i] * hiddenWeights[i, j];
            }
            hiddenOutputs[j] = Sigmoid(sum + hiddenBiases[j]);
        }

        float finalSum = 0;
        for (int i = 0; i < hiddenCount; i++)
        {
            finalSum += hiddenOutputs[i] * outputWeights[i];
        }

        float score = Sigmoid(finalSum + outputBias);
        return score > 0.5f;
    }
}
