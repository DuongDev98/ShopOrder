using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ShopOrder.Utils
{
    public class FileUploads
    {
        public static string UploadMatHang(HttpServerUtilityBase server, HttpPostedFileBase file)
        {
            return Upload(server, file, FileFolders.MatHang);
        }

        public static void DeleteMatHang(HttpServerUtilityBase server, string fileName)
        {
            Delete(server, fileName, FileFolders.MatHang);
        }
        public static string UploadKhachHang(HttpServerUtilityBase server, HttpPostedFileBase file)
        {
            return Upload(server, file, FileFolders.KhachHang);
        }

        public static void DeleteKhachHang(HttpServerUtilityBase server, string fileName)
        {
            Delete(server, fileName, FileFolders.KhachHang);
        }
        public static string UploadDonHang(HttpServerUtilityBase server, HttpPostedFileBase file)
        {
            return Upload(server, file, FileFolders.DonHang);
        }

        public static void DeleteDonHang(HttpServerUtilityBase server, string fileName)
        {
            Delete(server, fileName, FileFolders.DonHang);
        }

        private static string Upload(HttpServerUtilityBase server, HttpPostedFileBase file, string folders)
        {
            string tempFolder = Path.Combine(server.MapPath(string.Format("~/Images/{0}/", folders)));
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            string temp = file.FileName;
            string[] options = temp.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            string fileName = Guid.NewGuid().ToString();
            if (options.Length > 1)
            {
                fileName = fileName + "." + options[options.Length - 1];
            }
            else
            {
                fileName = fileName + ".jpg";
            }

            string path = string.Format("~/Images/{0}/{1}", folders, fileName);
            //path = Path.Combine(path);
            path = server.MapPath(path);
            if (File.Exists(path)) File.Delete(path);

            file.SaveAs(path);

            return fileName;
        }

        private static void Delete(HttpServerUtilityBase server, string fileName, string folders)
        {
            if (fileName.ToLower().Contains("noimage.jpg") || fileName.ToLower().Contains("no-image.jpg")) return;
            string path = Path.Combine(string.Format("~/Images/{0}/{1}", folders, fileName));
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    public class FileFolders {
        public const string MatHang = "Items";
        public const string KhachHang = "Customers";
        public const string DonHang = "Invoices";
    }
}