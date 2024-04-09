﻿namespace Test_lekcja.Resources.Class
{
    internal class Drawing : IDrawable
    {
        public Dictionary<string, Node> nodes = new Dictionary<string, Node>();
        public float radius = 10f;
        public string focusedNode = "";

        public void Draw(ICanvas canvas, RectF rect)
        {
            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 10;

            foreach(var node in nodes)
            {
                canvas.DrawCircle(node.Value.getLat(), node.Value.getLon(), radius);

                foreach (var friend in node.Value.getFriends().Keys) {

                    var start = new PointF(nodes[friend].getLat(), nodes[friend].getLon());
                    var end = new PointF(node.Value.getLat(), node.Value.getLon());
                    canvas.DrawLine(start, end);
                }
            }

            if(nodes.ContainsKey(focusedNode))
            {
                canvas.StrokeColor = Colors.Blue;
                canvas.DrawCircle(nodes[focusedNode].getLat(), nodes[focusedNode].getLon(), radius);
            }
        }
    }
}
