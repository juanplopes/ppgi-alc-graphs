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
using GraphSharp.Controls;
using QuickGraph;
using System.Globalization;
using System.IO;
using WPFExtensions.Controls;

namespace EigenThings
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            var files = Directory.GetFiles(".", "*.graph.txt").Select(x => Path.GetFileName(x));
            FileList.ItemsSource = files;
        }

        private void Relayout_Click(object sender, RoutedEventArgs e)
        {
            layout.Relayout();
        }

        private void CreateGraph_Click(object sender, RoutedEventArgs e)
        {
            var file = new GraphFile(GraphText.Text);
            DataContext = file.Graph;
            AdjacencyMatrix.Text = file.GetAdjacencyRepresentation();
            AdjacencyEigen.Text = file.GetAdjacencyEigenRepresentation();

            LaplacianMatrix.Text = file.GetLaplacianRepresentation();
            LaplacianEigen.Text = file.GetLaplacianEigenRepresentation();

            Information.Text = file.GetInformation();

            layout.Relayout();
        }

        private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GraphText.Text = File.ReadAllText(e.AddedItems.Cast<string>().First());
        }
    }
    public class MyGraphLayout : GraphLayout<string, IEdge<string>, IBidirectionalGraph<string, IEdge<string>>> { }
}
