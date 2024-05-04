using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PyinstallerHelper
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!Directory.Exists(Constants.AppDataPath))
            {
                Directory.CreateDirectory(Constants.AppDataPath);
            } if (!Directory.Exists(Constants.DistPath))
            {
                Directory.CreateDirectory(Constants.DistPath);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Routines.main = new Form1();
            Application.Run(Routines.main);
        }
    }
    public static class Routines
    {
        public static Form1 main;
        public static string GenerateNewTempdir()
        {
            string bases = Environment.ExpandEnvironmentVariables("%TEMP%\\PYIH-");
            Random r = new Random();
            bases += r.Next();
            Directory.CreateDirectory(bases);
            return bases;
        }

        public static string PadVersionRight(string inv)
        {
            int ln = Regex.Matches(inv,@"\.").Count+1;
            if (ln == 1)
            {
                return inv + ".0.0.0";
            } else if (ln == 2)
            {
                return inv + ".0.0";
            } else if (ln == 3)
            {
                return inv + ".0";
            } else
            {
                return inv;
            }
        }
        
    }
    public static class Constants
    {
        public static string AppDataPath = Environment.ExpandEnvironmentVariables("%APPDATA%\\PyinstallerHelper");
        public static string DistPath = Environment.ExpandEnvironmentVariables(AppDataPath+"\\dist");
        public static string VersionFileTemplate = @"# UTF-8
#
# For more details about fixed file info 'ffi' see:
# http://msdn.microsoft.com/en-us/library/ms646997.aspx

VSVersionInfo(
  ffi=FixedFileInfo(
    # filevers and prodvers should be always a tuple with four items: (1, 2, 3, 4)
    # Set not needed items to zero 0. Must always contain 4 elements.
    filevers=(#VERSSION),
    prodvers=(#VERSPSION),
    # Contains a bitmask that specifies the valid bits 'flags'r
    mask=0x3f,
    # Contains a bitmask that specifies the Boolean attributes of the file.
    flags=0x0,
    # The operating system for which this file was designed.
    # 0x4 - NT and there is no need to change it.
    OS=0x40004,
    # The general type of file.
    # 0x1 - the file is an application.
    fileType=0x1,
    # The function of the file.
    # 0x0 - the function is not defined for this fileType
    subtype=0x0,
    # Creation date and time stamp.
    date=(0, 0)
    ),
  kids=[
    StringFileInfo(
      [
      StringTable(
        u'040904B0',
        [StringStruct(u'CompanyName', u'#COMPANYNAME'),
        StringStruct(u'FileDescription', u'#DESCR'),
        StringStruct(u'FileVersion', u'#VERSION'),
        StringStruct(u'InternalName', u'#INAME'),
        StringStruct(u'LegalCopyright', u'#CO'),
        StringStruct(u'OriginalFilename', u'#OGF'),
        StringStruct(u'ProductName', u'#PNAME'),
        StringStruct(u'ProductVersion', u'#VERPSION')])
      ]), 
    VarFileInfo([VarStruct(u'Translation', [1033, 1200])])
  ]
)";
    }
    public static class CLISplit
    {
        //100% from SO!

        public static IEnumerable<string> SplitCommandLine(string commandLine)
        {
            bool inQuotes = false;

            return commandLine.Split(c =>
            {
                if (c == '\"')
                    inQuotes = !inQuotes;

                return !inQuotes && c == ' ';
            })
                              .Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
                              .Where(arg => !string.IsNullOrEmpty(arg));
        }
        public static IEnumerable<string> Split(this string str,
                                         Func<char, bool> controller)
        {
            int nextPiece = 0;

            for (int c = 0; c < str.Length; c++)
            {
                if (controller(str[c]))
                {
                    yield return str.Substring(nextPiece, c - nextPiece);
                    nextPiece = c + 1;
                }
            }

            yield return str.Substring(nextPiece);
        }
        public static string TrimMatchingQuotes(this string input, char quote)
        {
            if ((input.Length >= 2) &&
                (input[0] == quote) && (input[input.Length - 1] == quote))
                return input.Substring(1, input.Length - 2);

            return input;
        }
    }
}
