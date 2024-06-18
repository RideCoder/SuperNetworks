using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace SuperNetworks
{

    public enum Type
    {
        Normal,
        Output
    }
    public struct Connection
    {
        public long index;
        public float weight;


        public Connection(long index, float weight)
        {
            this.index = index;
            this.weight = weight;

        }
    }

    public class Neuron
    {
        private Texture2D texture;
        public Neuron[] neurons;
        public Type type = Type.Normal;
        public NeuralNetwork neuralNetwork;
        public Texture2D pixelTexture;
        public List<Connection> connections = new List<Connection>();
     
        private SpriteFont bangers;
        public int index;
        public Vector2 Position;
        public float activation;
        public float threshold = 0;
        public float sum = 0;
        public int refactory;
        public int timer;
        public bool active = false;
        private Random r = new Random();

        public Neuron(Texture2D pixelTexture, NeuralNetwork network, int index, int refactory)
        {
            this.pixelTexture = pixelTexture;
            this.type = type;
            this.neuralNetwork = network;
            neurons = network.neurons;
            this.index = index;
            Random r = new Random();
            threshold = .01f;// (float)r.NextDouble();
            int temp = r.Next(30);
            this.refactory = refactory;
            timer = 0;// r.Next(temp);
            // neurons[0] = null;
        }

        public void propagate()
        {

            foreach (Connection c in connections)
            {
                if (activation >= 1)
                {
                    activation = 1;

                }
                if (activation <= 0)
                {
                    activation = 0;
                }
                if (c.index < 0 || c.index >= neurons.Length)
                {
                    continue; // Skip this iteration to avoid the exception
                }
              
                    neurons[c.index].activation += activation * c.weight;


                if (activation >= 1)
                {
                    activation = 1;

                }
                if (activation <= 0)
                {
                    activation = 0;
                }
            }
        }

        public void Update(GameTime gameTime)
        {

            if (activation >= 1)
            {
                activation = 1;

            }
            if (activation <= 0)
            {
                activation = 0;
            }
            timer = timer + 1;

            if (timer > refactory)
            {
                timer = 0;


                if (activation >= threshold)
                {
                    if (neurons != null)
                    {
                        active = true;
                        propagate();
                    }

                }

            }
           

            if (activation > 0)
            {
                activation -= .01f;
            }
          
     
            if (activation >= 1)
            {
                activation = 1;
            }
            if (activation <= 0)
            {
                activation = 0;
            }




        }

    }


}
