﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomsNumber
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class RoomsNumber : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string massage, ElementSet elements)
        {
            Document arDoc = commandData.Application.ActiveUIDocument.Document;
            var rooms = new FilteredElementCollector(arDoc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .Select(r => r as Room)
                .GroupBy(x => x.LevelId)
                .ToList();

            Transaction transaction = new Transaction(arDoc, "Установка маркеров помещений этаж_номер");
            transaction.Start("Установка номеров помещений");
            foreach (var Levelid in rooms)
            {
                string floor = arDoc.GetElement(Levelid.Key).Name;
                floor = floor.Replace("Этаж ", "");


                int numberRoom = 1;
                foreach (var item in Levelid)
                {
                    item.LookupParameter("Номер").Set(floor + "_" + numberRoom.ToString());
                    numberRoom++;
                }
            }
            transaction.Commit();
            return Result.Succeeded;
        }
    }
}
