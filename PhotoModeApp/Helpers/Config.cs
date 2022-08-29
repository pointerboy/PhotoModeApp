using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoModeApp.Helpers
{
    public class Config
    {
        private static string fileName = "config.path";

        public static string GetPath() => File.ReadAllText(fileName);
        public static void WritePath(string str) => File.WriteAllText(fileName, str);
    }
}
