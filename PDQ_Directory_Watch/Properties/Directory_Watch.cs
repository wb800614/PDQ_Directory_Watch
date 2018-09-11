using System;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
            oldFiles.Capacity = files.Length;
            Parallel.ForEach(files, (f) =>
            {
                FileAdditionalInfo faddinfo = new FileAdditionalInfo(f);
                Thread.BeginCriticalRegion();
                oldFiles.Add(faddinfo);
                Thread.EndCriticalRegion();
            });
        }

        private void SetBackTouches()
        {
            foreach(FileAdditionalInfo f in oldFiles)
            {
                f.SetTouch(false);
            }
        }

        private int GetIndexFromOldFiles(FileInfo newfile)
        {
            for (int i = 0; i < oldFiles.Count; i++)
            {
                if (oldFiles[i].datafile.Name.ToUpper() == newfile.Name.ToUpper())
                {
                    oldFiles[i].SetTouch(true);
                    return i;
                }
            }
            return -1;
        }

        public void CheckFiles()
        {
            if (dir.Exists)
            {
                SetBackTouches();

                FileInfo[] newFiles = dir.GetFiles();

                Parallel.ForEach(newFiles, f =>
                {
                    int oldFileIndex = GetIndexFromOldFiles(f);

                    //New File
                    if (oldFileIndex == -1)
                    {
                        FileAdditionalInfo faddinfo = new FileAdditionalInfo(f);
                        Console.WriteLine("New File Created : " + faddinfo.datafile.Name + " with " + faddinfo.lineCount + " lines.");
                        faddinfo.SetTouch(true);
                        oldFiles.Add(faddinfo);
                    }
                    else
                    {
                        if (oldFiles[oldFileIndex].datafile.LastWriteTime != f.LastWriteTime)
                        {
                            FileAdditionalInfo faddinfo = new FileAdditionalInfo(f);
                            faddinfo.SetTouch(true);
                            int lineDiff = faddinfo.lineCount - oldFiles[oldFileIndex].lineCount;
                            Console.WriteLine("Modified File : " + f.Name + ". " + (lineDiff < 0 ? (lineDiff * -1).ToString() + " Lines Remove." : lineDiff.ToString() + " Lines Added."));
                            oldFiles.Remove(oldFiles[oldFileIndex]);
                            oldFiles.Add(faddinfo);
                        }
                    }
                });

                for (int i = 0; i < oldFiles.Count; i++)
                {
                    if (!oldFiles[i].touched)
                    {
                        Console.WriteLine("Deleted File : " + oldFiles[i].datafile.Name);
                        oldFiles.Remove(oldFiles[i]);
                    }
                }
            }
        }
    }
}
