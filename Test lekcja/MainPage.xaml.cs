using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Xml.Linq;
using Test_lekcja.Resources.Class;

namespace Test_lekcja
{
    public partial class MainPage : ContentPage
    {
        int nodeCountId;
        Drawing d;

        public MainPage()
        {
            InitializeComponent();
            d = new Drawing();
            nodeGraph.Drawable = d;
            changeLocationNode = "";
        }

        private void AddNextNode(string name, float x, float y)
        {
            if (name == null)
            {
                while (d.nodes.ContainsKey($"Node{nodeCountId}")) nodeCountId++;
                d.nodes.Add($"Node{nodeCountId}", new Node(x, y));
            }
            else if (d.nodes.ContainsKey(name))
            {
                throw new Exception("LP.1: Key already exists");
            }
            else
            {
                d.nodes.Add(name, new Node(x, y));
            }
        }

        private void OnNodeAdd(object sender, EventArgs e)
        {
            Random random = new Random();
            float lat = random.Next((int)nodeGraph.Width * 10) / 10;
            float lon = random.Next((int)nodeGraph.Height * 10) / 10;

            AddNextNode(null, lat, lon);
            UpdateNodeList();
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            var layout = (VerticalStackLayout)sender;
            nodeGraph.HeightRequest = layout.Height;
        }

        //interakcja z grafem
        private string changeLocationNode;

        private void UpdateNodeList()
        {
            nodeGraph.Invalidate();
            infoBlock.Children.Clear();
            infoBlock.Children.Add(infoTitle);

            if (!d.nodes.ContainsKey(d.focusedNode))
            {
                infoTitle.Text = $"Node List";

                var scrollView = new ScrollView
                {
                    HeightRequest = nodeGraph.Height - 70,
                };

                nodeGraph.SizeChanged += (s, e) =>
                {
                    scrollView.HeightRequest = this.nodeGraph.Height - 70;
                };
                var verticalLayout = new VerticalStackLayout();
                foreach (var node in d.nodes.Keys)
                {
                    var label = new Label
                    {
                        Text = node,
                        FontSize = 30,
                        HorizontalOptions = LayoutOptions.Fill,
                        Padding = 3,
                        Margin = 7,
                    };
                    var tapLabel = new TapGestureRecognizer();
                    tapLabel.Tapped += (s, e) =>
                    {
                        d.focusedNode = node;
                        UpdateNodeList();
                    };

                    label.GestureRecognizers.Add(tapLabel);

                    verticalLayout.Add(label);
                }

                scrollView.Content = verticalLayout;
                infoBlock.Children.Add(scrollView);
            }
            else
            {
                infoTitle.Text = $"{d.focusedNode}";

                var scrollView = new ScrollView
                {
                    HeightRequest = nodeGraph.Height - 150,
                };
                nodeGraph.SizeChanged += (s, e) =>
                {
                    scrollView.HeightRequest = this.nodeGraph.Height - 150;
                };
                var verticalLayout = new VerticalStackLayout();
                foreach (var friend in d.nodes[d.focusedNode].getFriends().Keys)
                {
                    var label = new Label
                    {
                        Text = friend,
                        FontSize = 30,
                        HorizontalOptions = LayoutOptions.Fill,
                        Padding = 3,
                        Margin = 7,
                    };
                    var tapLabel = new TapGestureRecognizer();
                    tapLabel.Tapped += (s, e) =>
                    {
                        d.focusedNode = friend;
                        UpdateNodeList();
                    };

                    label.GestureRecognizers.Add(tapLabel);

                    verticalLayout.Add(label);
                }

                int buttonHeight = 40;
                var removeButton = new Button
                {
                    Text = "Delete",
                    TextColor = Colors.WhiteSmoke,
                    BackgroundColor = Colors.DarkRed,
                    HeightRequest = buttonHeight,
                    Margin = new Thickness(40,0,0,0),
                };
                removeButton.Clicked += ConfirmDelete;
                var changeLocationButton = new Button
                {
                    Text = "Edit Location",
                    HeightRequest = buttonHeight,
                };
                changeLocationButton.Clicked += LocationChange;
                var changeNameButton = new Button
                {
                    Text = "Edit Name",
                    HeightRequest = buttonHeight,
                };
                changeNameButton.Clicked += NameChange;
                var horizontalLayout = new FlexLayout
                {
                    HeightRequest = 100,
                };
                horizontalLayout.Children.Add(changeNameButton);
                horizontalLayout.Children.Add(changeLocationButton);
                horizontalLayout.Children.Add(removeButton);

                scrollView.Content = verticalLayout;
                infoBlock.Children.Add(scrollView); 
                infoBlock.Children.Add(horizontalLayout);
            }
        }

        private void GraphTappedOnce(object sender, TappedEventArgs e)
        {
            Point? clickedPoint = e.GetPosition(nodeGraph);
            if (!clickedPoint.HasValue) return;
            double x = clickedPoint.Value.X, y = clickedPoint.Value.Y;

            if (changeLocationNode.Length > 0)
            {
                d.nodes[changeLocationNode].setLat((float)x);
                d.nodes[changeLocationNode].setLon((float)y);

                return;
            }

            bool madeOperation = false;
            foreach (var node in d.nodes)
            {
                double distance = Math.Sqrt(Math.Pow(x - node.Value.getLat(), 2) + Math.Pow(y - node.Value.getLon(), 2));
                if(distance < d.radius && d.focusedNode == "")
                {
                    d.focusedNode = node.Key;
                    madeOperation = true;
                    break;
                }
                else if (distance < d.radius && d.focusedNode == node.Key)
                {
                    madeOperation = true;
                    d.focusedNode = "";
                    break;
                }
                else if (distance < d.radius && d.focusedNode != "" && d.focusedNode != node.Key)
                {
                    if (d.nodes[d.focusedNode].ContainsFriend(node.Key))
                        d.nodes[d.focusedNode].RemoveFriend(node.Key);
                    else 
                        d.nodes[d.focusedNode].AddFriend(node.Key, 0);

                    madeOperation = true;
                    break;
                }
                else if (distance < 2f * d.radius)
                {
                    madeOperation = true;
                }
            }

            if (!madeOperation && d.focusedNode == "")
            {
                AddNextNode(null, (float)x, (float)y);
                d.focusedNode = "";
            }
            else if (!madeOperation && d.focusedNode != "") d.focusedNode = "";

            UpdateNodeList();
        }

        private void InfoTitleTapped(object sender, TappedEventArgs e)
        {
            d.focusedNode = "";
            UpdateNodeList();
        }

        private async void ConfirmDelete(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Confirmation", $"Do you really want to delete node {d.focusedNode}", "Yes", "No");

            if (answer) 
            {
                if (d.nodes.ContainsKey(d.focusedNode)) d.nodes.Remove(d.focusedNode);
                d.focusedNode = "";
                UpdateNodeList();
            }
        }

        private async void NameChange(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Name Changer", "Write the new node name");
        }

        private async void LocationChange(object sender, EventArgs e)
        {
            await DisplayAlert("Alert", "Click anywhere to change the location of the node", "OK");

            changeLocationNode = d.focusedNode;
        }
    }
}