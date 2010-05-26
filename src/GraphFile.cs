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

        public EigenvalueDecomposition AdjacencyEigen { get; protected set; }
        public EigenvalueDecomposition LaplacianEigen { get; protected set; }

        public int Vertices { get; protected set; }



        public GraphFile(string fileContent)
        {
            FileContent = fileContent;
            InitializeGraph();
        }

        protected IEnumerable<int> ReadFile()
        {
            var matches = Regex.Matches(FileContent, @"\d+");
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

        public string GetLaplacianEigenVectorRepresentation()
        {
            return RepresentateMatrix(LaplacianEigen.GetV());
        }

        public string GetAdjacencyEigenRepresentation()
        {
            return RepresentateEigen(AdjacencyEigen);
        }

        public string GetLaplacianEigenRepresentation()
        {
            return RepresentateEigen(LaplacianEigen);
        }

        public string GetInformation()
        {
            var asvd = AdjacencyMatrix.SVD();
            var matrix1 = AdjacencyMatrix.Multiply(AdjacencyMatrix).Multiply(AdjacencyMatrix);
            var tris = (Enumerable.Range(0, Vertices).Sum(x => matrix1.GetElement(x, x)) / 6).ToString("0");

            var eigen = LaplacianEigen;
            var components = eigen.RealEigenvalues.Where(x => Math.Abs(x) < 1e-5f).Count();
            var spanningTrees = eigen.RealEigenvalues.Where(x => Math.Abs(x) > 1e-5f).Aggregate(1.0, (x, y) => x * y) / Vertices;

            var adjEigen = AdjacencyEigen;
            bool isBipartite = true;
            for (int i = 0; i < Math.Ceiling(adjEigen.RealEigenvalues.Length/2.0); i++)
            {
                isBipartite = Math.Abs((adjEigen.RealEigenvalues[i] + adjEigen.RealEigenvalues[adjEigen.RealEigenvalues.Length - i - 1])) < 1e-5f;
            }

            var builder = new StringBuilder();
            builder.AppendFormat("Número de componentes: {0}\n", components);
            builder.AppendFormat("Conectividade algébrica: {0:0.00}\n", eigen.RealEigenvalues.OrderBy(x => x).Skip(1).FirstOrDefault());
            builder.AppendFormat("Número de triângulos: {0}\n", tris);
            if (components == 1)
                builder.AppendFormat("Grafo conexo: árvores geradoras: {0:0}\n", spanningTrees);
            else
                builder.AppendFormat("Grafo não-conexo\n");

            if (isBipartite)
                builder.AppendFormat("Grafo bipartido\n");
            else
                builder.AppendFormat("Grafo não-bipartido\n");

            return builder.ToString();
        }

        private string RepresentateEigen(EigenvalueDecomposition eigen)
        {
            var builder = new StringBuilder();

            foreach (var value in eigen.RealEigenvalues)
            {
                builder.Append(value.ToString(" 0.00 ;-0.00 "));
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
                    builder.Append(matrix.GetElement(i, j).ToString(" 0.00 ;-0.00 "));
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

            LaplacianEigen = LaplacianMatrix.Eigen();
            AdjacencyEigen = AdjacencyMatrix.Eigen();
        }
    }
}
