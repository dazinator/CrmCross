using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CrmCross.Tests;

namespace CrmCross.Droid
{
    public class FileSystem : IFileSystem
    {
        public bool Exists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }

        public string ReadAllText(string path)
        {
            try
            {
                return System.IO.File.ReadAllText(path);
            }
            catch (Exception e)
            {
                throw;
            }
          
        }
    }
}