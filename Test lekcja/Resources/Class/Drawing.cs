using Microsoft.Maui.Graphics;

namespace Test_lekcja.Resources.Class
{
    internal class Drawing : IDrawable
    {
        public Dictionary<string, Node> nodes = new Dictionary<string, Node>();
        public Dictionary<string, string> changedNames = new Dictionary<string, string>();
        public float radius = 10f;
        public string focusedNode = "";

        public void Draw(ICanvas canvas, RectF rect)
        {
            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 5;
            canvas.FillColor = Colors.White;


            foreach(var node in nodes)
            {
                foreach (var friend in node.Value.getFriends().Keys)
                {
                    string tmpf = friend;
                    if (!nodes.ContainsKey(friend) && !changedNames.ContainsKey(friend))
                    {
                        node.Value.RemoveFriend(friend);
                        continue;
                    } 
                    else  if (changedNames.ContainsKey(friend))
                    {
                        node.Key.Replace(friend, changedNames[friend]);
                        tmpf = friend;
                    }

                    var start = new PointF(nodes[tmpf].getLat(), nodes[tmpf].getLon());
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
