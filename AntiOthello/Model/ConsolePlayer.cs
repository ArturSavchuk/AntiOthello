using AntiOthello.Controller;
using System;
using System.Collections.Generic;
using System.Text;


namespace AntiOthello.Model
{
    public class ConsolePlayer
    {
        private List<char> firstSymbols = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
        private List<char> secondSymbols = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8' };
        private GameProcessor gameProcessor;

        public ConsolePlayer(GameProcessor gameProcessor)
        {
            this.gameProcessor = gameProcessor;
        }

        /// <summary>
        /// Make move from console
        /// </summary>
        /// <returns></returns>
        public Tuple<int, int> MakeMove()
        {
            List<Tuple<int, int>> availablePoints = gameProcessor.GetAvailablePoints();
            string move;
            Tuple<int, int> moveCoords = new Tuple<int, int>(-1, -1);

            //read from console white input isn`t correct
            do
            {
                move = Console.ReadLine();
                move = move.Trim().ToLower();


                if (!IsCorrectMove(move))
                {
                    //if this is word "pass" - make move pass
                    if (move == "pass")
                    {
                        gameProcessor.PassWithoutMassage();
                        break;
                    }
                    continue;
                }

                //make correct and available move
                moveCoords = ConsoleInput.ConvertToIntCoords(move);
                if (availablePoints.Contains(moveCoords))
                {
                    gameProcessor.MakeMove(moveCoords);
                    break;
                }
            }
            while (true);

            return moveCoords;
        }


        public bool IsCorrectMove(string move)
        {
            if (move.Length != 2)
                return false;
            return firstSymbols.Contains(move[0]) && secondSymbols.Contains(move[1]);
        }

    }
}
