namespace MaxSumSubmatrix;

/// <summary>
/// Находит прямоугольную подматрицу с максимально возможной суммой.
/// Использует сведение задачи к одномерным проходам алгоритма Кадане со сложностью O(columns^2 * rows).
/// </summary>
public static class MaximumSumSubmatrixFinder
{
    public static SubmatrixResult Find(MatrixData matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);

        double bestSum = double.NegativeInfinity;
        int bestTop = 0;
        int bestLeft = 0;
        int bestBottom = 0;
        int bestRight = 0;
        double[] accumulatedRows = new double[matrix.RowCount];

        for (int leftColumn = 0; leftColumn < matrix.ColumnCount; leftColumn++)
        {
            Array.Clear(accumulatedRows, 0, accumulatedRows.Length);

            for (int rightColumn = leftColumn; rightColumn < matrix.ColumnCount; rightColumn++)
            {
                for (int rowIndex = 0; rowIndex < matrix.RowCount; rowIndex++)
                {
                    accumulatedRows[rowIndex] += matrix.GetValue(rowIndex, rightColumn);
                }

                (double currentSum, int currentTop, int currentBottom) =
                    FindMaximumSumSubarray(accumulatedRows);

                if (currentSum > bestSum)
                {
                    bestSum = currentSum;
                    bestTop = currentTop;
                    bestLeft = leftColumn;
                    bestBottom = currentBottom;
                    bestRight = rightColumn;
                }
            }
        }

        return new SubmatrixResult(bestTop, bestLeft, bestBottom, bestRight, bestSum);
    }

    private static (double Sum, int StartIndex, int EndIndex) FindMaximumSumSubarray(double[] values)
    {
        double bestSum = values[0];
        double currentSum = values[0];
        int bestStart = 0;
        int bestEnd = 0;
        int currentStart = 0;

        for (int index = 1; index < values.Length; index++)
        {
            double extendedSum = currentSum + values[index];
            if (values[index] > extendedSum)
            {
                currentSum = values[index];
                currentStart = index;
            }
            else
            {
                currentSum = extendedSum;
            }

            if (currentSum > bestSum)
            {
                bestSum = currentSum;
                bestStart = currentStart;
                bestEnd = index;
            }
        }

        return (bestSum, bestStart, bestEnd);
    }
}
