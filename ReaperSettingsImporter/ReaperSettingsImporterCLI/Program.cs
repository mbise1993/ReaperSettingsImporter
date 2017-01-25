using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mbise1993.ReaperSettingsImporterCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[args] {0}", args.Length);
            ReaperProject sourceProject = ReaperProject.Load(args[0]);
            Console.WriteLine("[LOAD] Loaded source project {0}", sourceProject.FilePath);

            if(!args[1].Equals("-d", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("[ERROR] No destination projects listed. USAGE: <sourceProject> -d <destProject destProject ...>");
                return;
            }

            IList<ReaperProject> destProjects = new List<ReaperProject>();
            IList<string> trackNames = new List<string>();
            IList<TrackData> sourceTrackData = new List<TrackData>();

            int i = 2;
            while(i < args.Length && !args[i].Equals("-t", StringComparison.InvariantCultureIgnoreCase))
            {
                destProjects.Add(ReaperProject.Load(args[i]));
                Console.WriteLine("[LOAD] Loaded destination project {0}", args[i]);
                i++;
            }

            if(i < args.Length && args[i].Equals("-t", StringComparison.InvariantCultureIgnoreCase))
            {
                i++;
                while(i < args.Length)
                {
                    trackNames.Add($"\"{args[i]}\"");
                    trackNames.Add(args[i]);
                    i++;
                }
            }

            if(trackNames.Count > 0)
            {
                sourceTrackData = sourceProject.ReadTrackData(trackNames);
            }
            else
            {
                sourceTrackData = sourceProject.ReadTrackData();
            }
            Console.WriteLine("[STATUS] Read source track data: {0} tracks found", sourceTrackData.Count);
            

            foreach(ReaperProject destProject in destProjects)
            {
                destProject.WriteTrackData(sourceTrackData);
                Console.WriteLine("[STATUS] Finished writing settings to {0}", destProject.FilePath);
            }
        }
    }
}
