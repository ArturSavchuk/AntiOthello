using System;
using System.Collections.Generic;
using System.Text;

namespace AntiOthello.Model
{
    public class AIPlayer
    {
        private static Dictionary<int, Point> actions = new Dictionary<int, Point>();
        private const int negInfinity = -2147483647;
        private const int posInfinity = 2147483647;
        private List<char> firstSymbols = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
        private List<char> secondSymbols = new List<char> { '1', '2', '3', '4', '5', '6', '7', '8' };
        private bool isFirst;
        private enum scoreType { AI_PIECE, AI_EDGE, AI_CORNER, PLAYER_PIECE, PLAYER_EDGE, PLAYER_CORNER };
        private static int[] scoring = new int[6] { 1, 1, 1, -1, -50, -50 };
        public GameProcessor gameProcessor;

        int[][] PointRate = new int[][]{
            new int[]{500, -110, 20, 23, 22, 20, -110, 502},
            new int[]{-109, -146,  -19, -6, -6, -20, -146, -111},
            new int[]{19, -20,  -27, -4, -4, -28, -20, 20},
            new int[]{25, -6,  -4, 0 , 0, -4, -5, 26},
            new int[]{26, -7,  -4, 0 , 0, -4, -6, 27},
            new int[]{21, -20,  -28, -4, -4, -28, -20, 19},
            new int[]{-110, -146,  -20, -6, -6, -20, -146, -110},
            new int[]{501, -110, 20, 24, 23, 20, -110, 501}
        };

        public AIPlayer(GameProcessor gameProcessor, bool isFirst)
        {
            this.gameProcessor = gameProcessor;
            this.isFirst = isFirst;
        }
        private List<List<Point>> CloneGameState(List<List<Point>> list)
        {
            List<List<Point>> copy = new List<List<Point>>();
            foreach (var row in list)
            {
                List<Point> rowCopy = new List<Point>();
                foreach (var point in row)
                {
                    Point pointCopy = new Point(point.State);
                    rowCopy.Add(pointCopy);
                }
                copy.Add(rowCopy);
            }
            return copy;
        }
        public void MakeMove()
        {
            List<Tuple<int, int>> availablePoints = gameProcessor.GetAvailablePoints();

            //pass if there is no available moves
            if (availablePoints.Count == 0)
            {
                gameProcessor.Pass();
                return;
            }

            //set dafault values
            Tuple<int, int> bestMove = availablePoints[0];
            int bestScore = int.MaxValue;

            //get the best move from all
            foreach (var move in availablePoints)
            {
                List<List<Point>> pointsBeforeMove = CloneGameState(gameProcessor.GetPoints());
                gameProcessor.MakeMove(move);
                int score = MiniMax(3, int.MinValue, int.MaxValue, true);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }

                gameProcessor.UndoMove(pointsBeforeMove);
            }

            //make move
            Console.WriteLine(ConvertCoords(bestMove));
            gameProcessor.MakeMove(bestMove);
        }
        public string ConvertCoords(Tuple<int, int> coords)
        {
            return firstSymbols[coords.Item2].ToString() + secondSymbols[coords.Item1];
        }
        public int MiniMax(int depth, int alpha, int beta, bool isMinimizing)
        {
            if (depth == 0 || gameProcessor.IsGameFinished())
            {
                return Eval(gameProcessor.GetPoints());
            }

            if (isMinimizing)
            {
                return Min(depth, alpha, beta);
            }
            else
            {
                return Max(depth, alpha, beta);
            }
        }
        private int Min(int depth, int alpha, int beta)
        {
            int bestScore = int.MaxValue;
            var availableMoves = gameProcessor.GetAvailablePoints();
            //pass if there is no available moves
            if (availableMoves.Count == 0)
            {
                List<List<Point>> pointsBeforeMove = CloneGameState(gameProcessor.GetPoints());
                gameProcessor.PassWithoutMassage();
                int score = MiniMax(depth - 1, alpha, beta, false);
                bestScore = GetMin(score, bestScore);
                gameProcessor.UndoMove(pointsBeforeMove);
            }
            else
            {
                //go deeper in tree for every posible move
                foreach (var move in availableMoves)
                {
                    List<List<Point>> pointsBeforeMove = CloneGameState(gameProcessor.GetPoints());
                    gameProcessor.MakeMove(move);
                    int score = MiniMax(depth - 1, beta, alpha, false);
                    bestScore = GetMin(score, bestScore);
                    beta = GetMin(beta, bestScore);

                    gameProcessor.UndoMove(pointsBeforeMove);

                    //alpha-beta puning
                    if (beta <= alpha)
                        break;
                }
            }
            return bestScore;
        }

        private int Max(int depth, int alpha, int beta)
        {
            int bestScore = int.MinValue;
            var availableMoves = gameProcessor.GetAvailablePoints();

            //pass if there is no available moves
            if (availableMoves.Count == 0)
            {
                List<List<Point>> pointsBeforeMove = CloneGameState(gameProcessor.GetPoints());
                gameProcessor.PassWithoutMassage();
                int score = MiniMax(depth - 1, alpha, beta, true);
                bestScore = GetMax(score, bestScore);
                gameProcessor.UndoMove(pointsBeforeMove);
            }
            else
            {
                //go deeper in tree for every posible move
                foreach (var move in availableMoves)
                {
                    List<List<Point>> pointsBeforeMove = CloneGameState(gameProcessor.GetPoints());
                    gameProcessor.MakeMove(move);
                    int score = MiniMax(depth - 1, beta, alpha, true);
                    bestScore = GetMax(score, bestScore);
                    alpha = GetMax(beta, bestScore);

                    gameProcessor.UndoMove(pointsBeforeMove);

                    //alpha-beta puning
                    if (beta <= alpha)
                        break;
                }
            }
            return bestScore;
        }

        public int Eval(List<List<Point>> points)
        {
            int score = 0;
            PointState playerColor;
            if (isFirst)
            {
                playerColor = PointState.Black;
            }
            else
            {
                playerColor = PointState.White;
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (points[i][j].State == playerColor)
                    {
                        score += PointRate[i][j];
                    }
                }
            }
            return score;
        }

        private int GetMin(int firstValue, int secondValue)
        {
            if (firstValue < secondValue)
                return firstValue;
            return secondValue;
        }

        private int GetMax(int firstValue, int secondValue)
        {
            if (firstValue > secondValue)
                return firstValue;
            return secondValue;
        }
    }
}
