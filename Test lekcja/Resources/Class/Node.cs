namespace Test_lekcja.Resources.Class
{
    internal class Node
    {
        private int lat, lon;
        private Dictionary<int, int> friends;

        public Node()
        {
            lat = 0; lon = 0;
            friends = new Dictionary<int, int>();
        }

        public Node(int lat, int lon)
        {
            this.lat = lat;
            this.lon = lon;
            friends = new Dictionary<int, int>();
        }

        public void AddFriend(int FriendID, int weight)
        {
            friends.Add(FriendID, weight);
        }

        public void ChangeFriendWeight(int FriendID, int weight)
        {
            friends[FriendID] = weight;
        }

        public void RemoveFriend(int FriendID)
        {
            friends.Remove(FriendID);
        }

        public void ChangePosition(int lat, int lon)
        {
            this.lat = lat;
            this.lon = lon;
        }

        public int getLat() { return lat; }
        public int getLon() { return lon; }

        public void setLat(int lat) { this.lat = lat; }
        public void setLon(int lon) {  this.lon = lon; }

        public Dictionary<int, int> getFriends() { return friends; }
    }
}
