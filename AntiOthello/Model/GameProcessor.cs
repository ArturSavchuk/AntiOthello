using System;
using System.Collections.Generic;

namespace AntiOthello.Model
{
    public class GameProcessor
    {
        private PointState firstPlayerColor = PointState.Black;
        private PointState secondPlayerColor = PointState.White;
        private PointState currentPlayerColor;
        Board board;
        public int passedMovesCount = 0;
        
        //events
        //*******************************************
        public event Action<List<List<Point>>> MoveMade;
        public event Action<List<List<Point>>> GameStarted;
        public event Action<List<Tuple<int, int>>> AvailablePointsCalculated;
        public event Action<int, int> GameFinished;
        public event Action<int, int> ScoresCalculated;
        public event Action GameRestarted;
        public event Action MovePassed;
        //*******************************************
        public GameProcessor()
        {
            board = new Board();
        }

        public List<List<Point>> GetPoints()
        {
            return board.Points;
        }

        public void UndoMove(List<List<Point>> pointsBeforeMove)
        {
            board = new Board(pointsBeforeMove);
            SwapTurn();
        }

        public List<Tuple<int, int>> GetAvailablePoints()
        {
            var availablePoints = BoardHandler.GetAvailablePoints(currentPlayerColor, board.Points);
            return availablePoints;
        }

        public bool IsGameFinished()
        {
            return BoardHandler.isFull(board.Points) || passedMovesCount == 2;
        }

        public bool IsFirstPlayerWon()
        {
            int firstPlayerPointsCount = BoardHandler.CountPoints(firstPlayerColor, board.Points);
            int secondPlayerPointsCount = BoardHandler.CountPoints(secondPlayerColor, board.Points);
            return firstPlayerPointsCount < secondPlayerPointsCount;
        }

        public void MakeMove(Tuple<int, int> coolds)
        {
            if (GetAvailablePoints().Count == 0)
            {
                Pass();
            }
            else
            {
                passedMovesCount = 0;
            }
            List<List<Point>> points = BoardHandler.SetPoint(currentPlayerColor, coolds, board.Points);
            MoveMade?.Invoke(points);
            SwapTurn();
            AvailablePointsCalculated?.Invoke(GetAvailablePoints());
            CalculatePlayersScore(points);
            if (BoardHandler.isFull(points) || passedMovesCount == 2)
            {
                FinishGame(points);
                return;
            }
            board = new Board(points);
        }
        public void RestartGame(Tuple<int, int> blackHoleCoords)
        {
            board = new Board();
            board.SetBlackHole(blackHoleCoords);
            GameRestarted?.Invoke();
        }

        public void StartGame(Tuple<int, int> blackHoleCoords)
        {
            currentPlayerColor = firstPlayerColor;

            board.SetBlackHole(blackHoleCoords);

            GameStarted?.Invoke(board.Points);

            var availablePoints = GetAvailablePoints();
            AvailablePointsCalculated?.Invoke(availablePoints);
        }

        public void StartGame(Tuple<int, int> blackHoleCoords, Tuple<int, int> firstMove)
        {
            currentPlayerColor = firstPlayerColor;

            board.SetBlackHole(blackHoleCoords);

            GameStarted?.Invoke(board.Points);

            var availablePoints = GetAvailablePoints();
            AvailablePointsCalculated?.Invoke(availablePoints);
            MakeMove(firstMove);
        }

        public void CalculatePlayersScore(List<List<Point>> points)
        {
            ScoresCalculated?.Invoke(BoardHandler.CountPoints(firstPlayerColor, points), BoardHandler.CountPoints(secondPlayerColor, points));
        }

        public void FinishGame(List<List<Point>> points)
        {
            int firstPlayerPointsCount = BoardHandler.CountPoints(firstPlayerColor, points);
            int secondPlayerPointsCount = BoardHandler.CountPoints(secondPlayerColor, points);
            GameFinished?.Invoke(firstPlayerPointsCount, secondPlayerPointsCount);
        }

        public void Pass()
        {
            passedMovesCount += 1;
            SwapTurn();
            MovePassed?.Invoke();
        }

        public void PassWithoutMassage()
        {
            passedMovesCount += 1;
            SwapTurn();
        }
        public void SwapTurn()
        {
            currentPlayerColor = currentPlayerColor == firstPlayerColor ? secondPlayerColor : firstPlayerColor;
        }
    }


    public static class BoardHandler
    {

