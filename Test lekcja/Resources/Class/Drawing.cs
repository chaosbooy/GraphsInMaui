using Microsoft.Maui.Graphics;

namespace Test_lekcja.Resources.Class
{
    internal class Drawing : IDrawable
    {
        public Dictionary<string, Node> nodes = new Dictionary<string, Node>();
        public Dictionary<string, string> changedNames = new Dictionary<string, string>();
        public float radius = 10f;
        public string focusedNode = "";
        public string changeLocationNode;

        public void Draw(ICanvas canvas, RectF rect)
        {
            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 5;
            canvas.FillColor = Colors.White;

            if(changeLocationNode != "")
            {
                canvas.FillColor = Colors.Red;
                foreach (var node in nodes)
                {
                    if (node.Key == changeLocationNode) continue;

                    canvas.FillCircle(node.Value.getLat(), node.Value.getLon(), radius * 2);
                }

                return;
            }

            foreach(var node in nodes)
            {
                {
                    var newFriends = new Dictionary<string, int>();
                    foreach (var friend in node.Value.getFriends().Keys)
                    {
                        string tmpf = friend;
                        if (nodes.ContainsKey(friend) && !changedNames.ContainsKey(friend))
                        {
                            newFriends.Add(tmpf,node.Value.getFriends()[friend]);
                        }
                        else if (changedNames.ContainsKey(friend))
                        {
                            tmpf = changedNames[friend];

                            int tmp = node.Value.getFriends()[friend];
                            newFriends.Add(tmpf, tmp);
                        }
                    }
                    node.Value.ReplaceFriends(newFriends);
                }

                foreach (var friend in node.Value.getFriends().Keys)
                {
                    var start = new PointF(nodes[friend].getLat(), nodes[friend].getLon());
                    var end = new PointF(node.Value.getLat(), node.Value.getLon());
                    canvas.DrawLine(start, end);
                }

                canvas.FillCircle(node.Value.getLat(), node.Value.getLon(), radius);
            }
            changedNames.Clear();

            if(nodes.ContainsKey(focusedNode))
            {
                canvas.StrokeSize = 5;
                canvas.StrokeColor = Colors.Blue;
                canvas.DrawCircle(nodes[focusedNode].getLat(), nodes[focusedNode].getLon(), radius - 2.5f);

                canvas.StrokeColor = Colors.LightBlue;
                foreach(var friend in nodes[focusedNode].getFriends().Keys)
                {
                    canvas.DrawCircle(nodes[friend].getLat(), nodes[friend].getLon(), radius - 2.5f);
                }
            }
        }
    }
}
