namespace Craft.Algorithms
{
    // Algorithm described here: https://perso.ensta-paris.fr/~manzaner/Download/IAD/Shih_Wu_04.pdf
    // The algorithm operates by passing the 4 different structuring elements illustrated below over the
    // image (o indicates the central pixel and x a neighbor)
    //
    // 1) xxx  2)  xo  3) xo   4) ox
    //     ox             xxx
    //
    // The first structuring element starts at the upper right corner of the image
    // and processes the first row from right to left. Then the second (smaller) structuring element
    // processes the same (first) row from left to right. These two structuring elements then proceeds
    // with row 2 and so on.
    // Structuring elements 3 and 4 operate in a similar manner, but starts at the lower left corner of
    // the image
    // The structuring elements compute the vectors that point from a background pixel to the closest
    // object pixel in the image.
    // When the structuring elements are done processing the image, the distance image itself can be
    // computed simply as the length of the distance vectors
    public static class DistanceTransform
    {
        public static void EuclideanDistanceTransform(
            int[,] input,
            out double[,] distances,
            out int[,] xValues,
            out int[,] yValues)
        {
            var rows = input.GetLength(0);
            var cols = input.GetLength(1);

            xValues = new int[rows, cols];
            yValues = new int[rows, cols];

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    if (input[r, c] > 0)
                    {
                        xValues[r, c] = 0;
                        yValues[r, c] = 0;
                    }
                    else
                    {
                        xValues[r, c] = 9999;
                        yValues[r, c] = 9999;
                    }
                }
            }

            var incrX = new int[5];
            var incrY = new int[5];

            // First pass
            for (var r = 0; r < rows; r++)
            {
                // Forward scan
                for (var c = cols - 1; c >= 0; c--)
                {
                    if (xValues[r, c] == 0 && yValues[r, c] == 0)
                    {
                        continue;
                    }

                    if (r == 0 || c == 0)
                    {
                        incrX[0] = 9999;
                        incrY[0] = 9999;
                    }
                    else
                    {
                        incrX[0] = xValues[r - 1, c - 1] - 1;
                        incrY[0] = yValues[r - 1, c - 1] - 1;
                    }

                    if (r == 0)
                    {
                        incrX[1] = 9999;
                        incrY[1] = 9999;
                    }
                    else
                    {
                        incrX[1] = xValues[r - 1, c];
                        incrY[1] = yValues[r - 1, c] - 1;
                    }

                    if (r == 0 || c == cols - 1)
                    {
                        incrX[2] = 9999;
                        incrY[2] = 9999;
                    }
                    else
                    {
                        incrX[2] = xValues[r - 1, c + 1] + 1;
                        incrY[2] = yValues[r - 1, c + 1] - 1;
                    }

                    if (c == cols - 1)
                    {
                        incrX[3] = 9999;
                        incrY[3] = 9999;
                    }
                    else
                    {
                        incrX[3] = xValues[r, c + 1] + 1;
                        incrY[3] = yValues[r, c + 1];
                    }

                    incrX[4] = xValues[r, c];
                    incrY[4] = yValues[r, c];

                    var iOpt = 0;
                    var sqrDistMin = int.MaxValue;

                    for (var i = 0; i < 5; i++)
                    {
                        var sqrDist = incrY[i] * incrY[i] + incrX[i] * incrX[i];

                        if (sqrDist >= sqrDistMin) continue;

                        sqrDistMin = sqrDist;
                        iOpt = i;
                    }

                    xValues[r, c] = incrX[iOpt];
                    yValues[r, c] = incrY[iOpt];
                }

                // Backward scan
                for (var c = 0; c < cols; c++)
                {
                    if (xValues[r, c] == 0 && yValues[r, c] == 0)
                    {
                        continue;
                    }

                    if (c == 0)
                    {
                        incrX[0] = 9999;
                        incrY[0] = 9999;
                    }
                    else
                    {
                        incrX[0] = xValues[r, c - 1] - 1;
                        incrY[0] = yValues[r, c - 1];
                    }

                    incrX[1] = xValues[r, c];
                    incrY[1] = yValues[r, c];

                    var iOpt = 0;
                    var sqrDistMin = int.MaxValue;

                    for (var i = 0; i < 2; i++)
                    {
                        var sqrDist = incrY[i] * incrY[i] + incrX[i] * incrX[i];

                        if (sqrDist >= sqrDistMin) continue;

                        sqrDistMin = sqrDist;
                        iOpt = i;
                    }

                    xValues[r, c] = incrX[iOpt];
                    yValues[r, c] = incrY[iOpt];
                }
            }

