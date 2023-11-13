﻿
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SPTC_APP.View.Controls
{
    /// <summary>
    /// This class is a Helper for Changing the Design of the Grid
    /// </summary>
    /// <typeparam name="T">This is for the specific Object Class that is to be displayed, Uses ToString() method for each parameter</typeparam>
public class DataGridHelper<T>
    {
        //private List<T> stack;

        public DataGridHelper(DataGrid dataGrid, List<ColumnConfiguration> columnConfigurations)
        {
            //this.stack = new List<T>();
            // Clear existing columns
            dataGrid.Columns.Clear();
            dataGrid.Focusable = true;
            dataGrid.IsReadOnly = true;
            dataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            dataGrid.AutoGenerateColumns = false;

            dataGrid.GridLinesVisibility = DataGridGridLinesVisibility.None;
           


            foreach (var config in columnConfigurations)
            {
                var column = new DataGridTextColumn
                {
                    Header = config.Header,
                    Binding = new System.Windows.Data.Binding(config.BindingPath),
                    Width = new DataGridLength(config.Width, DataGridLengthUnitType.Star),
                    MinWidth = config.Width,
                    MaxWidth = config.MaxWidth,
                    HeaderStyle = new Style
                    {
                        Setters =
                        {
                            new Setter(Control.FontWeightProperty, FontWeights.Bold),
                            new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center),
                            new Setter(Control.FontFamilyProperty, new FontFamily("Inter")),
                        }
                    },
                    
                    CellStyle = new System.Windows.Style
                    {
                        Setters =
                        {
                            new Setter(Control.BackgroundProperty, config.BackgroundColor),
                            new Setter(Control.ForegroundProperty, Brushes.Black),
                            new Setter(Control.FontWeightProperty, config.FontWeight),
                            new Setter(Control.HeightProperty, config.Height),
                            new Setter(Control.FontSizeProperty, config.FontSize),
                            new Setter(TextBlock.TextAlignmentProperty, config.Centralize? TextAlignment.Center: TextAlignment.Left),
                            new Setter(Control.FontFamilyProperty, new FontFamily("Inter")),
                            new Setter(Control.FontStretchProperty, config.Numeric? FontStretches.Expanded : FontStretches.Normal),
                            new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Center)
                        }
                    },
                    Visibility = config.Visibility 

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
        public bool Centralize { get; }
        public bool Numeric { get; }
        public Brush BackgroundColor { get; }
        public FontWeight FontWeight { get; }
        public Visibility Visibility { get; }

        public ColumnConfiguration(
            string bindingPath,
            string header,
            double minWidth = 100,
            double height = 30,
            double maxWidth = 200,
            double fontSize = 13,
            bool isCenter = false,
            bool isNumeric = false,
            Brush backgroundColor = null,
            FontWeight? fontWeight = null,
            Visibility visibility = Visibility.Visible) 
        {
            BindingPath = bindingPath;
            Header = header;
            Width = minWidth;
            Height = height;
            MaxWidth = maxWidth;
            FontSize = fontSize;
            Centralize = isCenter;
            Numeric = isNumeric;
            BackgroundColor = backgroundColor ?? Brushes.White;

            FontWeight = fontWeight.HasValue ? fontWeight.Value : FontWeights.SemiBold;

            Visibility = visibility;
        }
    }



}




