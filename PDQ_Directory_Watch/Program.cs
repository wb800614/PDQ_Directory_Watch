using System;
using System.Threading;
using PDQ_Directory_Watch.Properties;

namespace PDQ_Directory_Watch
{
    class MainClass
    {

        public static void Main(string[] args)
        {
            Directory_Watch watcher = new Directory_Watch("/Users/wesleybook/Desktop/Fall2018/vico2561");
            watcher.Watch();
        }
    }
}
