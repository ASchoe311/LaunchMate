using Microsoft.Win32;
using Shell32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LaunchMate.Utilities
{
    public class AppSelector
    {
        public static Tuple<string, string, string> SelectApp()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.DereferenceLinks = false;
            openFileDialog.Filter = "Executable file (.exe)|*.exe";


            Nullable<bool> result = openFileDialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                string lnkName = null;
                string targetname = openFileDialog.FileName;
                string args = string.Empty;
                if (openFileDialog.FileName.Contains(".lnk"))
                {
                    //logger.Debug("File chosen is a shortcut, trying to extract target and args");
                    // Open document
                    lnkName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    string pathOnly = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                    string filenameOnly = System.IO.Path.GetFileName(openFileDialog.FileName);

                    Shell shell = new Shell();
                    Shell32.Folder folder = shell.NameSpace(pathOnly);
                    FolderItem folderItem = folder.ParseName(filenameOnly);
                    if (folderItem != null)
                    {
                        Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
                        targetname = link.Target.Path;  // <-- main difference
                        args = link.Arguments;
                        if (targetname.StartsWith("{"))
                        { // it is prefixed with {54A35DE2-guid-for-program-files-x86-QZ32BP4}
                            int endguid = targetname.IndexOf("}");
                            if (endguid > 0)
                            {
                                targetname = "C:\\program files (x86)" + targetname.Substring(endguid + 1);
                            }
                        }
                        //string file = LnkToFile(openFileDialog.FileName);
                    }
                }
                return new Tuple<string, string, string>(targetname, args, lnkName);
            }
            return null;
        }
    }
}
