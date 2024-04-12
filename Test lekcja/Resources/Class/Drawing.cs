using Microsoft.Maui.Graphics;

namespace Test_lekcja.Resources.Class
{
    internal class Drawing : IDrawable
    {
        public Dictionary<string, Node> nodes = new Dictionary<string, Node>();
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
                    if (!nodes.ContainsKey(friend)) nodes.Remove(friend);

                    var start = new PointF(nodes[friend].getLat(), nodes[friend].getLon());
                    var end = new PointF(node.Value.getLat(), node.Value.getLon());
                    canvas.DrawLine(start, end);
                }

                canvas.FillCircle(node.Value.getLat(), node.Value.getLon(), radius);
            }

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
