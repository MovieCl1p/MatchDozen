using System.Collections.Generic;
using Core;
using Game.Cells;
using Game.Commands.Arguments;
using Game.Factories;
using Mechanics.Defense;
using UnityEngine;

namespace Game.Commands
{
    public class PlaceElementsCommand : StateCommand
    {
        protected override void OnStart(object[] args)
        {
            base.OnStart(args);

            PlaceElementsCommandArguments arg = (PlaceElementsCommandArguments) args[0];
            
            StartField(arg);

//            Test(arg);
            
            FinishCommand();
        }
        
        private void Test(PlaceElementsCommandArguments arg)
        {

            CellController cellController = CellFactory.GetCell(arg.PlaceHolder);
            cellController.SetPosition(new MapPoint(0, 4));
            cellController.SetCount(1);
            cellController.SetViewPosition(new Vector3(0, 0, 0));
            arg.CellControllers.Add(cellController);
//            arg.Field[0][4] = false;

            cellController = CellFactory.GetCell(arg.PlaceHolder);
            cellController.SetPosition(new MapPoint(0, 4));
            cellController.SetCount(2);
            cellController.SetViewPosition(new Vector3(0, 0, 0));
            arg.CellControllers.Add(cellController);
            //            arg.Field[0][1] = false;

            cellController = CellFactory.GetCell(arg.PlaceHolder);
            cellController.SetPosition(new MapPoint(0, 4));
            cellController.SetCount(3);
            cellController.SetViewPosition(new Vector3(0, 0, 0));
            arg.CellControllers.Add(cellController);

            cellController = CellFactory.GetCell(arg.PlaceHolder);
            cellController.SetPosition(new MapPoint(0, 4));
            cellController.SetCount(4);
            cellController.SetViewPosition(new Vector3(0, 0, 0));
            arg.CellControllers.Add(cellController);

            cellController = CellFactory.GetCell(arg.PlaceHolder);
            cellController.SetPosition(new MapPoint(0, 4));
            cellController.SetCount(5);
            cellController.SetViewPosition(new Vector3(0, 0, 0));
            arg.CellControllers.Add(cellController);
        }

        private void StartField(PlaceElementsCommandArguments arg)
        {
            Dictionary<int, int> respawnList = new Dictionary<int, int>();
            for (int i = 0; i < arg.Field.Length; i++)
            {
                for (int j = 0; j < arg.Field[i].Length; j++)
                {
                    if (arg.Field[i][j])
                    {
                        if (!respawnList.ContainsKey(i))
                        {
                            respawnList.Add(i, 0);
                        }

                        respawnList[i]++;
                        
                        CellController cellController = CellFactory.GetCell(arg.PlaceHolder);
                        cellController.SetPosition(new MapPoint(i, j));
                        cellController.SetCount(Random.Range(1, 5));
                        cellController.SetViewPosition(new Vector3(i, j, 0));
                        cellController.MoveToPosition();

                        cellController.CachedTransform.localPosition = FieldUtils.GetPosition(new Vector3(i, 5, 0));

                        if (arg.OnClickAction != null)
                        {
                            cellController.Click += arg.OnClickAction;
                        }

                        arg.CellControllers.Add(cellController);

                        arg.Field[i][j] = false;
                    }
                }
            }
        }
    }
}
