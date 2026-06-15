using System;

namespace ScriptsToTest
{
    public static class HungarianAlgorithm
    {
        public static int[] FindAssignments(int[,] costs)
        {
            if (costs == null) throw new ArgumentNullException(nameof(costs));

            int rows = costs.GetLength(0);
            int cols = costs.GetLength(1);
            
            bool transposed = rows > cols;
            int[,] matrix = transposed ? Transpose(costs) : costs;

            int n = matrix.GetLength(0); 
            int m = matrix.GetLength(1); 
            
            int[] u = new int[n + 1]; // potential for rows
            int[] v = new int[m + 1]; // potential for columns
            
            int[] p = new int[m + 1]; // row assigned to column j
            
            int[] way = new int[m + 1]; // alternating path for backtracking

            // Find a valid assignment for each row
            for (int i = 1; i <= n; i++)
            {
                p[0] = i; // Current row  in the dummy column 0
                int j0 = 0; // Tracks current visiting column
                
                int[] minv = new int[m + 1];
                for (int j = 0; j <= m; j++) minv[j] = int.MaxValue;

                // Visited columns
                bool[] used = new bool[m + 1];
                
                do
                {
                    used[j0] = true; 
                    int i0 = p[j0]; 
                    int delta = int.MaxValue;
                    int j1 = 0; 

                    for (int j = 1; j <= m; j++)
                    {
                        if (!used[j])
                        {
                            int cur = matrix[i0 - 1, j - 1] - u[i0] - v[j];
                            
                            if (cur < minv[j])
                            {
                                minv[j] = cur;
                                way[j] = j0; 
                            }

                            if (minv[j] < delta)
                            {
                                delta = minv[j];
                                j1 = j;
                            }
                        }
                    }
                    
                    for (int j = 0; j <= m; j++)
                    {
                        if (used[j])
                        {
                            u[p[j]] += delta; 
                            v[j] -= delta; 
                        }
                        else
                        {
                            minv[j] -= delta;
                        }
                    }
                    
                    j0 = j1;
                } while (p[j0] != 0);
                
                do
                {
                    int j1 = way[j0];
                    p[j0] = p[j1]; 
                    j0 = j1;
                } while (j0 != 0);
            }
            
            int[] assignment = new int[n];
            for (int j = 1; j <= m; j++)
            {
                if (p[j] != 0) 
                {
                    assignment[p[j] - 1] = j - 1;
                }
            }

            
            if (transposed)
            {
                int[] originalAssignment = new int[rows];
                for (int i = 0; i < rows; i++) originalAssignment[i] = -1; // -1 represents Unassigned

                for (int i = 0; i < n; i++)
                {
                    originalAssignment[assignment[i]] = i;
                }

                return originalAssignment;
            }

            return assignment;
        }
        
        private static int[,] Transpose(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int[,] transposed = new int[cols, rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    transposed[j, i] = matrix[i, j];
                }
            }

            return transposed;
        }
    }
}