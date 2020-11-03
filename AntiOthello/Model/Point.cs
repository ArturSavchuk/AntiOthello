using System;

namespace AntiOthello.Model
{
    public enum PointState
    {
        Black,
        White,
        Empty,
        BlackHole
    }
    public class Point
    {
        public PointState State { get; set; }
        public void ChangeColor()
        {
            if (State == PointState.Black)
                State = PointState.White;
            else if (State == PointState.White)
                State = PointState.Black;
        }

        public Point(PointState state)
        {
            this.State = state;
        }
    }
}
