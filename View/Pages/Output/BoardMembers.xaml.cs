using MySqlX.XDevAPI.Relational;
using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Pages.Input;
using SPTC_APP.View.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static SPTC_APP.View.Pages.Output.BoardMembers;

namespace SPTC_APP.View.Pages.Output
{
    /// <summary>
    /// Interaction logic for BoardMembers.xaml
    /// </summary>
    public partial class BoardMembers : Window
    {
        public BoardMembers()
        {
            InitializeComponent();
            int row = 7;

            Dictionary<string, int> positionMap = new Dictionary<string, int>
            {
                { AppState.ALL_EMPLOYEES[0], 2 },
                { AppState.ALL_EMPLOYEES[1], 3 },
                { AppState.ALL_EMPLOYEES[2], 4 },
                { AppState.ALL_EMPLOYEES[3], 5 },
                { AppState.ALL_EMPLOYEES[4], 0 }
            };
            foreach (Employee e in Retrieve.GetDataUsingQuery<Employee>(RequestQuery.GET_ALL_EMPLOYEES))
            {
                Member member;
                int position;
                if (e.position?.title != null && positionMap.ContainsKey(e.position.title))
                {
                    position = positionMap[e.position.title];
                    member = new Member(e, position, position == 0);
                }
                else
                {
                    position = row;
                    member = new Member(e, position);
                    row += 1;
                }

                member.DrawCard(memGrid);
            }
        }
        public async Task<Grid> Fetch()
        {
            await Task.Delay(1);
            if (gridBoardMem.Parent != null)
            {
                Window currentParent = gridBoardMem.Parent as Window;
                if (currentParent != null)
                {
                    currentParent.Content = null;
                }
            }
            this.Close();
            return gridBoardMem;
        }
        public class Member
        {
            private Employee employee;
            private int row;
            private Button btnEdit;
            private bool isManage = false;

            private int imageRatio = 70;
            private int nameFontSize = 20;
            private int positionFontSize = 16;
            private Thickness cellSpace = new Thickness(25, 10, 25, 10);
            private Brush bg = Brushes.White;

            public Member(Employee employee, int row, bool isManage = false)
            {
                this.employee = employee;
                //this.col = col;
                this.row = row;
                this.isManage = isManage;
            }


