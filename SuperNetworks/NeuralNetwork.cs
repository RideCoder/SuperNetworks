using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperNetworks
{
    public class NeuralNetwork
    {
        public Neuron[] neurons;
        Random r = new Random();
        private Texture2D squareTexture;
        public int count;
        private int scale;
        public int connectionCount;
        public float closestFoodX = 1000;
        public float closestFoodY = 1000;
        public Color color;
        public int gen;

        public NeuralNetwork(Texture2D squareTexture)
        {
            this.squareTexture = squareTexture;
        }


        public NeuralNetwork(Texture2D squareTexture, int neuronCount, int connectionCount)
        {
            // TODO: Add your initialization logic here

            neurons = new Neuron[neuronCount];

            color = new Color(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255));
            count = neuronCount;
            
            this.connectionCount = connectionCount;
            scale = neuronCount / 10;

           



            for (int i = 0; i < neuronCount; i++)
            {





                if (i < 8)
                {
                    neurons[i] = new Neuron(squareTexture, this, i, r.Next(55) + 4)
                    { Position = new Vector2(((float)r.NextDouble() * 1400) + 50, ((float)r.NextDouble() * 1000) + 50), type = Type.Output };
                }
                else
                {
                    neurons[i] = new Neuron(squareTexture, this, i, r.Next(55) + 4)
                    { Position = new Vector2(((float)r.NextDouble() * 1400) + 50, ((float)r.NextDouble() * 1000) + 50) };
                }
                        



            }
            Parallel.For(0, connectionCount, i =>
            {
                int rand = r.Next(neurons.Length);
                int source = r.Next(neurons.Length - 1);
                if (source >= rand) source++; // Ensuring source is different from rand
           
                    rand = r.Next(neurons.Length);
                

                Connection c = new Connection(rand, (float)(r.NextDouble() * 2) - 1);
                lock (neurons[source].connections)
                {

                    neurons[source].connections.Add(c);
                }
            });




        }
        private void AddNewNeuron()
        {
            // Create a new Neuron with a random type (excluding ClosestFood for simplicity)
            var typeProbabilities = new Dictionary<Type, double>
    {
        { Type.Normal, 1 },  // Example probability for Normal
      
    };

       
            Neuron newNeuron = new Neuron(squareTexture, this, neurons.Length, r.Next(55) + 5)
            { Position = new Vector2(((float)r.NextDouble() * 1400) + 50, ((float)r.NextDouble() * 1000) + 50) };

            // Resize the neurons array to accommodate the new neuron
            Array.Resize(ref neurons, neurons.Length + 1);
            neurons[neurons.Length - 1] = newNeuron;

            // Connect the new neuron to a random existing neuron
            int existingNeuronIndex = r.Next(neurons.Length - 1);
            newNeuron.connections.Add(new Connection(existingNeuronIndex, (float)r.NextDouble()));
            count++;
            connectionCount++;
        }
        private void AddNewWeight()
        {
            int sourceIndex = r.Next(neurons.Length);
            int targetIndex = r.Next(neurons.Length);
           


           
            
                targetIndex = r.Next(neurons.Length);



            // Add connection if it doesn't exist
            if (neurons[sourceIndex].type != Type.Output && neurons[targetIndex].type != Type.Output)
            {
                if (neurons[sourceIndex] != neurons[targetIndex])
                {
                    if (!neurons[sourceIndex].connections.Any(c => c.index == targetIndex))
                    {
                        connectionCount++;
                        neurons[sourceIndex].connections.Add(new Connection(targetIndex, (float)(r.NextDouble() * 2 - 1)));
                    }
                }
            }
        }


        private void ModifyWeight()
        {
            Neuron neuron = neurons[r.Next(neurons.Length)];
            if (neuron.connections.Any())
            {
                int connectionIndex = r.Next(neuron.connections.Count);
                Connection conn = neuron.connections[connectionIndex];
                conn.weight += (float)(r.NextDouble() - 0.5) * (float).5; // Modify weight
                neuron.connections[connectionIndex] = conn; // Update the connection
            }
        }
        private void RemoveWeight()
        {
            
            Neuron neuron = neurons[r.Next(neurons.Length)];
            if (neuron.connections.Any())
            {
                neuron.connections.RemoveAt(r.Next(neuron.connections.Count));
                connectionCount--;
            }
        }
        public void mutate(int power)
        {
            float num = ((float)(neurons.Length / (11.42 * Math.Pow(neurons.Length, 1.29))));
            // Define probabilities for each mutation type
            double addNeuronProbability = .01 * num;  // 20% chance to add a new neuron
            double addWeightProbability = 0.01;  // 20% chance to add a new weight
            double modifyWeightProbability = .998 - .01 - num * .01;  // 40% chance to modify a weight
            double removeWeightProbability = 0.002;  // 10% chance to remove a weight
            double removeNeuronProbability = 0;  // 10% chance to remove a neuron

            // Generate a random number between 0 and 1
            for (int i = 0; i < power; i++)
            {


                double randomNumber = r.NextDouble();

                // Accumulate probabilities to select the mutation based on the random number
                if (randomNumber < addNeuronProbability)
                {
                
                        AddNewNeuron();
                    
                }
                else if (randomNumber < addNeuronProbability + addWeightProbability)
                {
                  
                        AddNewWeight();

                    
                }
                else if (randomNumber < addNeuronProbability + addWeightProbability + modifyWeightProbability)
                {
                    ModifyWeight();
                }
                else if (randomNumber < addNeuronProbability + addWeightProbability + modifyWeightProbability + removeWeightProbability)
                {
                    RemoveWeight();
                }
              
            }
        }
    }



}
