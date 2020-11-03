using System;
using System.Collections;
using System.Collections.Generic;

namespace AntiOthello.Model
{
    public class Board
    {
        public List<List<Point>> Points { get; private set; }

        public Board()
        {
            int size = 8;
            Points = new List<List<Point>>(size);
            for (int i = 0; i < size; i++)
            {
                Points.Add(new List<Point>(size));
                for (int j = 0; j < size; j++)
                {
                    Points[i].Add(new Point(PointState.Empty));
                }
            }
            SetInitialBoardState();
        }

        public Board(List<List<Point>> points)
        {
            Points = points;
        }

        public void SetBlackHole(Tuple<int, int> coords)
        {
            Points[coords.Item1][coords.Item2].State = PointState.BlackHole;
        }

        private void SetInitialBoardState()
        {
            Points[3][3].State = PointState.White;
            Points[4][4].State = PointState.White;
            Points[3][4].State = PointState.Black;
            Points[4][3].State = PointState.Black;
        }
    }
}
