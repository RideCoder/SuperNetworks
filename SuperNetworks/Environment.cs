using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperNetworks
{
    public class Environment : Game
    {
        private GraphicsDeviceManager graphics;

        private MouseState mouseState;
        private SpriteBatch spriteBatch;
        private Texture2D squareTexture;
        private Texture2D texture;
        private SpriteFont timesNewRoman;
        private List<Neuron> outputNeurons = new List<Neuron>();
        private NeuralNetwork net;
        private NeuralNetwork oldNet;
        private int fitness = 0;
        private string outputText;
        float maxWidth = 400f; // Set the maximum width for the text box
        private int activations = 0;
        private int curActivations = 0;
        public Environment()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            squareTexture = new Texture2D(GraphicsDevice, 1, 1);
            texture = Content.Load<Texture2D>("neuron1");
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);
            squareTexture.SetData(new Color[] { Color.White });

            // Initialize NeuralNetwork here
            net = new NeuralNetwork(squareTexture, 9, 0);
            foreach (var neuron in net.neurons)
            {
                if (neuron.type == Type.Output)
                {
                    outputNeurons.Add(neuron);
                }
            }
           
            base.Initialize();
        }
        private string FilterUnsupportedCharacters(string text, SpriteFont font)
        {
            StringBuilder filteredText = new StringBuilder();

            foreach (char c in text)
            {
                if (font.Characters.Contains(c))
                {
                    filteredText.Append(c);
                }
            }

            return filteredText.ToString();
        }
        private string WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
        {
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                Vector2 size = spriteFont.MeasureString(c.ToString());

                if (c == ' ')
                {
                    if (lineWidth + size.X < maxLineWidth)
                    {
                        sb.Append(c);
                        lineWidth += size.X;
                    }
                    else
                    {
                        sb.Append("\n");
                        lineWidth = 0f;
                    }
                }
                else
                {
                    if (lineWidth + size.X < maxLineWidth)
                    {
                        sb.Append(c);
                        lineWidth += size.X;
                    }
                    else
                    {
                        sb.Append("\n" + c);
                        lineWidth = size.X;
                    }
                }
            }

            return sb.ToString();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            timesNewRoman = Content.Load<SpriteFont>("Times New Roman");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            String output = "";
            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.R) && net != null)
            {
                Random r = new Random();
                foreach (var n in net.neurons)
                {

                    n.activation = 1;


                }
               
            }

            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);

            if (mouseState.LeftButton == ButtonState.Pressed && net != null)
            {
                float neuronRadius = 16f; // Adjust this value based on the neuron's visual representation

                foreach (var neuron in net.neurons)
                {
                    float distance = Vector2.Distance(neuron.Position, mousePosition);
                  
                    if (distance <= neuronRadius)
                    {
                        neuron.activation = 1;
                    }
                }
            }

            foreach (var neuron in net.neurons)
            {
                neuron.Update(gameTime);
                if (neuron.type == Type.Output)
                {
                    if (neuron.threshold <= neuron.activation)
                    {
                        output += "1";
                    }
                    else
                    {
                        output += "0";
                    }
                }

            }
            var ascii = new StringBuilder();
            for (int i = 0; i < output.Length; i += 8)
            {
                string byteString = output.Substring(i, 8);
                byte b = Convert.ToByte(byteString, 2);
                ascii.Append((char)b);
            }

            outputText += ascii;
            net.mutate((net.connectionCount/5)+20);
            base.Update(gameTime);
        }
        public void DrawLine(SpriteBatch spriteBatch, Texture2D pixelTexture, Vector2 start, Vector2 end, Color color, float thickness)
        {
            Vector2 edge = end - start;
            // Calculate the angle to rotate the line
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            // Draw the line
            spriteBatch.Draw(pixelTexture,
                             start,
                             null,
                             color,
                             angle,
                             Vector2.Zero,
                             new Vector2(edge.Length(), thickness),
                             SpriteEffects.None,
                             0);
        }
        protected override void Draw(GameTime gameTime)
        {
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            GraphicsDevice.Clear(Color.Black);
            curActivations = 0;
            spriteBatch.Begin();
            foreach (var neuron in net.neurons)
            {
                foreach (Connection c in neuron.connections)
                {
                    Color wireCol = new Color(-c.weight / 3, c.weight / 3, 0);

                    if (neuron.active)
                    {
                        
                            DrawLine(spriteBatch, squareTexture, neuron.Position, new Vector2(net.neurons[c.index].Position.X, net.neurons[c.index].Position.Y), Color.White, neuron.activation * 8);
                        activations = activations + 1;
                        curActivations = curActivations + 1;
                        neuron.active = false;
                    }
                   
                

                }
            }
            
            foreach (var neuron in net.neurons)
            {
                Color col;
                if (neuron.type == Type.Normal)
                {
                    col = new Color(1 - neuron.activation, neuron.activation, 0);
                }
                else if (neuron.type == Type.Output)
                {
                    col = new Color(neuron.activation+.2f, neuron.activation + .2f, neuron.activation + .2f);
                }
                else
                {
                    col = Color.White; // Default color if no conditions are met
                }
                spriteBatch.Draw(texture, neuron.Position, null, col, 0f, origin, 1f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(timesNewRoman, Math.Round(Math.Abs(neuron.activation),2).ToString(), new Vector2(neuron.Position.X-texture.Width/2,neuron.Position.Y-texture.Height-8), Color.White);
                spriteBatch.DrawString(timesNewRoman, neuron.refactory.ToString(), new Vector2(neuron.Position.X - texture.Width / 2, neuron.Position.Y - texture.Height - 24), Color.Red);

              
            }
            string filteredText = FilterUnsupportedCharacters(outputText, timesNewRoman);
            string wrappedText = WrapText(timesNewRoman, filteredText, maxWidth);
            spriteBatch.DrawString(timesNewRoman, wrappedText, new Vector2(1470,100), Color.White);
            spriteBatch.DrawString(timesNewRoman, "Neurons: "+net.count, new Vector2(1470, 0), Color.White);
            spriteBatch.DrawString(timesNewRoman, "Synapses: " + net.connectionCount, new Vector2(1470, 20), Color.White);
            spriteBatch.DrawString(timesNewRoman, "Activations: " + activations, new Vector2(1470, 40), Color.White);
            spriteBatch.DrawString(timesNewRoman, "Current Activations: " + curActivations, new Vector2(1470, 60), Color.White);
            // Add drawing logic here
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}