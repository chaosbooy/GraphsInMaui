using Microsoft.Maui.Graphics;

namespace Test_lekcja.Resources.Class
{
    internal class Drawing : IDrawable
    {
        public Dictionary<string, Node> nodes = new Dictionary<string, Node>();
        public (string, string) changedNames = ("","");
        public List<string> fastestPath = new List<string>();
        public float radius = 10f;
        public string focusedNode = "";
        public string changeLocationNode = "";


        public void Draw(ICanvas canvas, RectF rect)
        {
            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 5;
            canvas.FillColor = Colors.White;
            radius = 10;

            if(fastestPath.Count > 0)
            {

            }

            if(changeLocationNode != "")
            {
                canvas.FillColor = Colors.Red;
                canvas.StrokeSize = 0;
                radius = 20;
            }

            foreach (var node in nodes)
            {
                if (changedNames.Item1 != null && node.Value.getFriends().ContainsKey(changedNames.Item1))
                {
                    node.Value.AddFriend(changedNames.Item2, node.Value.getFriends()[changedNames.Item1]);
                    node.Value.RemoveFriend(changedNames.Item1);
                }

                foreach (var friend in node.Value.getFriends().Keys)
                {
                    var start = new PointF(nodes[friend].getLat(), nodes[friend].getLon());
                    var end = new PointF(node.Value.getLat(), node.Value.getLon());
                    canvas.DrawLine(start, end);
                }
                canvas.FillCircle(node.Value.getLat(), node.Value.getLon(), radius);
            }
            changedNames = ("", "");

            if (nodes.ContainsKey(focusedNode))
            {
                canvas.StrokeSize = 5;
                canvas.StrokeColor = Colors.Blue;
                canvas.DrawCircle(nodes[focusedNode].getLat(), nodes[focusedNode].getLon(), radius - 2.5f);

                canvas.StrokeColor = Colors.LightBlue;
                foreach (var friend in nodes[focusedNode].getFriends().Keys)
                {
                    canvas.DrawCircle(nodes[friend].getLat(), nodes[friend].getLon(), radius - 2.5f);
                }
            }
            
        }
    }
}
