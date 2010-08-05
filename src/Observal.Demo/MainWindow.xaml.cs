﻿using System.Windows;

namespace Observal.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var sampleEmployees =
                new Employee("Ryan Howard", 200000,
                    new Employee("Michael Scott", 130000,
                        new Employee("Dwight Schrute", 80000),
                        new Employee("Jim Halpert", 80000,
                            new Employee("Andy Bernard", 75000,
                                new Employee("Stanley Hudson", 70000),
                                new Employee("Phyllis Lapin", 70000)))));

            DataContext = new OrgChartViewModel(new[] { sampleEmployees });
        }
    }
}
