using SQLite4Unity3d;

namespace Database
{
    public class UsersTable
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        public string LastSyncDate { get; set; }

        public string SocialId { get; set; }

        public string AccountId { get; set; }

        public bool IsConnected { get; set; }

        public bool IsFirstConnected { get; set; }

        public int BrilliantAmount { get; set; }

        public int GoldAmount { get; set; }

        public int InstallDate { get; set; }

        public int Age { get; internal set; }
    }
}