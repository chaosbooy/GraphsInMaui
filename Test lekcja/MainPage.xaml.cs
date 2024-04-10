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

        private void OnNodeAdd(object sender, EventArgs e)
        {
            Random random = new Random();
            float lat = random.Next((int)nodeGraph.Width * 10) / 10;
            float lon = random.Next((int)nodeGraph.Height * 10) / 10;

            AddNextNode(null, lat, lon);
            UpdateNodeList();
        }

        private void OnNodeDelete(object sender, EventArgs e)
        {
            string selectedNode = NodeId.Text.Trim();

            if (!d.nodes.ContainsKey(selectedNode) || NodeId.Text.Trim() == string.Empty) return;
            d.nodes.Remove(selectedNode);

            UpdateNodeList();
        }

        private void OnNodeAddWithId(object sender, EventArgs e)
        {
            string selectedNode = NodeId.Text.Trim();
            float nextLat = 0;
            float nextLon = 0;

            if (d.nodes.ContainsKey(selectedNode) || NodeId.Text.Trim().Length < 4 || !float.TryParse(NodeLat.Text, out nextLat) || !float.TryParse(NodeLon.Text, out nextLon)) return;

            AddNextNode(selectedNode, nextLat, nextLon);
            UpdateNodeList();
        }

        private void OnNodeChangeLocation(object sender, EventArgs e)
        {
            string selectedNode = NodeId.Text.Trim();
            int lat = 0;
            int lon = 0;

            if (!d.nodes.ContainsKey(selectedNode) || NodeId.Text.Trim() == string.Empty || !Int32.TryParse(NodeLat.Text, out lat) || !Int32.TryParse(NodeLon.Text, out lon)) return;


            d.nodes[selectedNode].setLat(lat);
            d.nodes[selectedNode].setLon(lon);
            UpdateNodeList();
        }

        private void OnNodeChanged(object sender, EventArgs e)
        {
            Picker p = (Picker)sender;
        }

        private void NumberInput(object sender, TextChangedEventArgs e)
        {
            Entry entry = (Entry)sender;

            if (!int.TryParse(e.NewTextValue, out _))
            {
                entry.Text = new string(e.NewTextValue.Where(char.IsDigit).ToArray());
                return;
            }
        }

        private void UpdateNodeList()
        {
            nodeGraph.Invalidate();
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

        //interakcja z grafem
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
                    d.focusedNode = "";
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
    }
}
