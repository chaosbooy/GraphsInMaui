using Test_lekcja.Resources.Class;

namespace Test_lekcja
{
    public partial class MainPage : ContentPage
    {
        Dictionary<int, Node> nodes;
        int nodeCountId;
        int selectedNode;

        public MainPage()
        {
            InitializeComponent();
            nodes = new Dictionary<int, Node>();
            selectedNode = 0;
        }

        private void OnNodeAdd(object sender, EventArgs e)
        {
            Random random = new Random();
            int lat = random.Next(100);
            int lon = random.Next(100);

            while (nodes.ContainsKey(nodeCountId)) nodeCountId++;

            nodes.Add(nodeCountId, new Node(lat, lon));
            
            UpdateNodeList();
        }

        private void OnNodeDelete(object sender, EventArgs e)
        {
            if (!nodes.ContainsKey(selectedNode) || NodeId.Text.Trim() == string.Empty) return;
            nodes.Remove(selectedNode);

            UpdateNodeList();
        }

        private void OnNodeAddWithId(object sender, EventArgs e)
        {
            int nextLat = 0;
            int nextLon = 0;

            if (nodes.ContainsKey(selectedNode) || NodeId.Text.Trim() == string.Empty || !Int32.TryParse(NodeLat.Text, out nextLat) || !Int32.TryParse(NodeLon.Text, out nextLon)) return;
            
            nodes.Add(selectedNode, new Node(nextLat, nextLon));
            UpdateNodeList();
        }

        private void OnNodeChangeLocation(object sender, EventArgs e)
        {
            int lat = 0;
            int lon = 0;

            if (!nodes.ContainsKey(selectedNode) || NodeId.Text.Trim() == string.Empty || !Int32.TryParse(NodeLat.Text, out lat) || !Int32.TryParse(NodeLon.Text, out lon)) return;


            nodes[selectedNode].setLat(lat);
            nodes[selectedNode].setLon(lon);
            UpdateNodeList();
        }

        private void NodeIDInput(object sender, TextChangedEventArgs e)
        {
            Entry entry = (Entry)sender;

            if (!int.TryParse(e.NewTextValue, out _))
            {
                entry.Text = new string(e.NewTextValue.Where(char.IsDigit).ToArray());
                return;
            }

            if (entry.Text == string.Empty) return;
               selectedNode = Int32.Parse(entry.Text);
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
            NodeList.Text = "Node List: ";

            foreach (int id in nodes.Keys)
            {
                NodeList.Text += $"\n{id} Node {nodes[id].getLat()} {nodes[id].getLon()}";
            }

            SemanticScreenReader.Announce(NodeList.Text);
        }


        //operowanie na obiekcie wyświetlanym
        bool doubleTapped;
        bool ignoreNextTap;
        void OnTapGestureRecognizerDoubleTapped(object sender, TappedEventArgs args)
        {
            doubleTapped = true;
        }
        async void OnTapGestureRecognizerSingleTapped(object sender, TappedEventArgs args)
        {
            var delay = Task.Delay(100);
            await delay;
            if (ignoreNextTap)
            {
                ignoreNextTap = false;
                return;
            }
            if (doubleTapped)
            {
                doubleTapped = false;
                ignoreNextTap = true;
                NodeList.Text = "double tapped";
            }
            else
            {
                NodeList.Text = "single tapped";
            }
        }

        void OnDragRecognizer(object sender, DragStartingEventArgs args) 
        {
            NodeList.Text = "DragStart";

            SemanticScreenReader.Announce(NodeList.Text);
        }
        void OnDropRecognizer(object sender, DropCompletedEventArgs args) 
        {
            NodeList.Text = "Dropped";

            NodeVisual.Layout(new Rect(100, 100, 50, 50));

            
            SemanticScreenReader.Announce(NodeList.Text);
        }

    }

}
