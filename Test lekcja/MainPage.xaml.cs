using Microsoft.Maui.Controls;
using Test_lekcja.Resources.Class;

namespace Test_lekcja
{
    public partial class MainPage : ContentPage
    {
        int nodeCountId;
        string selectedNode;
        Drawing d;
        List<string> nodePicker;

        public MainPage()
        {
            InitializeComponent();
            d = new Drawing();
            nodePicker = new List<string>();
            selectedNode = "";
            nodeGraph.Drawable = d;
        }

        private void OnNodeAdd(object sender, EventArgs e)
        {
            Random random = new Random();
            float lat = random.Next(15000) / 10;
            float lon = random.Next(6000) / 10;

            while (d.nodes.ContainsKey($"Node{nodeCountId}")) nodeCountId++;

            d.nodes.Add($"Node{nodeCountId}", new Node(lat, lon));
            UpdateNodeList();
        }

        private void OnNodeDelete(object sender, EventArgs e)
        {
            if (!d.nodes.ContainsKey(selectedNode) || NodeId.Text.Trim() == string.Empty) return;
            d.nodes.Remove(selectedNode);
            nodePicker.Remove(selectedNode);

            UpdateNodeList();
        }

        private void OnNodeAddWithId(object sender, EventArgs e)
        {
            float nextLat = 0;
            float nextLon = 0;

            if (d.nodes.ContainsKey(selectedNode) || NodeId.Text.Trim() == string.Empty || !float.TryParse(NodeLat.Text, out nextLat) || !float.TryParse(NodeLon.Text, out nextLon)) return;

            d.nodes.Add(selectedNode, new Node(nextLat, nextLon));
            nodePicker.Add(selectedNode);
            UpdateNodeList();
        }

        private void OnNodeChangeLocation(object sender, EventArgs e)
        {
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


        //interakcja z grafem

        private void GraphTappedOnce(object sender, TappedEventArgs e)
        {
            Point? clickedPoint = e.GetPosition(nodeGraph);
            if (!clickedPoint.HasValue) return;
            double x = clickedPoint.Value.X, y = clickedPoint.Value.Y;

            Node currentNode = new Node();
            bool madeOperation = false;
            foreach (var node in d.nodes)
            {
                double distance = Math.Sqrt(Math.Pow(x - node.Value.getLat(), 2) + Math.Pow(y - node.Value.getLon(), 2));
                if(distance < d.radius && d.focusedNode == "")
                {
                    ShowNodeInfo(node.Key);  //JESZCZE NIE ISTNIEJE
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
                else if (distance < 1.5f * d.radius)
                {
                    madeOperation = true;
                    break;
                }
            }

            if (!madeOperation && d.focusedNode == "")
            {
                d.nodes.Add("sdsd", new Node((float)x, (float)y));  //DO POPRAWY 
                d.focusedNode = "";
            }
            else if (!madeOperation && d.focusedNode != "") d.focusedNode = "";

            nodeGraph.Invalidate();
        }


        private void ShowNodeInfo(string nodeKey)
        {

        }
    }
}
