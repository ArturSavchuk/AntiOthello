using AntiOthello.Model;
using System;
using System.Collections.Generic;


namespace AntiOthello.Controller
{
    public class ConsoleInput
    {
        private readonly GameProcessor gameProcessor;

        public ConsoleInput(GameProcessor gameProcessor)
        {
            this.gameProcessor = gameProcessor;
        }

        public void StartGame()
        {
            AIPlayer firstPlayer;
            ConsolePlayer secondPlayer = new ConsolePlayer(gameProcessor);
            Tuple<int, int> blackHole = GetBlackHole();

            PointState color = GetColor();
            if (color == PointState.White)
            {
                firstPlayer = new AIPlayer(gameProcessor, false);
                gameProcessor.StartGame(blackHole, secondPlayer.MakeMove());
            }
            else
            {
                firstPlayer = new AIPlayer(gameProcessor, true);
                gameProcessor.StartGame(blackHole);
            }

            //game loop
            while (true)
            {
                firstPlayer.MakeMove();
                secondPlayer.MakeMove();
            }
        }

        public Tuple<int, int> GetBlackHole()
        {
            return ConvertToIntCoords(Console.ReadLine());
        }

        public PointState GetColor()
        {
            string color = Console.ReadLine();
            if (color == "white")
            {
                return PointState.White;
            }
            else if (color == "black")
            {
                return PointState.Black;
            }
            return PointState.Black;
        }

        public static Tuple<int, int> ConvertToIntCoords(string coords)
        {
            List<char> firstSymbols = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            List<char> secondSymbols = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8' };

            coords = coords.ToLower().Trim();
            int column = firstSymbols.FindIndex(x => x == coords[0]);
            int row = secondSymbols.FindIndex(x => x == coords[1]);
            return new Tuple<int, int>(row, column);
        }
    }
}
