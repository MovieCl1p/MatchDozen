using System.Linq;
using Core.Database;
using Core.Mapper;
using Core.Utils;
using Data;
using Database;
using Enums;
using Model;
using strange.extensions.dispatcher.eventdispatcher.api;
using Services.Interfaces;
using SQLite4Unity3d;

namespace Services
{
    public class PlayerService : IPlayerService
    {
        [Inject]
        public IEventDispatcher Dispatcher { get; set; }

        [Inject]
        public PlayerModel PlayerModel { get; set; }

        private string _tempPlayerId;

        public void Initialize()
        {
            
        }

        public void GetPlayer(string socialId = null, string accountId = null, int age = 0)
        {
            UsersTable userCell = null;

            SQLRunner.Instance.AsyncCall(() =>
                {
                    SQLiteConnection connection = SQLRunner.Instance.Connection;

                    connection.BeginTransaction();

                    var usersTable = connection.Table<UsersTable>();

                    if (socialId != null)
                    {
                        userCell = usersTable.FirstOrDefault(p => p.SocialId == socialId);
                    }
                    else
                    {
                        var maxSyncDate = usersTable.Max(p => p.LastSyncDate);
                        userCell = usersTable.FirstOrDefault(p => p.LastSyncDate == maxSyncDate);
                    }

                    var isUserAlreadyExists = userCell != null;
                    if (isUserAlreadyExists)
                    {
                        if (userCell.Id != null)
                        {
                            int userId = (int)userCell.Id;
                            
                        }
                        
                        if (socialId != null)
                        {
                            userCell.IsConnected = true;
                        }

                    }
                    else
                    {
                        userCell = new UsersTable
                        {
                            BrilliantAmount = 20,
                            GoldAmount = 0,
                            InstallDate = TimeUtils.UnixTimeStamp,
                            LastSyncDate = "0",
                            SocialId = socialId,
                            AccountId = accountId ?? _tempPlayerId,
                            Age = age
                        };

                        if (socialId != null)
                        {
                            userCell.IsConnected = true;
                            userCell.IsFirstConnected = true;
                        }
                        
                        connection.InsertOrReplace(userCell);
                        int userId = (int)userCell.Id;
                        
                    }

                    connection.Commit();
                },
                () =>
                {
                    if (PlayerModel.Player == null)
                    {
                        PlayerModel.Player = new PlayerData();
                    }
                    
                    ObjectMapper.Map(userCell, PlayerModel.Player);
                    
                    Dispatcher.Dispatch(PlayerServiceEnum.GetPlayerComplete);
                },
                () =>
                {
                    Dispatcher.Dispatch(PlayerServiceEnum.GetPlayerFail);
                });
        }
    }
}