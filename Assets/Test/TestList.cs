using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TestList
{
    public struct Point2
    {
        public int x;
        public int y;

        public Point2(int x = 0,int y = 0)
        {
            this.x = x;
            this.y = y;
        }

        public void Set(int x,int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public static int[,] pieceArray { get; private set; } = new int[19, 31];

    public static void Enter(Point2 point)
    {
        pieceArray[point.x, point.y] += 1;
    }
	
}
