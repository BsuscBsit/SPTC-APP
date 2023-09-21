
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace SPTC_APP.View.Controls
{
    /// <summary>
    /// This class is a Helper for Changing the Design of the Grid
    /// </summary>
    /// <typeparam name="T">This is for the specific Object Class that is to be displayed, Uses ToString() method for each parameter</typeparam>
public class DataGridHelper<T>
    {
        private DataGrid dataGrid;
        private List<T> stack;

        public DataGridHelper(DataGrid dataGrid, List<ColumnConfiguration> columnConfigurations)
        {
            this.dataGrid = dataGrid;
            this.stack = new List<T>();
            // Clear existing columns
            dataGrid.Columns.Clear();
            dataGrid.Focusable = true;
            dataGrid.IsReadOnly = true;
            dataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            dataGrid.AutoGenerateColumns = false;
            foreach (var config in columnConfigurations)
            {
                var column = new DataGridTextColumn
                {
                    Header = config.Header,
                    Binding = new System.Windows.Data.Binding(config.BindingPath),
                    Width = config.Width,
                    MaxWidth = config.MaxWidth,
                    CellStyle = new System.Windows.Style
                    {
                        Setters =
            {
                new Setter(System.Windows.Controls.Control.BackgroundProperty, config.BackgroundColor),
                new Setter(System.Windows.Controls.Control.FontWeightProperty, config.FontWeight),
                new Setter(System.Windows.Controls.Control.FontSizeProperty, config.FontSize),
                new Setter(System.Windows.Controls.Control.HeightProperty, config.Height)
            }
                    },
                    Visibility = config.Visibility // Apply the visibility setting
                };

                dataGrid.Columns.Add(column);
            }
        }

    }

    /// <summary>
    /// This is for desiging each Field of the Opject
    /// </summary>
    public class ColumnConfiguration
    {
        public string BindingPath { get; }
        public string Header { get; }
        public double Width { get; }
        public double Height { get; }
        public double MaxWidth { get; }
        public double FontSize { get; }
        public System.Windows.Media.Brush BackgroundColor { get; }
        public System.Windows.FontWeight FontWeight { get; }
        public Visibility Visibility { get; } // New property for visibility

        public ColumnConfiguration(
            string bindingPath,
            string header,
            double width = 100,
            double height = 20,
            double maxWidth = 200,
            double fontSize = 10,
            System.Windows.Media.Brush backgroundColor = null,
            System.Windows.FontWeight? fontWeight = null,
            Visibility visibility = Visibility.Visible) // Default visibility is Visible
        {
            BindingPath = bindingPath;
            Header = header;
            Width = width;
            Height = height;
            MaxWidth = maxWidth;
            FontSize = fontSize;
            BackgroundColor = backgroundColor ?? System.Windows.Media.Brushes.LightGray;

            // Check if fontWeight is provided, otherwise use the default value
            FontWeight = fontWeight.HasValue ? fontWeight.Value : System.Windows.FontWeights.Normal;

            Visibility = visibility;
        }
    }



}




