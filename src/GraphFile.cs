using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;
using System.Text.RegularExpressions;
using DotNetMatrix;

namespace EigenThings
{
    public class GraphFile
    {
        public string FileContent { get; set; }
        public BidirectionalGraph<string, IEdge<string>> Graph { get; protected set; }
        public GeneralMatrix AdjacencyMatrix { get; protected set; }
        public GeneralMatrix LaplacianMatrix { get; protected set; }
        public int Vertices { get; protected set; }

        public GraphFile(string fileContent)
        {
            FileContent = fileContent;
            InitializeGraph();
        }

        protected IEnumerable<int> ReadFile()
        {
            var matches = Regex.Matches(FileContent, @"\d");
            return matches.Cast<Match>().Where(x => x.Success).Select(x => int.Parse(x.Value));
        }

        public string GetAdjacencyRepresentation()
        {
            return RepresentateMatrix(AdjacencyMatrix);
        }

        public string GetLaplacianRepresentation()
        {
            return RepresentateMatrix(LaplacianMatrix);
        }

        public string GetAdjacencyEigenRepresentation()
        {
            return RepresentateEigen(AdjacencyMatrix);
        }

        public string GetLaplacianEigenRepresentation()
        {
            return RepresentateEigen(LaplacianMatrix);
        }


        private string RepresentateEigen(GeneralMatrix matrix)
        {
            var builder = new StringBuilder();
            var eigen = new EigenvalueDecomposition(matrix);

            foreach (var value in eigen.RealEigenvalues.OrderByDescending(x => x).OrderByDescending(x => x))
            {
                builder.Append(value.ToString("0.00 "));
            }
            return builder.ToString();
        }

        private string RepresentateMatrix(GeneralMatrix matrix)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < Vertices; i++)
            {
                for (int j = 0; j < Vertices; j++)
                {
                    builder.Append(matrix.GetElement(i, j).ToString("0.00 "));
                }
                builder.Append("\n");
            }

            return builder.ToString();
        }

        protected void InitializeGraph()
        {
            var file = ReadFile().ToArray();
            int vertices = file.FirstOrDefault();

            Vertices = vertices;
            Graph = new BidirectionalGraph<string, IEdge<string>>();
            AdjacencyMatrix = new GeneralMatrix(vertices, vertices);
            LaplacianMatrix = new GeneralMatrix(vertices, vertices);

            foreach (var i in Enumerable.Range(1, vertices))
                Graph.AddVertex(i.ToString());

            for (int i = 1; i + 1 < file.Length; i += 2)
            {
                var start = file[i];
                var end = file[i + 1];

                if (start == end) continue;
                AdjacencyMatrix.SetElement(start - 1, end - 1, 1);
                AdjacencyMatrix.SetElement(end - 1, start - 1, 1);

                LaplacianMatrix.SetElement(start - 1, end - 1, -1);
                LaplacianMatrix.SetElement(end - 1, start - 1, -1);

                Graph.AddEdge(new Edge<string>(start.ToString(), end.ToString()));
                Graph.AddEdge(new Edge<string>(end.ToString(), start.ToString()));
            }

            for (int i = 0; i < vertices; i++)
            {
                var count = 0;
                for (int j = 0; j < vertices; j++)
                {
                    count += (int)AdjacencyMatrix.GetElement(i, j);
                }
                LaplacianMatrix.SetElement(i, i, count);
            }


        }
    }
}
