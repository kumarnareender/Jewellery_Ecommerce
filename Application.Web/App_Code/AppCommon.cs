using Application.Model.Models;
using Application.Service;
using Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using AutoMapper;
using Application.Common;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Drawing.Drawing2D;

namespace Application.Web.App_Code
{
    public static class AppCommon
    {
        public static void WriteActionLog(IActionLogService actionLogService, string module, string description, string value, string actionType, string actionBy)
        {
            try
            {
                ActionLog al = new ActionLog();
                al.Id = Guid.NewGuid().ToString();
                al.Module = module;
                al.Description = description;
                al.Value = value;
                al.ActionType = actionType;
                al.ActionBy = actionBy;
                al.ActionDate = DateTime.Now;
                actionLogService.CreateActionLog(al);
            }
            catch { }
        }

        public static string GetAllChildIds11(string id)
        {
            string sqlQuery = String.Empty;
            string allChildIds = id;

            sqlQuery = String.Format(@"
                                            ;WITH r as (
                                             SELECT ID
                                             FROM Category
                                             WHERE ParentID = {0}
                                             UNION ALL
                                             SELECT d.ID 
                                             FROM Category d
                                                INNER JOIN r 
                                                   ON d.ParentID = r.ID
                                        )
                                        SELECT ID FROM r ", id);

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (var context = new Data.Models.ApplicationEntities())
            {
                var recordList = context.Database.SqlQuery<ChildIds>(sqlQuery).ToList();
                if (recordList != null && recordList.Count > 0)
                {
                    foreach (var record in recordList)
                    {
                        allChildIds += "," + record.Id;
                    }
                }
            }

            return allChildIds;
        }
        
        public static void GenerateOrderBarcode(string barcode)
        {
            string barcodeFileName = barcode;
            barcode = "*" + barcode + "*";
            
            using (Bitmap bitmap = new Bitmap(500, 180)) // For 18 digits
            {
                bitmap.SetResolution(240, 240);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    Font font = new Font("IDAutomationHC39M", 10, FontStyle.Regular, GraphicsUnit.Point);

                    graphics.Clear(Color.White);
                    StringFormat stringformat = new StringFormat(StringFormatFlags.NoWrap);
                    graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    graphics.TextContrast = 10;
                    SolidBrush black = new SolidBrush(Color.Black);
                    SolidBrush white = new SolidBrush(Color.White);
                    PointF TextPosition = new PointF(45F, 10F);
                    SizeF TextSize = graphics.MeasureString(barcode, font, TextPosition, stringformat);
                    PointF pointPrice = new PointF(90f, 125f);
                    Font newfont2 = new Font("Cambria", 8, FontStyle.Regular, GraphicsUnit.Point);
                    Font newfont3 = new Font("Arial Black", 10, FontStyle.Regular, GraphicsUnit.Point);
                    if (TextSize.Width > bitmap.Width)
                    {
                        float ScaleFactor = (bitmap.Width - (TextPosition.X / 2)) / TextSize.Width;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.ScaleTransform(ScaleFactor, ScaleFactor);
                    }

                    graphics.DrawString(barcode, font, new SolidBrush(Color.Black), TextPosition, StringFormat.GenericTypographic);

                    string dirPath = HttpContext.Current.Server.MapPath("~") + "/Photos/Barcode/Orders";
                    bitmap.Save(dirPath + "\\" + barcodeFileName + ".Jpeg", ImageFormat.Jpeg);                    
                    font.Dispose();
                }
            } 
        }
    }

    public class ChildIds
    {
        public int Id { get; set; }
    }
}