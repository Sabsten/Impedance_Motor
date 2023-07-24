using System;
using System.Threading;

class Program
{
    static char[][] canvas = new char[17][];
    static int centerX = 8;
    static int centerY = 8;

    public static void a()
    {
        for (int i = 0; i < 17; i++)
        {
            canvas[i] = new char[17];
            for (int j = 0; j < 17; j++)
            {
                canvas[i][j] = ' ';
            }
        }

        while (true)
        {
            for (int i = 0; i < 12; i++)
            {
                DrawCenter();
                DrawArm(i);
                DisplayCanvas();
                ClearCanvas();
                Thread.Sleep(500);
            }
        }
    }

    static void DrawCenter()
    {
        // Top and bottom
        for (int j = -4; j <= 4; j++)
        {
            canvas[centerY - 2][centerX + j] = '─';
            canvas[centerY + 2][centerX + j] = '─';
        }

        // Sides
        for (int i = -1; i <= 1; i++)
        {
            canvas[centerY + i][centerX - 4] = '│';
            canvas[centerY + i][centerX + 4] = '│';
        }

        // Circle
        canvas[centerY - 1][centerX - 3] = '/';
        canvas[centerY - 1][centerX + 3] = '\\';
        canvas[centerY + 1][centerX - 3] = '\\';
        canvas[centerY + 1][centerX + 3] = '/';

        // Center point
        canvas[centerY][centerX] = '·';
    }

    static void DrawArm(int position)
    {
        switch (position)
        {
            case 0:
                for (int i = 1; i <= 8; i++)
                {
                    canvas[centerY][centerX + i] = '-';
                }
                break;
            case 1:
                for (int i = 1; i <= 5; i++)
                {
                    canvas[centerY + i][centerX + i] = '\\';
                }
                break;
            case 2:
                for (int i = 1; i <= 5; i++)
                {
                    canvas[centerY + 3 + i][centerX + i] = '\\';
                }
                canvas[centerY + 2][centerX + 2] = '\\';
                canvas[centerY + 1][centerX + 1] = '\\';
                break;
            case 3:
                for (int i = 1; i <= 8; i++)
                {
                    canvas[centerY + i][centerX] = '|';
                }
                break;
            case 4:
                for (int i = 1; i <= 5; i++)
                {
                    canvas[centerY + 3 + i][centerX - i] = '/';
                }
                break;
            case 5:
                for (int i = 1; i <= 5; i++)
                {
                    canvas[centerY + i][centerX - i] = '/';
                }
                canvas[centerY + 1][centerX - 1] = '/';
                canvas[centerY + 2][centerX - 2] = '/';
                break;
            case 6:
                for (int i = 1; i <= 8; i++)
                {
                    canvas[centerY][centerX - i] = '-';
                }
                break;
            case 7:
                for (int i = 1; i <= 5; i++)
                {
                    canvas[centerY - i][centerX - i] = '\\';
                }
                break;
            case 8:
                for (int i = 1; i <= 5; i++)
                {
                    canvas[centerY - 3 - i][centerX - i] = '\\';
                }
                canvas[centerY - 1][centerX - 1] = '\\';
                canvas[centerY - 2][centerX - 2] = '\\';
                break;
            case 9:
                for (int i = 1; i <= 8; i++)
                {
                    canvas[centerY - i][centerX] = '|';
                }
                break;
            case 10:
                for (int i = 1; i <= 5; i++)
                {
                    canvas[centerY - 3 - i][centerX + i] = '/';
                }
                break;
            case 11:
                for (int i = 1; i <= 5; i++)
                {
                    canvas[centerY - i][centerX + i] = '/';
                }
                canvas[centerY - 1][centerX + 1] = '/';
                canvas[centerY - 2][centerX + 2] = '/';
                break;
            case 12:
                for (int i = 1; i <= 8; i++)
                {
                    canvas[centerY][centerX + i] = '-';
                }
                break;
        }
    }

    static void DisplayCanvas()
    {
        Console.Clear();
        for (int i = 0; i < 17; i++)
        {
            for (int j = 0; j < 17; j++)
            {
                Console.Write(canvas[i][j]);
            }
            Console.WriteLine();
        }
    }

    static void ClearCanvas()
    {
        for (int i = 0; i < 17; i++)
        {
            for (int j = 0; j < 17; j++)
            {
                canvas[i][j] = ' ';
            }
        }
    }
}
