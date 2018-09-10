using System;
using System.IO;
using System.Windows.Forms;

namespace PDQ_Directory_Watch
{
    class MainClass
    {
        private static System.IO.FileInfo[] oldFiles;
        private static System.IO.FileInfo[] newFiles;

        public static void Main(string[] args)
        {
            while(true)
            {
                OpenAndGrabNewFiles();
                CheckChangesOnNewFiles();
                CheckChangesOnOldFiles();
            }
        }

        public static void OpenAndGrabNewFiles()
        {
            System.IO.DirectoryInfo dir = new DirectoryInfo("/Users/wesleybook/Desktop/Fall2018/cs425/Homework0");
            if (oldFiles == null || oldFiles.Length == 0)
            {
                newFiles = dir.GetFiles();
                oldFiles = newFiles;
            }
            else
            {
                oldFiles = newFiles;
                newFiles = dir.GetFiles();
            }
            return;
        }

        public static void CheckChangesOnNewFiles()
        {
            //Checking New Files
            for (int i = 0; i < newFiles.Length; i++)
            {
                int oldFileIndex = GetPreviousFile(i);

                //New File
                if (oldFileIndex == -1)
                {
                    System.Console.WriteLine("New File Created: " + newFiles[i].Name);
                }
                else 
                {
                    if (newFiles[i].LastWriteTime != oldFiles[oldFileIndex].LastWriteTime)
                    {

                        System.Console.WriteLine("File Modification: " + newFiles[i].Name + ". " + (GetNumberOfLines(newFiles[i]) - GetNumberOfLines(oldFiles[oldFileIndex])).ToString() + " lines added.");
                    }
                }
            }
            return;
        }

        public static void CheckChangesOnOldFiles()
        {
            return;
        }

        public static int GetPreviousFile(int index)
        {
            for (int i = 0; i < oldFiles.Length; i++)
            {
                if (oldFiles[i].CreationTime == newFiles[index].CreationTime
                    && oldFiles[i].Name == newFiles[index].Name)
                    return i;
            }
            return -1;
        }

        public static int GetNumberOfLines(FileInfo file)
        {
            FileStream f = file.OpenRead();
            int count = 0;
            if (f.CanRead)
            {
                byte[] data = new byte[f.Length+1];
                f.Read(data,0,(int)f.Length);
                for (int i = 0; i < data.Length; i++)
                    if (data[i] == '\n')
                        count++;
            }
            return count;
        }
    }
}
