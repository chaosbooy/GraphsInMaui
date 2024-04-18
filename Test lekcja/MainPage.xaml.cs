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
        bool whichRoute;

        public MainPage()
        {
            InitializeComponent();
            d = new Drawing();
            nodeGraph.Drawable = d;
            d.changeLocationNode = "";
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

        private async void RouteClicked(object sender, EventArgs e)
        {
            d.fastestPath.Clear();
            d.fastestPath.Add(d.focusedNode);

            whichRoute = await DisplayAlert("Route Selection", "Which route do you wanna take", "Fastest", "Shortest");
        }

        private void UpdateNodeList()
        {
            nodeGraph.Invalidate();
            infoBlock.Children.Clear();
            infoBlock.Children.Add(infoTitle);

            if (d.fastestPath.Count > 1)
            {
                infoTitle.Text = (whichRoute) ? "Fastest Route" : "Shortest Route";

                var scrollView = new ScrollView
                {
                    HeightRequest = nodeGraph.Height - 150,
                };
                nodeGraph.SizeChanged += (s, e) =>
                {
                    scrollView.HeightRequest = this.nodeGraph.Height - 150;
                };
                var verticalLayout = new VerticalStackLayout();


                foreach (var node in d.fastestPath)
                {
                    var label = new Label
                    {
                        TextColor = d.focusedNode == node ? Colors.LightBlue : Colors.White,
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

                var cancelButton = new Button
                {
                    Text = "go back",
                    HeightRequest = 40,
                };
                cancelButton.Clicked += (s, e) =>
                {
                    d.focusedNode = d.fastestPath[0];
                    d.fastestPath.Clear();
                    UpdateNodeList();
                };
                infoBlock.Children.Add(cancelButton);

                return;
            }
            else if (!d.nodes.ContainsKey(d.focusedNode))
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
                    Margin = new Thickness(30, 0, 0, 0),
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
                var routeButton = new Button
                {
                    Text = "Find Route",
                    TextColor = Colors.WhiteSmoke,
                    BackgroundColor = Colors.Green,
                    HeightRequest = buttonHeight,
                };
                routeButton.Clicked += RouteClicked;
                var horizontalLayout = new FlexLayout
                {
                    HeightRequest = 100,
                };
                horizontalLayout.Children.Add(changeNameButton);
                horizontalLayout.Children.Add(changeLocationButton);
                horizontalLayout.Children.Add(routeButton);
                horizontalLayout.Children.Add(removeButton);

                scrollView.Content = verticalLayout;
                infoBlock.Children.Add(scrollView);
                infoBlock.Children.Add(horizontalLayout);
            }

            d.fastestPath.Clear();
        }

        private async void GraphTappedOnce(object sender, TappedEventArgs e)
        {
            Point? clickedPoint = e.GetPosition(nodeGraph);
            if (!clickedPoint.HasValue) return;
            double x = clickedPoint.Value.X, y = clickedPoint.Value.Y;

            bool madeOperation = false;
            if (d.changeLocationNode != "")
            {

                foreach (var node in d.nodes)
                    if (node.Key != d.changeLocationNode && Math.Sqrt(Math.Pow(x - node.Value.getLat(), 2) + Math.Pow(y - node.Value.getLon(), 2)) < d.radius * 2)
                    {
                        madeOperation = true;
                        break;
                    }

                if (madeOperation)
                {
                    await DisplayAlert("Error", $"You are moving {d.focusedNode} too close to another node. Try again", "OK");
                    return;
                }

                d.nodes[d.changeLocationNode].setLat((float)x);
                d.nodes[d.changeLocationNode].setLon((float)y);

                d.changeLocationNode = "";
                UpdateNodeList();
                return;
            }
            else if (d.fastestPath.Count == 1)
            {
                foreach (var node in d.nodes)
                {
                    double distance = Math.Sqrt(Math.Pow(x - node.Value.getLat(), 2) + Math.Pow(y - node.Value.getLon(), 2));
                    if (distance < d.radius)
                    {
                        if (node.Key == d.focusedNode) { break; }
                        try
                        {
                            d.fastestPath = GetFastestRoute(d.focusedNode, node.Key);
                        }
                        catch (Exception ex)
                        {
                            d.fastestPath.Clear();
                            await DisplayAlert("Error", ex.Message, "OK");
                        }

                        madeOperation = true;
                    }
                }

                if (!madeOperation) d.fastestPath.Clear();
                UpdateNodeList();
                return;
            }

            foreach (var node in d.nodes)
            {
                double distance = Math.Sqrt(Math.Pow(x - node.Value.getLat(), 2) + Math.Pow(y - node.Value.getLon(), 2));
                if (distance < d.radius && d.fastestPath.Count > 1)
                {
                    d.focusedNode = node.Key;
                    madeOperation = true;
                    break;

                }
                else if (distance < d.radius && d.focusedNode == "")
                {
                    d.focusedNode = node.Key;
                    madeOperation = true;
                    break;
                }
                else if (distance < d.radius && d.focusedNode == node.Key)
                {
                    d.focusedNode = "";
                    madeOperation = true;
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
            else if (!madeOperation && d.focusedNode != "" && d.fastestPath.Count < 2) d.focusedNode = "";

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
                d.changeLocationNode = "";
                UpdateNodeList();
            }
        }

        private async void NameChange(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Name Changer", "Write the new node name. To cancel write nothing");
            result.Trim();

            if (result == "") return;
            d.changedNames = (d.focusedNode, result);

            var node = d.nodes[d.focusedNode];
            d.nodes.Remove(d.focusedNode);
            d.nodes.Add(result, node);

            d.focusedNode = result;

            UpdateNodeList();

        }

        private async void LocationChange(object sender, EventArgs e)
        {
            await DisplayAlert("Alert", "Click anywhere to change the location of the node", "OK");

            d.changeLocationNode = (d.changeLocationNode == "") ? d.focusedNode : "";
            nodeGraph.Invalidate();
        }

        private List<string> GetFastestRoute(string start, string stop)
        {
            var ranking = new List<(List<string>, float)>();
            var history = new List<string>();
            float distance = EvaluateDistance(d.nodes[start], d.nodes[stop]);

            ranking.Add(new(item1: new List<string> { start }, item2: distance));

            while (ranking.Count > 0)
            {
                List<string> bestRoute = ranking[0].Item1;
                float onlyPoints = ranking[0].Item2 - EvaluateDistance(d.nodes[bestRoute[bestRoute.Count - 1]], d.nodes[stop]);
                history.Add(bestRoute[bestRoute.Count - 1]);
                ranking.RemoveAt(0);
                foreach (var friend in d.nodes[bestRoute[bestRoute.Count - 1]].getFriends())
                {
                    if (history.Contains(friend.Key)) continue;
                    else if (friend.Key == stop)
                    {
                        bestRoute.Add(stop);
                        return bestRoute;
                    }

                    List<string> next = new List<string>(bestRoute);
                    next.Add(friend.Key);
                    var nextDistance = EvaluateDistance(d.nodes[friend.Key], d.nodes[bestRoute[bestRoute.Count - 1]]);
                    distance = EvaluateDistance(d.nodes[friend.Key], d.nodes[stop]) + onlyPoints + nextDistance + ((whichRoute) ? friend.Value : 0);

                    ranking.Add(new(item1: next, item2: distance));
                }

                ranking = ranking.OrderBy(o => o.Item2).ToList();
            }

            throw new Exception($"There is no path from {start} to {stop}");
        }

        private float EvaluateDistance(Node first, Node second) { return (float)Math.Sqrt(Math.Pow(first.getLat() - second.getLat(), 2) + Math.Pow(first.getLon() - second.getLon(), 2)); }
    }
}