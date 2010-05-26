using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphSharp.Controls;
using QuickGraph;
using CSML;
using System.Globalization;

namespace EigenThings
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Main : Window
    {
        private BidirectionalGraph<string, IEdge<string>> graph;

        public Main()
        {
            InitializeComponent();
            graph = new BidirectionalGraph<string, IEdge<string>>();
            DataContext = graph;
        }

        private void Relayout_Click(object sender, RoutedEventArgs e)
        {
            layout.Relayout();
        }

        private void CreateGraph_Click(object sender, RoutedEventArgs e)
        {
            DataContext = graph = new BidirectionalGraph<string, IEdge<string>>();
            var m = new Matrix(GraphText.Text);
            
            foreach(var number in Enumerable.Range(1, m.ColumnCount))
                graph.AddVertex(number.ToString());

            for (int i = 1; i <= m.RowCount; i++)
            {
                for (int j = 1; j <= m.ColumnCount; j++)
                {
                    if (m[i,j] != 0)
                    {
                        m[i,j] = new Complex(1);
                        graph.AddEdge(new Edge<string>(i.ToString(), j.ToString()));
                    }
                }
            }


            Eigenvalues.Content = string.Join(", ", EigenvalueList(m).OrderByDescending(x=>x).Select(x => x.ToString("0.00", CultureInfo.InvariantCulture)).ToArray());

        }

        private IEnumerable<double> EigenvalueList(Matrix m)
        {
            for (int i = 1; i <= m.RowCount; i++)
            {
                yield return m[i,1].Re;
            }
        }
    }
    public class MyGraphLayout : GraphLayout<string, IEdge<string>, IBidirectionalGraph<string, IEdge<string>>> { }
}
