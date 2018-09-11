using System;
namespace PDQ_Directory_Watch.Properties
{
    public class FileAdditionalInfo
    {
        public System.IO.FileInfo datafile { get; set; }
        public int LineCount { get; set; }

        public FileAdditionalInfo(System.IO.FileInfo f)
        {
            datafile = f;
            GetData();
        }

        private void GetData()
        {
            System.IO.FileStream f = datafile.OpenRead();
            if (f.CanRead)
            {
                byte [] data = new byte[f.Length];
                f.Read(data, 0, (int)f.Length);
                for (int i = 0; i < data.Length; i++)
                    if (data[i] == '\n')
                        LineCount++;
            }
            f.Close();
            if (LineCount > 0)
                LineCount++;
            return;
        }
    }
}
