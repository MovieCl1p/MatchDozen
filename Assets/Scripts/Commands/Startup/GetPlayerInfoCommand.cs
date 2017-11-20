using Enums;
using Model;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using Services.Interfaces;
using UnityEngine;

namespace Commands.Startup
{
    public class GetPlayerInfoCommand : Command
    {
        [Inject]
        public IPlayerService PlayerService { get; set; }

        [Inject]
        public PlayerModel PlayerModel { get; set; }

        public override void Execute()
        {
            Retain();

            PlayerService.Dispatcher.AddListener(PlayerServiceEnum.GetPlayerComplete, OnLoginSuccess);
            PlayerService.Dispatcher.AddListener(PlayerServiceEnum.GetPlayerFail, OnLoginFail);
            PlayerService.GetPlayer();
        }

        private void OnLoginSuccess()
        {
            RemoveListeners();
            Debug.Log("GetPlayerInfoCommand:DONE");
            Release();
        }

        private void OnLoginFail(IEvent evt)
        {
            RemoveListeners();
            Fail();
        }

        private void RemoveListeners()
        {
            PlayerService.Dispatcher.RemoveListener(PlayerServiceEnum.GetPlayerComplete, OnLoginSuccess);
            PlayerService.Dispatcher.RemoveListener(PlayerServiceEnum.GetPlayerFail, OnLoginFail);
        }
    }
}
