using System;
namespace BunrodDaddy
{
    using System.Collections.Generic;
    using Bunrod_Daddy;
    using Microsoft.Xna.Framework;

    public class Bubble
    {
        public Vector2 Posicion;
        public Vector2 Direccion;
        public Color Color{ get;  private set; }

        public Boolean moving = false;
        public Boolean destroy = false;

        public Vector2 positionBox { get; set; }
        public List<Bubble> Neighbors { get; set; }


        public Bubble(Vector2 posicion, Color color)
        {
            Posicion = posicion;
            this.Direccion = Vector2.Zero;
            Color = color;
        }

        public void Mover()
        {
            if (moving)
            {
                this.Posicion += Direccion;
            }
        }

        public void findClosestBox()
        {
            int fy = (int)(Posicion.Y - Game1.limitUp + (Game1.bubbleSize / 2))
               / Game1.bubbleSize;
            int fx = (int)((Posicion.X - Game1.limitLeft + (Game1.bubbleSize / 2) - ((fy % 2) *
               (Game1.bubbleSize / 2))) / Game1.bubbleSize);

            positionBox = new Vector2(fx, fy);
        }

        public List<Bubble> findTheSame()
        {
            List<Bubble> ready = new List<Bubble>();
            ready.Add(this);
            this.destroy = true;

            // find equals recursively ค้นหาเท่ากับแบบเรียกซ้ำ
            findTheSame(ready, this);

            return ready;
        }

        public void findTheSame(List<Bubble> lista, Bubble bubble)
        {
            bubble.findNeighbors();
            foreach (Bubble bubbles in bubble.Neighbors)
            {
                // if the color is the same and it has not been previously added to the list
                if (Color == bubbles.Color && !lista.Contains(bubbles))
                {
                    bubbles.destroy = true;
                    lista.Add(bubbles);
                    findTheSame(lista, bubbles);
                }
            }
        }

        public void findNeighbors()
        {
            Neighbors.Clear();

            foreach (Bubble bubble in Game1.bubblesStuck)
            {
                // look for bubbles nearby
                if (nextTo(bubble))
                {
                    Neighbors.Add(bubble);
                }
            }
        }

        public bool nextTo(Bubble bubble)
        {
            if (bubble.positionBox == new Vector2(positionBox.X - ((positionBox.Y + 1) % 2), positionBox.Y - 1))
            {
                //check top left
                return true;
            }
            else if (bubble.positionBox == new Vector2(positionBox.X - ((positionBox.Y + 1) % 2) + 1, positionBox.Y - 1))
            {
                // check above
                return true;
            }
            else if (bubble.positionBox == new Vector2(positionBox.X - 1, positionBox.Y))
            {
                // check left
                return true;
            }
            else if (bubble.positionBox == new Vector2(positionBox.X + 1, positionBox.Y))
            {
                // check der
                return true;
            }
            else if (bubble.positionBox == new Vector2(positionBox.X - ((positionBox.Y + 1) % 2), positionBox.Y + 1))
            {
                // check bottom left
                return true;
            }
            else if (bubble.positionBox == new Vector2(positionBox.X + 1 - ((positionBox.Y + 1) % 2), positionBox.Y + 1))
            {
                // check below
                return true;
            }
            return false;
        }


        public List<Bubble> findBubblesConnected()
        {
            List<Bubble> lista = new List<Bubble>();
            lista.Add(this);
            // find recursively connected bubbles
            findBubblesConnected(lista, this);

            return lista;
        }

        public void findBubblesConnected(List<Bubble> lista, Bubble bubble)
        {
            bubble.findNeighbors();
            foreach (Bubble bubbles in bubble.Neighbors)
            {
                // if not previously added
                if (!bubbles.destroy && !lista.Contains(bubble))
                {
                    lista.Add(bubble);
                    //if it is root we exit to avoid searching in all the bubbles
                    if (bubbles.positionBox.Y == 0)
                        break;
                    findBubblesConnected(lista, bubble);
                }
            }
        }
    }
}
