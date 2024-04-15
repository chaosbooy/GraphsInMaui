namespace Test_lekcja.Resources.Class
{
    internal class Node
    {
        private float lat, lon;
        private Dictionary<string, int> friends;

        public Node()
        {
            lat = 0; lon = 0;
            friends = new Dictionary<string, int>();
        }

        public Node(float lat, float lon)
        {
            this.lat = lat;
            this.lon = lon;
            friends = new Dictionary<string, int>();
        }

        public void AddFriend(string FriendID, int weight)
        {
            friends.Add(FriendID, weight);
        }

        public void ChangeFriendWeight(string FriendID, int weight)
        {
            friends[FriendID] = weight;
        }

        public void RemoveFriend(string FriendID)
        {
            friends.Remove(FriendID);
        }

        public void ChangePosition(float lat, float lon)
        {
            this.lat = lat;
            this.lon = lon;
        }

        public bool ContainsFriend(string FriendID)
        {
            return friends.ContainsKey(FriendID);
        }

        public float getLat() { return lat; }
        public float getLon() { return lon; }

        public void setLat(float lat) { this.lat = lat; }
        public void setLon(float lon) {  this.lon = lon; }

        public Dictionary<string, int> getFriends() { return friends; }

        public void ClearFriends() { friends.Clear(); }

        public void ReplaceFriends(Dictionary<string, int> newFriends) { friends = newFriends; }
    }
}
