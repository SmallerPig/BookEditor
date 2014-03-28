using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RY.Common
{
    public static class DirectoryAndFile
    {
        #region 静态方法

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="directory">相对路径</param>
        public static void CreateDirectory(string directory)
        {
            if (directory.Length == 0) return;
            if (!System.IO.Directory.Exists(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + directory))
                System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + directory);
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="absolutePath">是否为绝对路径</param>
        /// <param name="directory">目录</param>
        public static void DeleteDirectory(bool absolutePath, string directory)
        {
            if (absolutePath)
            {
                DirectoryInfo oldDirectory = new DirectoryInfo(directory);
                oldDirectory.Delete(true);
            }
            else
            {
                if (directory.Length == 0) return;
                if (System.IO.Directory.Exists(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + directory))
                    System.IO.Directory.Delete(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + directory);
            }
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="file">相对路径和文件名（包含扩展名）</param>
        /// <returns>是否存在</returns>
        public static bool FileExists(string file)
        {
            if (File.Exists(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + file))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="file">相对路径和文件名（包含扩展名）</param>
        /// <returns>文件不存在，返回 null</returns>
        public static string ReadFile(string file)
        {
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + file, System.Text.Encoding.UTF8);
                string str = sr.ReadToEnd();
                sr.Close();
                return str;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 保存文件内容
        /// </summary>
        /// <param name="fileContent">文件内容</param>
        /// <param name="file">相对路径和文件名（包含扩展名）</param>
        /// <returns>是否成功</returns>
        public static bool SaveFile(string fileContent, string file)
        {
            try
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + file, false, System.Text.Encoding.UTF8);
                sw.Write(fileContent);
                sw.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="file">相对路径和文件名（包含扩展名）</param>
        public static void DeleteFile(string file)
        {
            if (file.Length == 0) return;
            if (System.IO.File.Exists(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + file))
                System.IO.File.Delete(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\" + file);
        }

        /// <summary>
        /// 获得文件的目录路径
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>以\结尾的路径</returns>
        public static string GetFoldPath(string filePath)
        {
            return GetFoldPath(false, filePath);
        }

        /// <summary>
        /// 获得文件的目录路径
        /// </summary>
        /// <param name="isUrl">是否是网址</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>
        /// 网址：以/结尾的路径
        /// 物理地址：以\结尾的路径
        /// </returns>
        public static string GetFoldPath(bool isUrl, string filePath)
        {
            if (isUrl)
                return filePath.Substring(0, filePath.LastIndexOf("/") + 1);
            else
                return filePath.Substring(0, filePath.LastIndexOf("\\") + 1);
        }

        /// <summary>
        /// 获得文件的名称
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件名</returns>
        public static string GetFileName(string filePath)
        {
            return GetFileName(false, filePath);
        }

        /// <summary>
        /// 获得文件的名称
        /// </summary>
        /// <param name="isUrl">是否是网址</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件名</returns>
        public static string GetFileName(bool isUrl, string filePath)
        {
            if (isUrl)
                return filePath.Substring(filePath.LastIndexOf("/") + 1, filePath.Length - filePath.LastIndexOf("/") - 1);
            else
                return filePath.Substring(filePath.LastIndexOf("\\") + 1, filePath.Length - filePath.LastIndexOf("\\") - 1);
        }

        /// <summary>
        /// 获得文件的扩展名
        /// 不带点，小写
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>扩展名</returns>
        public static string GetFileExt(string filePath)
        {
            return filePath.Substring(filePath.LastIndexOf(".") + 1, filePath.Length - filePath.LastIndexOf(".") - 1).ToLower();
        }

        /// <summary>
        /// 目录拷贝
        /// </summary>
        /// <param name="oldDirectory">拷贝目录</param>
        /// <param name="newDirectory">目标目录</param>
        public static void CopyDirectory(string oldDirectory, string newDirectory)
        {
            DirectoryInfo oldDir = new DirectoryInfo(oldDirectory);
            DirectoryInfo newDir = new DirectoryInfo(newDirectory);
            CopyDirectory(oldDir, newDir);
        }

        /// <summary>
        /// 目录拷贝
        /// </summary>
        /// <param name="oldDirectory">拷贝目录</param>
        /// <param name="newDirectory">目标目录</param>
        private static void CopyDirectory(DirectoryInfo oldDirectory, DirectoryInfo newDirectory)
        {
            string NewDirectoryFullName = newDirectory.FullName + "\\" + oldDirectory.Name;

            if (!Directory.Exists(NewDirectoryFullName))
                Directory.CreateDirectory(NewDirectoryFullName);

            FileInfo[] OldFileAry = oldDirectory.GetFiles();
            foreach (FileInfo aFile in OldFileAry)
                File.Copy(aFile.FullName, NewDirectoryFullName + "\\" + aFile.Name, true);

            DirectoryInfo[] OldDirectoryAry = oldDirectory.GetDirectories();
            foreach (DirectoryInfo aOldDirectory in OldDirectoryAry)
            {
                DirectoryInfo aNewDirectory = new DirectoryInfo(NewDirectoryFullName);
                CopyDirectory(aOldDirectory, aNewDirectory);
            }
        }

        /// <summary>
        /// 目录剪切
        /// </summary>
        /// <param name="oldDirectory">剪切目录</param>
        /// <param name="newDirectory">目标目录</param>
        public static void CutDirectory(string oldDirectory, string newDirectory)
        {
            CopyDirectory(oldDirectory, newDirectory);
            DeleteDirectory(false, oldDirectory);
        }

        #endregion
    }
}
