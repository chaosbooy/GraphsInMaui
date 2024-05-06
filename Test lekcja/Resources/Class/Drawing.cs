using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Text;

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
            canvas.FontColor = Colors.White;
            canvas.FontSize = 23;

            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 5;
            canvas.FillColor = Colors.White;
            radius = 10;

            

            if (changeLocationNode != "")
            {
                canvas.FillColor = Colors.Red;
                canvas.StrokeSize = 0;
                radius = 20;
            }

            if (fastestPath.Count > 0)
            {
                foreach (var node in nodes)
                {
                    foreach (var friend in node.Value.getFriends())
                    {
                        var start = new PointF(nodes[friend.Key].getLat(), nodes[friend.Key].getLon());
                        var end = new PointF(node.Value.getLat(), node.Value.getLon());

                        canvas.StrokeColor = (fastestPath.Contains(node.Key) && fastestPath[fastestPath.FindIndex(x => x == node.Key) + 1] == friend.Key) ? Colors.Bisque : Colors.White;
                        canvas.DrawLine(start, end);

                        var textDisplay = GetCenter(start, end);
                        float x = node.Value.getLat() - nodes[friend.Key].getLat();
                        float y = node.Value.getLon() - nodes[friend.Key].getLon();
                        float angle = (float)(Math.Atan2(y, x) * 180 / Math.PI);
                        canvas.Rotate(angle, textDisplay.X, textDisplay.Y);
                        canvas.DrawString(friend.Value.ToString(), textDisplay.X, textDisplay.Y - 5, HorizontalAlignment.Center);
                        canvas.Rotate(-angle, textDisplay.X, textDisplay.Y);
                    }
                }

                foreach (var node in nodes)
                {

                    canvas.FillColor = fastestPath.Contains(node.Key) ? Colors.Salmon : Colors.White;
                    canvas.FillCircle(node.Value.getLat(), node.Value.getLon(), radius);
                }
                
            }

            else
            {
                foreach (var node in nodes)
                {
                    if (changedNames.Item1 != null && node.Value.getFriends().ContainsKey(changedNames.Item1))
                    {
                        node.Value.AddFriend(changedNames.Item2, node.Value.getFriends()[changedNames.Item1]);
                        node.Value.RemoveFriend(changedNames.Item1);
                    }

                    foreach (var friend in node.Value.getFriends())
                    {
                        var start = new PointF(nodes[friend.Key].getLat(), nodes[friend.Key].getLon());
                        var end = new PointF(node.Value.getLat(), node.Value.getLon());
                        canvas.DrawLine(start, end);

                        var textDisplay = GetCenter(start, end);
                        float x = node.Value.getLat() - nodes[friend.Key].getLat();
                        float y = node.Value.getLon() - nodes[friend.Key].getLon();
                        float angle = (float)(Math.Atan2(y,x) * 180 / Math.PI);
                        canvas.Rotate(angle, textDisplay.X, textDisplay.Y);
                        canvas.DrawString(friend.Value.ToString(), textDisplay.X, textDisplay.Y - 5, HorizontalAlignment.Center);
                        canvas.Rotate(-angle, textDisplay.X, textDisplay.Y);
                    }
                    canvas.FillCircle(node.Value.getLat(), node.Value.getLon(), radius);
                }
                changedNames = ("", "");
            }

            if (nodes.ContainsKey(focusedNode))
            {
                canvas.StrokeSize = 5;
                canvas.StrokeColor = Colors.Blue;
                canvas.DrawCircle(nodes[focusedNode].getLat(), nodes[focusedNode].getLon(), radius - 2.5f);

                canvas.StrokeColor = Colors.LightBlue;

                if(fastestPath.Count < 2 && changeLocationNode == "")
                    foreach (var friend in nodes[focusedNode].getFriends().Keys)
                    {
                        canvas.DrawCircle(nodes[friend].getLat(), nodes[friend].getLon(), radius - 2.5f);
                    }
            }
            
        }

        private PointF GetCenter(PointF node1, PointF node2)
        {
            return new PointF
            {
                X = (node1.X + node2.X) / 2,
                Y = (node1.Y + node2.Y) / 2,
            };
        }
    }
}