            // Second and final pass
            for (var r = rows - 1; r >= 0; r--)
            {
                // Forward scan
                for (var c = 0; c < cols; c++)
                {
                    if (xValues[r, c] == 0 && yValues[r, c] == 0)
                    {
                        continue;
                    }

                    if (r == rows - 1 || c == cols - 1)
                    {
                        incrX[0] = 9999;
                        incrY[0] = 9999;
                    }
                    else
                    {
                        incrX[0] = xValues[r + 1, c + 1] + 1;
                        incrY[0] = yValues[r + 1, c + 1] + 1;
                    }

                    if (r == rows - 1)
                    {
                        incrX[1] = 9999;
                        incrY[1] = 9999;
                    }
                    else
                    {
                        incrX[1] = xValues[r + 1, c];
                        incrY[1] = yValues[r + 1, c] + 1;
                    }

                    if (r == rows - 1 || c == 0)
                    {
                        incrX[2] = 9999;
                        incrY[2] = 9999;
                    }
                    else
                    {
                        incrX[2] = xValues[r + 1, c - 1] - 1;
                        incrY[2] = yValues[r + 1, c - 1] + 1;
                    }

                    if (c == 0)
                    {
                        incrX[3] = 9999;
                        incrY[3] = 9999;
                    }
                    else
                    {
                        incrX[3] = xValues[r, c - 1] - 1;
                        incrY[3] = yValues[r, c - 1];
                    }

                    incrX[4] = xValues[r, c];
                    incrY[4] = yValues[r, c];

                    var iOpt = 0;
                    var sqrDistMin = int.MaxValue;

                    for (var i = 0; i < 5; i++)
                    {
                        var sqrDist = incrY[i] * incrY[i] + incrX[i] * incrX[i];

                        if (sqrDist >= sqrDistMin) continue;

                        sqrDistMin = sqrDist;
                        iOpt = i;
                    }

                    xValues[r, c] = incrX[iOpt];
                    yValues[r, c] = incrY[iOpt];
                }

                // Backward scan
                for (var c = cols - 1; c >= 0; c--)
                {
                    if (xValues[r, c] == 0 && yValues[r, c] == 0)
                    {
                        continue;
                    }

                    if (c == cols - 1)
                    {
                        incrX[0] = 9999;
                        incrY[0] = 9999;
                    }
                    else
                    {
                        incrX[0] = xValues[r, c + 1] + 1;
                        incrY[0] = yValues[r, c + 1];
                    }

                    incrX[1] = xValues[r, c];
                    incrY[1] = yValues[r, c];

                    var iOpt = 0;
                    var sqrDistMin = int.MaxValue;

                    for (var i = 0; i < 2; i++)
                    {
                        var sqrDist = incrY[i] * incrY[i] + incrX[i] * incrX[i];

                        if (sqrDist >= sqrDistMin) continue;

                        sqrDistMin = sqrDist;
                        iOpt = i;
                    }

                    xValues[r, c] = incrX[iOpt];
                    yValues[r, c] = incrY[iOpt];
                }
            }

            distances = new double[rows, cols];

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    var dx = xValues[r, c];
                    var dy = yValues[r, c];

                    distances[r, c] = System.Math.Sqrt(dx * dx + dy * dy);
                }
            }
        }
    }
}
