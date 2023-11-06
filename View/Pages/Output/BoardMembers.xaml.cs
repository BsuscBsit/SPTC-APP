using SPTC_APP.Database;
using SPTC_APP.Objects;
using SPTC_APP.View.Pages.Input;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            int col = 0;
            int row = 4;
            foreach(Employee e in Retrieve.GetDataUsingQuery<Employee>(RequestQuery.GET_ALL_EMPLOYEES))
            {
                //GET CHAIRMAN
                if(e.position?.title == AppState.ALL_EMPLOYEES[4])
                {
                    Member member = new Member(e, 2, 0);
                    member.DrawCard(memGrid);
                } 
                else if (e.position?.title == AppState.ALL_EMPLOYEES[0])
                {
                    Member member = new Member(e, 0, 2, true);
                    member.DrawCard(memGrid);
                }
                else if (e.position?.title == AppState.ALL_EMPLOYEES[1])
                {
                    Member member = new Member(e, 1, 2, true);
                    member.DrawCard(memGrid);
                }
                else if (e.position?.title == AppState.ALL_EMPLOYEES[2])
                {
                    Member member = new Member(e, 2, 2, true);
                    member.DrawCard(memGrid);
                }
                else if (e.position?.title == AppState.ALL_EMPLOYEES[3])
                {
                    Member member = new Member(e, 3, 2, true);
                    member.DrawCard(memGrid);
                }


                else
                {
                    Member member = new Member(e, col, row);
                    member.DrawCard(memGrid);
                    col += 1;
                    if(col >= 5)
                    {
                        row += 1;
                        col = 0;
                    }
                }
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
            private int width = 130, height = 145;
            private int col, row;
            private Button btnEdit;
            private bool isManage = false;

            public Member(Employee employee, int col, int row, bool isManage = false)
            {
                this.employee = employee;
                this.col = col;
                this.row = row;
                this.isManage = isManage;
            }

            public void DrawCard(Grid grid)
            {
                Border memberCard = new Border
                {
                    CornerRadius = new CornerRadius(10),
                    Width = this.width,
                    Height = this.height,
                    Background = Brushes.Yellow,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1)
                };

                Grid.SetColumn(memberCard, this.col);
                Grid.SetRow(memberCard, this.row);

                Grid mainGrid = new Grid();
                mainGrid.Background = Brushes.Transparent;

                Grid nestedGrid = new Grid
                {
                    Margin = new Thickness(0, 5, 0, 0),
                    Height = 50,
                    Width = 50,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top
                };

                Ellipse profilePicture = new Ellipse
                {
                    Width = 48,
                    Height = 48,
                    StrokeThickness = 1,
                    StrokeStartLineCap = PenLineCap.Round,
                    Stroke = Brushes.Black
                };

                ImageBrush imageBrush = new ImageBrush();
                if (employee.image?.GetSource() != null)
                {
                    imageBrush.ImageSource = employee.image.GetSource();
                }
                else
                {
                    imageBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/SPTC APP;component/View/Images/icons/person.png"));
                }

                profilePicture.Fill = imageBrush;
                nestedGrid.Children.Add(profilePicture);

                TextBlock nameTextBlock = new TextBlock
                {
                    Text = employee.name?.lastname?.ToString() ?? "???",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 58, 0, 0),
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 78,
                    TextAlignment = TextAlignment.Center,
                    FontSize = 12,
                    FontWeight = FontWeights.DemiBold
                };

                TextBlock positionTextBlock = new TextBlock
                {
                    Text = employee.position?.title?.ToString() ?? "???",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 70, 0, 0),
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 78,
                    TextAlignment = TextAlignment.Center,
                    FontSize = 10,
                    FontWeight = FontWeights.Light
                };

                mainGrid.Children.Add(nestedGrid);
                mainGrid.Children.Add(nameTextBlock);
                mainGrid.Children.Add(positionTextBlock);


                mainGrid.Margin = new Thickness(0, 15, 0, 0);

                memberCard.Child = mainGrid;

                if ((AppState.USER?.position?.accesses[17] ?? false)){
                    Button editButton = new Button
                    {
                        Content = isManage ? ((employee.position.title == AppState.USER.position.title)?"PROFILE": "MANAGE") : "EDIT",
                        Height = 25,
                        Width = 70,
                        Style = (Style)Application.Current.FindResource("ControlledButtonStyle"),
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(0, 0, 0, 5),
                        Visibility = Visibility.Hidden,
                        FontSize = 10
                    };
                    editButton.Click += EditClick;
                    mainGrid.Children.Add(editButton);
                    this.btnEdit = editButton;
                    mainGrid.MouseEnter += MemberMouseEnter;
                    mainGrid.MouseLeave += MemberMouseLeave;
                }

                grid.Children.Add(memberCard);
            }

            private void EditClick(object sender, RoutedEventArgs e)
            {
                string noname = "???";
                if (isManage )
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
                    } else
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
                    if(ControlWindow.ShowTwoway("Edit Profile", $"Board Member: {employee.name?.legalName ?? noname}", Icons.NOTIFY))
                    {
                        (new EditEmployee(employee, true, false)).Show();
                    }
                }
            }

            private void MemberMouseEnter(object sender, MouseEventArgs e)
            {
                if(sender is Grid grid)
                {
                    btnEdit.Visibility = Visibility.Visible;
                }
            }

            private void MemberMouseLeave(object sender, MouseEventArgs e)
            {
                if (sender is Grid grid)
                {
                    btnEdit.Visibility = Visibility.Hidden;
                }
            }


        }

    }
}
