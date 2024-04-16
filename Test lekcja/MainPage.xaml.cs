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

        //interakcja z grafem

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
                    Margin = new Thickness(40, 0, 0, 0),
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

        private async void GraphTappedOnce(object sender, TappedEventArgs e)
        {
            Point? clickedPoint = e.GetPosition(nodeGraph);
            if (!clickedPoint.HasValue) return;
            double x = clickedPoint.Value.X, y = clickedPoint.Value.Y;

            if (d.changeLocationNode != "")
            {
                bool obstacle = false;

                foreach (var node in d.nodes)
                    if (node.Key != d.changeLocationNode && Math.Sqrt(Math.Pow(x - node.Value.getLat(), 2) + Math.Pow(y - node.Value.getLon(), 2)) < d.radius * 2)
                    {
                        obstacle = true;
                        break;
                    }

                if (obstacle)
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

            bool madeOperation = false;
            foreach (var node in d.nodes)
            {
                double distance = Math.Sqrt(Math.Pow(x - node.Value.getLat(), 2) + Math.Pow(y - node.Value.getLon(), 2));
                if (distance < d.radius && d.focusedNode == "")
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
                d.changeLocationNode = null;
                UpdateNodeList();
            }
        }

        private async void NameChange(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Name Changer", "Write the new node name. To cancel write nothing");
            result.Trim();

            if (result == "") return;
            else if (d.changedNames.ContainsValue(d.focusedNode)) d.changedNames[d.changedNames.First(x => x.Value == d.focusedNode).Key] = result;
            else d.changedNames.Add(d.focusedNode, result);

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
            var ranking = new List<Improvised>();
            var history = new List<string>();
            float distance = EvaluateDistance(d.nodes[start], d.nodes[stop]);

            ranking.Add(new Improvised
            {
                path = new List<string> { start },
                dist = distance,
            });

            while (ranking.Count > 0)
            {
                List<string> bestRoute = ranking[0].path;
                float onlyPoints = ranking[0].dist - EvaluateDistance(d.nodes[bestRoute[bestRoute.Count - 1]], d.nodes[stop]);
                history.Add(bestRoute[bestRoute.Count - 1]);
                ranking.RemoveAt(0);
                foreach(var friend in d.nodes[bestRoute[bestRoute.Count - 1]].getFriends())
                {
                    if (history.Contains(friend.Key)) continue;
                    else if(friend.Key == stop)
                    {
                        bestRoute.Add(stop);
                        return bestRoute;
                    }

                    List<string> next = bestRoute;
                    next.Add(friend.Key);
                    distance = EvaluateDistance(d.nodes[friend.Key], d.nodes[stop]) + onlyPoints + friend.Value;

                    ranking.Add(new Improvised
                    {
                        dist = distance,
                        path = next,
                    });
                }

                ranking.Sort((x, y) => x.dist.CompareTo(y.dist));
            }

            return null;
        }

        private float EvaluateDistance(Node first, Node second) { return (float)Math.Sqrt(Math.Pow(first.getLat() - second.getLat(), 2) + Math.Pow(first.getLon() - second.getLon(), 2)); }
    }
}
public struct Improvised
{
    public List<string> path;
    public float dist;
}