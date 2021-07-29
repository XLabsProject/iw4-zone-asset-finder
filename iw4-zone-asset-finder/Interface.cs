using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iw4_zone_asset_finder
{
    public class Interface : LouveSystems.CommandLineInterface.CommandLineInterface
    {
        const int READLINE_BUFFER_SIZE = 8192;

        public Interface() : base("IW4 Zone Asset Finder",null, true)
        {
            SetDefaultCommand("HELP");

            SetCommand("UPDATE", new Commands.Update());
            SetCommand("RELOAD", new Commands.Reload());

            SetCommand("SEARCH", new Commands.FindAsset());
            SetCommand("FIND", new Commands.FindPreciseAsset());

            SetCommand("BUILDREQ", new Commands.BuildRequirements());
            SetCommand("IGNORE", new Commands.Ignore());
            SetCommand("IWDFILES", new Commands.IwdFiles());

            SetDefaultCommand("EXIT");

            Run();
        }

        public override string ReadInput()
        {
            Stream inputStream = Console.OpenStandardInput(READLINE_BUFFER_SIZE); // declaring a new stream to read data, max readline size
            byte[] bytes = new byte[READLINE_BUFFER_SIZE]; // defining array with the max size
            int outputLength = inputStream.Read(bytes, 0, READLINE_BUFFER_SIZE); //reading
            char[] chars = Encoding.UTF7.GetChars(bytes, 0, outputLength); // casting it to a string
            return new string(chars).Trim(); // returning
        }

        protected override void Initialize()
        {
            base.Initialize();

            WriteLine("Loading dependency graph...");
            DependencyGraph.Load();
            WriteLine("Done!");
        }
    }
}
