using Microsoft.Maui.Controls;
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


        //interakcja z grafem

        private void UpdateNodeList()
        {
            nodeGraph.Invalidate();
            for (int i = 1; i < infoBlock.Children.Count; i++) infoBlock.Children.RemoveAt(i);

            if (d.focusedNode == "")
            {
                infoTitle.Text = "Node List";

                var scrollView = new ScrollView
                {
                    HeightRequest = 400,
                };

                var verticalLayout = new VerticalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Start,
                };
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
                infoTitle.Text = d.focusedNode;

                var scrollView = new ScrollView();
                var verticalLayout = new VerticalStackLayout 
                {
                    HeightRequest = infoBlock.Height,
                };
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

                var removeButton = new Button
                {
                    Text = "Delete",
                    TextColor = Colors.WhiteSmoke,
                    BackgroundColor = Colors.DarkRed,
                };
                var changeLocationButton = new Button
                {
                    Text = "Edit Location",
                };
                var changeNameButton = new Button
                {
                    Text = "Edit Name",
                };
                var horizontalLayout = new HorizontalStackLayout();
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
    }
}