            public void DrawCard(Grid grid)
            {
                // ~boku no...
                Grid userContainerView = new Grid
                {
                    Margin = cellSpace
                };
                Grid.SetRow(userContainerView, this.row);

                Border effectBorder = new Border
                {
                    Background = bg,
                    CornerRadius = new CornerRadius(12),
                    Effect = new DropShadowEffect
                    {
                        BlurRadius = 10,
                        Opacity = 0.2,
                        ShadowDepth = 0
                    }
                };
                Border contentBorder = new Border
                {
                    Background = bg,
                    CornerRadius = new CornerRadius(10)
                };

                Grid contentGrid = new Grid
                {
                    MinWidth = 500,
                    Margin = new Thickness(15)
                };

                contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                contentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                contentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                Ellipse effectEllipse = new Ellipse
                {
                    Width = imageRatio,
                    Height = imageRatio,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Fill = Brushes.White,
                    Effect = new DropShadowEffect
                    {
                        BlurRadius = 15,
                        Direction = -90,
                        Opacity = 0.2,

                        ShadowDepth = 5
                    }
                };

                contentGrid.Children.Add(effectEllipse);
                Grid.SetColumn(effectEllipse, 0);
                Grid.SetRow(effectEllipse, 0);
                Grid.SetRowSpan(effectEllipse, 2);

                Ellipse profileEllipse = new Ellipse
                {
                    Width = imageRatio,
                    Height = imageRatio,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Stroke = (Brush)Application.Current.Resources["BrushGrey"]
                };

                if (employee.image?.GetSource() != null)
                {
                    profileEllipse.Fill = new ImageBrush(employee.image.GetSource());
                }
                else
                {
                    profileEllipse.Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/SPTC APP;component/View/Images/icons/person.png")));
                }

                contentGrid.Children.Add(profileEllipse);
                Grid.SetColumn(profileEllipse, 0);
                Grid.SetRow(profileEllipse, 0);
                Grid.SetRowSpan(profileEllipse, 2);

                Label lblName = new Label
                {
                    Content = employee.name?.lastname?.ToString() ?? "Unknown(No Name Found)",
                    FontFamily = new FontFamily("Inter"),
                    FontSize = nameFontSize,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(15, 0, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom
                };

                contentGrid.Children.Add(lblName);
                Grid.SetColumn(lblName, 1);
                Grid.SetRow(lblName, 0);

                Label lblPosition = new Label
                {
                    Content = employee.position?.title?.ToString() ?? "(Position Not Assigned)",
                    FontFamily = new FontFamily("Inter"),
                    FontSize = positionFontSize,
                    FontWeight = FontWeights.SemiBold,
                    Margin = new Thickness(15, 0, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };

                contentGrid.Children.Add(lblPosition);
                Grid.SetColumn(lblPosition, 1);
                Grid.SetRow(lblPosition, 1);

                if ((AppState.USER?.position?.accesses[17] ?? false))
                {
                    Button editButton = new Button
                    {
                        Width = 120,
                        Height = 30,
                        Content = isManage ? ((employee.position.title == AppState.USER.position.title) ? "PROFILE" : "MANAGE") : "EDIT",
                        Style = (Style)Application.Current.FindResource("ControlledButtonStyle"),
                        Margin = new Thickness(15, 0, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Visibility = Visibility.Hidden
                    };
                    editButton.Click += EditClick;
                    contentGrid.Children.Add(editButton);
                    Grid.SetColumn(editButton, 2);
                    Grid.SetRowSpan(editButton, 2);

                    this.btnEdit = editButton;
                    userContainerView.MouseEnter += MemberMouseEnter;
                    userContainerView.MouseLeave += MemberMouseLeave;
                }

                contentBorder.Child = contentGrid;

                userContainerView.Children.Add(effectBorder);
                userContainerView.Children.Add(contentBorder);
                grid.Children.Add(userContainerView);
            }

            private void EditClick(object sender, RoutedEventArgs e)
            {
                string noname = "???";
                if (isManage)
                {
                    if (AppState.IS_ADMIN)
                    {
                        int result = ControlWindow.ShowThreway("Manage Employee", $"Employee name: {employee.name?.legalName ?? noname}", "EDIT", "CHANGE", Icons.NOTIFY);
                        if (result == 0)
                        {
                            (new EditEmployee(employee, true, true)).Show();
                        }
                        else if (result == 1)
                        {
                            (new EditEmployee(employee, false, true)).Show();
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (employee.position.title == AppState.USER.position.title)
                        {
                            (new EditEmployee(employee, true, false)).Show();
                        }
                        else
                        {
                            ControlWindow.ShowStatic("Manage Employee", "Cannot Edit Employee Information.\nRequest Admin assistance.", Icons.ERROR);
                        }
                    }
                }
                else
                {
                    if (ControlWindow.ShowTwoway("Edit Profile", $"Board Member: {employee.name?.legalName ?? noname}", Icons.NOTIFY))
                    {
                        (new EditEmployee(employee, true, false)).Show();
                    }
                }
            }

            private void MemberMouseEnter(object sender, MouseEventArgs e)
            {
                if (sender is Grid grid)
                {
                    btnEdit.FadeIn(0.3);
                }
            }

            private void MemberMouseLeave(object sender, MouseEventArgs e)
            {
                if (sender is Grid grid)
                {
                    btnEdit.FadeOut(0.3);
                }
            }


        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            (new EditEmployee(new Employee(), false, false)).Show();
        }
    }
}
