using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FlaskeAutomatOpgave
{
    class BottleVendingMachine
    {
        List<Drink> newDrinks = new List<Drink>();
        Queue<Drink> newDrinkTray = new Queue<Drink>(6);
        Queue<Drink> beerTray = new Queue<Drink>(3);
        Queue<Drink> sodaTray = new Queue<Drink>(3);

        public void CreateDrinks()
        {
            int count = 0;
            while (Thread.CurrentThread.IsAlive)
            {

                if (newDrinkTray.Count == 0)
                {
                    lock (newDrinkTray)
                    {
                        if (newDrinkTray.Count != 0)
                        {
                            Monitor.PulseAll(newDrinkTray);
                            Monitor.Wait(newDrinkTray);
                        }
                        else if (newDrinkTray.Count == 0)
                        {
                            while (newDrinkTray.Count < 6)
                            {
                                count++;
                                Drink newSoda = new Drink { Type = "Soda", Number = count };
                                Drink newBeer = new Drink { Type = "Beer", Number = count };
                                Console.WriteLine("producer is producing " + newSoda.Type + " " + newSoda.Number);
                                Console.WriteLine("producer is producing " + newBeer.Type + " " + newBeer.Number);
                                newDrinkTray.Enqueue(newSoda);
                                newDrinkTray.Enqueue(newBeer);
                            }
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
        }


        public void SplitDrinks()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                lock (newDrinkTray)
                {
                    while (newDrinkTray.Count == 0)
                    {
                        Console.WriteLine("Splitter is waiting");
                        Monitor.PulseAll(newDrinkTray);
                        Monitor.Wait(newDrinkTray);
                    }
                    while (newDrinkTray.Count != 0)
                    {
                        if (newDrinkTray.Peek().Type == "Soda" && sodaTray.Count != 3)
                        {
                            Console.WriteLine("Splitting Soda.");
                            sodaTray.Enqueue(newDrinkTray.Dequeue());
                        }
                        else if (newDrinkTray.Peek().Type == "Beer" && beerTray.Count != 3)
                        {
                            Console.WriteLine("Splitting Beer.");
                            beerTray.Enqueue(newDrinkTray.Dequeue());
                        }
                    }
                    Thread.Sleep(1000);
                }
            }
        }


        public void GetSoda()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                lock (sodaTray)
                {
                    while (sodaTray.Count == 0)
                    {
                        Console.WriteLine("Sconsumer is waiting");
                        Thread.Sleep(1000);
                    }
                    Console.WriteLine("Sconsumer consumes " + sodaTray.Peek().Type + " " + sodaTray.Peek().Number.ToString());
                    sodaTray.Dequeue();
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
            }
        }


        public void GetBeer()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                lock (beerTray)
                {
                    while (beerTray.Count == 0)
                    {
                        Console.WriteLine("Bconsumer is waiting");
                        Thread.Sleep(1000);
                    }
                    Console.WriteLine("Bconsumer consumes " + beerTray.Peek().Type + " " + beerTray.Peek().Number.ToString());
                    beerTray.Dequeue();
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
            }
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            BottleVendingMachine bwm = new BottleVendingMachine();

            Thread factory = new Thread(bwm.CreateDrinks);
            Thread splitter = new Thread(bwm.SplitDrinks);
            Thread beerConsumer = new Thread(bwm.GetBeer);
            Thread sodaConsumer = new Thread(bwm.GetSoda);

            factory.Start();
            splitter.Start();
            beerConsumer.Start();
            sodaConsumer.Start();

            Console.Read();
        }
    }


    public class Drink
    {
        public string Type { get; set; }

        public int Number { get; set; }

    }
}