        private static List<Tuple<int, int>> directions = new List<Tuple<int, int>>()
        {
             new Tuple<int, int>(-1, -1),
             new Tuple<int, int>(-1, 0),
             new Tuple<int, int>(-1, 1),
             new Tuple<int, int>(0, -1),
             new Tuple<int, int>(0, 1),
             new Tuple<int, int>(1, -1),
             new Tuple<int, int>(1, 0),
             new Tuple<int, int>(1, 1)
        };
        public static List<List<Point>> SetPoint(PointState playerColor, Tuple<int, int> coords, List<List<Point>> points)
        {
            List<List<Point>> pointsCopy = new List<List<Point>>(points);
            pointsCopy[coords.Item1][coords.Item2].State = playerColor;
            pointsCopy = UpdateBoard(playerColor, coords, pointsCopy);
            return pointsCopy;
        }

        private static List<List<Point>> UpdateBoard(PointState playerColor, Tuple<int, int> coords, List<List<Point>> points)
        {
            List<List<Point>> pointsCopy = new List<List<Point>>(points);
            int rowIndex = coords.Item1;
            int columnIndex = coords.Item2;

            foreach (var direction in directions)
            {
                if (IsInLine(playerColor, rowIndex, columnIndex, direction.Item1, direction.Item2, pointsCopy))
                    pointsCopy = PaintInLine(playerColor, rowIndex, columnIndex, direction.Item1, direction.Item2, pointsCopy);
            }
            return pointsCopy;
        }

        private static List<List<Point>> PaintInLine(PointState playerColor, int rowIndex, int columnIndex, int rowDiff, int columnDiff, List<List<Point>> points)
        {
            List<List<Point>> pointsCopy = new List<List<Point>>(points);
            rowIndex += rowDiff;
            columnIndex += columnDiff;
            while (IsPointInsideBoard(rowIndex, columnIndex, pointsCopy) && pointsCopy[rowIndex][columnIndex].State == GetOppositeColor(playerColor))
            {
                pointsCopy[rowIndex][columnIndex].State = playerColor;
                rowIndex += rowDiff;
                columnIndex += columnDiff;
            }
            return pointsCopy;
        }
        public static PointState GetOppositeColor(PointState color)
        {
            if (color == PointState.Black)
            {
                return PointState.White;
            }
            else if (color == PointState.White)
            {
                return PointState.Black;
            }
            return PointState.Empty;
        }

        public static List<Tuple<int, int>> GetAvailablePoints(PointState playerColor, List<List<Point>> points)
        {
            List<Tuple<int, int>> availablePoints = new List<Tuple<int, int>>();
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points[i].Count; j++)
                {
                    Point point = points[i][j];
                    if (point.State == PointState.Empty && IsPointAvailable(playerColor, i, j, points))
                    {
                        availablePoints.Add(new Tuple<int, int>(i, j));
                    }
                }
            }
            return availablePoints;
        }

        public static int CountPoints(PointState playerColor, List<List<Point>> points)
        {
            int count = 0;
            foreach (var row in points)
            {
                foreach (var point in row)
                {
                    if (point.State == playerColor)
                        count++;
                }
            }
            return count;
        }

        public static bool isFull(List<List<Point>> points)
        {
            foreach (var row in points)
            {
                foreach (var point in row)
                {
                    if (point.State == PointState.Empty)
                        return false;
                }
            }
            return true;
        }

        private static bool IsPointAvailable(PointState playerColor, int rowIndex, int columnIndex, List<List<Point>> points)
        {
            foreach (var direction in directions)
            {
                if (IsInLine(playerColor, rowIndex, columnIndex, direction.Item1, direction.Item2, points))
                    return true;
            }
            return false;
        }

        private static bool IsInLine(PointState playerColor, int rowIndex, int columnIndex, int rowDiff, int columnDiff, List<List<Point>> points)
        {
            rowIndex += rowDiff;
            columnIndex += columnDiff;
            int pointsInLine = 0;
            while (IsPointInsideBoard(rowIndex, columnIndex, points) && points[rowIndex][columnIndex].State == GetOppositeColor(playerColor))
            {
                pointsInLine += 1;
                rowIndex += rowDiff;
                columnIndex += columnDiff;
            }

            return (IsPointInsideBoard(rowIndex, columnIndex, points) && points[rowIndex][columnIndex].State == playerColor && pointsInLine > 0);
        }

        private static bool IsPointInsideBoard(int rowIndex, int columnIndex, List<List<Point>> points)
        {
            return (rowIndex >= 0 && columnIndex >= 0 && rowIndex < points.Count && columnIndex < points.Count);
        }
    }
}
