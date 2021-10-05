using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading;

namespace Snake
{
    class Program
    {
        private static Timer timer;
        static Snake mySnake = new Snake(new Coordinates(x / 2, y / 2));

        static void Main(string[] args)
        {
            ConsoleConfigurations();
            FoodGenerator.CreateFood();

            timer = new Timer(Loop , null , 0 , 130);

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    mySnake.Rotation(key.Key , true);
                }
            }
        }

        static bool CheckIfFaced()
        {
            bool temp = false;
            int i = 0;

            foreach (var item in mySnake.snake)
            {
                i++;
                if (i < mySnake.snake.Count()) 
                {
                    if (item.X == mySnake.snake.LastOrDefault().X && item.Y == mySnake.snake.LastOrDefault().Y)
                    {
                        temp = true;
                    }
                }
            }

            return temp;
        }


        private static bool IsHit() 
        {
            bool temp = false;

            if (mySnake.snake.LastOrDefault().X == 0 || mySnake.snake.LastOrDefault().Y == 0 || mySnake.snake.LastOrDefault().Y >= y || mySnake.snake.LastOrDefault().X >= x)
                temp = true;

            return temp;
        }

        static void Loop(object obj)
        {
            if (IsHit() || CheckIfFaced())
            {
                Console.Clear();
                Console.WriteLine("YOU DIED...");
                System.Environment.Exit(0);
            }
            else if (mySnake.EatFood())
            {
               
            }
            else
            {
                mySnake.MoveSnake();
            }
        }



        class FoodGenerator
        {
            static private Random random = new Random();
            static private Coordinates foodCoordinates = new Coordinates();
            static private void RemoveOldFood()
            {
                if (foodCoordinates != null)
                {
                    Console.SetCursorPosition(foodCoordinates.X, foodCoordinates.Y);
                    Drawer.Clear();
                }
            }

            static private void DrawNewFood()
            {
                Console.SetCursorPosition(foodCoordinates.X, foodCoordinates.Y);
                Drawer.drawFood();
            }

            static public void CreateFood()
            {
                RemoveOldFood();
                foodCoordinates = new Coordinates(random.Next(1, x - 1), random.Next(1, y - 1));

                if (mySnake.snake != null)
                {
                    foreach(Coordinates item in mySnake.snake)
                    {
                        if (item.X == foodCoordinates.X && item.Y == foodCoordinates.Y)
                            foodCoordinates = new Coordinates(random.Next(1, x - 1), random.Next(1, y - 1));
                    }
                }
                    
                DrawNewFood();
            }

            static public Coordinates GetFoodCoordinates() 
            {
                return foodCoordinates;
            }
        }

        enum Directions 
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        const int x = 41;
        const int y = 21;
        class Coordinates 
        {
            public Coordinates()
            {
                if (this.X < 0 || this.Y < 0)
                {
                    Console.Clear();
                    Console.WriteLine("YOU DIED");
                    System.Environment.Exit(0);
                }
            }

            public Coordinates(Coordinates coordinates)
            {
                if (coordinates.X < 0 || coordinates.Y < 0)
                {
                    Console.Clear();
                    Console.WriteLine("YOU DIED");
                    System.Environment.Exit(0);
                }
                else 
                {
                    X = coordinates.X;
                    Y = coordinates.Y;
                }
            }

            public Coordinates(int x , int y)
            {
                if (this.X < 0 || this.Y < 0)
                {
                    Console.Clear();
                    Console.WriteLine("YOU DIED");
                    System.Environment.Exit(0);
                }
                else 
                {
                    X = x;
                    Y = y;
                }
            }

            public int X { get; set; }
            public int Y { get; set; }
        }

        class Snake
        {
            public Queue<Coordinates> snake;
            public Directions direction;

            public Snake(Coordinates coordinates , int snakeLength = 3)
            {
                direction = Directions.RIGHT;
                snake = new Queue<Coordinates>();

                for (int i = coordinates.X - snakeLength + 1; i <= coordinates.X; i++)
                {
                    Coordinates point = new Coordinates(i, coordinates.Y);
                    snake.Enqueue(point);
                    Drawer.drawPoint(point);
                }
            }

            public Coordinates GetLast() => snake.LastOrDefault();

            public Coordinates GetNextCoordinates()
            {
                 Coordinates p = new Coordinates(GetLast());
                 switch (direction)
                 {
                     case Directions.LEFT:
                         p.X--;
                         break;
                     case Directions.RIGHT:
                         p.X++;
                         break;
                     case Directions.UP:
                         p.Y--;
                         break;
                     case Directions.DOWN:
                         p.Y++;
                         break;
                    }
                    return p;
            }

            public void MoveSnake()
            {
                Coordinates nextCoordinates = GetNextCoordinates();

                Drawer.DrawHead(nextCoordinates , snake);
                Drawer.RemoveTail(snake);
            }

            public bool EatFood() 
            {
               Coordinates foodCoordinates = FoodGenerator.GetFoodCoordinates();
               Coordinates myCoordinates = mySnake.snake.LastOrDefault();
                if (myCoordinates.X == foodCoordinates.X  && myCoordinates.Y == foodCoordinates.Y)
                {
                    Drawer.DrawHead(mySnake.GetNextCoordinates(), snake);
                    FoodGenerator.CreateFood();
                    return true;
                }
                else 
                {
                    return false;
                }
            }

            public void Rotation(ConsoleKey key , bool rotate)
            {
                if (rotate)
                {
                    switch (direction)
                    {
                        case Directions.LEFT:
                        case Directions.RIGHT:
                            if (key == ConsoleKey.DownArrow)
                                direction = Directions.DOWN;
                            else if (key == ConsoleKey.UpArrow)
                                direction = Directions.UP;
                            break;
                        case Directions.UP:
                        case Directions.DOWN:
                            if (key == ConsoleKey.LeftArrow)
                                direction = Directions.LEFT;
                            else if (key == ConsoleKey.RightArrow)
                                direction = Directions.RIGHT;
                            break;
                    }
                    rotate = false;
                }
            }

        }

        class Drawer
        {
            private static readonly char pointSymbol = '*';
            private static readonly char fieldSymbol = '%';
            private static readonly char foodSymbol = '@';

            public static void FieldDrawer()
            {
                for (int i = 0; i <= x; i++)
                {
                    Console.SetCursorPosition(i, 0);
                    Console.Write(fieldSymbol);
                }

                for (int k = 0; k <= x; k++)
                {
                    Console.SetCursorPosition(k, y);
                    Console.Write(fieldSymbol);
                }

                for (int i = 0; i <= y; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write(fieldSymbol);
                }

                for (int k = 0; k <= y; k++)
                {
                    Console.SetCursorPosition(x, k);
                    Console.Write(fieldSymbol);
                }
            }

            public static void drawFood()
            {
                Console.Write(foodSymbol);
            }

            public static void drawPoint(Coordinates coordinates)
            {
                Console.SetCursorPosition(coordinates.X, coordinates.Y);
                Console.Write(pointSymbol);
            }
            public static void Clear()
            {
                Console.Write(' ');
            }

            public static void DrawHead(Coordinates myCoordinates , Queue<Coordinates> snake)
            { 
                Console.SetCursorPosition(myCoordinates.X, myCoordinates.Y);
                Console.Write(pointSymbol);
                snake.Enqueue(myCoordinates);
            }

            public static void RemoveTail(Queue<Coordinates> snake)
            {
                Coordinates myCoordinates = snake.Peek();
                Console.SetCursorPosition(myCoordinates.X, myCoordinates.Y);
                Clear();
                snake.Dequeue();
            }
        }
        static void ConsoleConfigurations() 
        {
            Console.SetWindowSize(x + 1, y + 1);
            Console.SetBufferSize(x + 1, y + 1);
            Console.CursorVisible = false;
            Drawer.FieldDrawer();
        }
    }
}
