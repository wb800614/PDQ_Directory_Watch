using System;
using System.Windows.Forms;
using System.IO;
using PDQ_Directory_Watch.Properties;
using System.Collections.Generic;

namespace PDQ_Directory_Watch.Properties
{
    public class Directory_Watch
    {
        private System.IO.DirectoryInfo dir { get; set; }
        private List<FileAdditionalInfo> oldFiles { get; set; }

        public Directory_Watch(string path)
        {
            oldFiles = new List<FileAdditionalInfo>();
            SetPath(path);
        }

        private void SetPath(string path)
        {
            dir = new System.IO.DirectoryInfo(path);
            if (dir.Exists)
            {
                InitializeFiles();
            }
            else
            {
                Console.WriteLine("Folder path " + path + " does not exist.");
            }
        }

        private void InitializeFiles()
        {
            FileInfo[] files = dir.GetFiles();
            foreach(FileInfo f in files)
            {
                FileAdditionalInfo faddinfo = new FileAdditionalInfo(f);
                oldFiles.Add(faddinfo);
            }
        }

        private int GetIndexFromOldFiles(FileInfo newfile)
        {
            for (int i = 0; i < oldFiles.Count; i++)
            {
                if (oldFiles[i].datafile.Name == newfile.Name)
                    return i;
            }
            return -1;
        }

        public void CheckFiles()
        {
            if (dir.Exists)
            {
                FileInfo[] newFiles = dir.GetFiles();

                for (int i = 0; i < newFiles.Length; i++)
                {
                    int oldFileIndex = GetIndexFromOldFiles(newFiles[i]);

                    //New File
                    if (oldFileIndex == -1)
                    {
                        Console.WriteLine("New File Created : " + newFiles[i].Name);
                        FileAdditionalInfo faddinfo = new FileAdditionalInfo(newFiles[i]);
                        oldFiles.Add(faddinfo);
                    }
                    else
                    {
                        if (oldFiles[oldFileIndex].datafile.LastWriteTime != newFiles[i].LastWriteTime)
                        {
                            FileAdditionalInfo faddinfo = new FileAdditionalInfo(newFiles[i]);
                            int lineDiff = faddinfo.LineCount - oldFiles[oldFileIndex].LineCount;
                            Console.WriteLine("Modified File : " + newFiles[i].Name + ". " + (lineDiff < 0 ? (lineDiff * -1).ToString() + " Lines Remove." : lineDiff.ToString() + " Lines Added."));
                            oldFiles.Remove(oldFiles[oldFileIndex]);
                            oldFiles.Add(faddinfo);
                        }
                    }
                }
            }
        }
    }
}
