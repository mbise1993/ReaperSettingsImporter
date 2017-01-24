using mbise1993.ReaperProjectUtil.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaperSettingsImporterCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Project sourceProject = new Project(args[1]);

            IList<Project> destProjects = new List<Project>();
            foreach(string destProjectPath in args.Skip(2).ToArray())
            {
                destProjects.Add(new Project(destProjectPath));
            }

            IList<TrackInfo> trackInfos = new List<TrackInfo>();
            foreach(Element track in sourceProject.GetTracks())
            {
                string name = sourceProject.GetTrackParam(track, TrackParams.NAME);
                string volPan = sourceProject.GetTrackParam(track, TrackParams.VOLPAN);
                string muteSolo = sourceProject.GetTrackParam(track, TrackParams.MUTESOLO);
                Element fxChain = sourceProject.GetTrackFxChain(track);

                trackInfos.Add(new TrackInfo(name, volPan, muteSolo, fxChain));
            }

            foreach(Project destProject in destProjects)
            {
                foreach (TrackInfo trackInfo in trackInfos)
                {
                    Element destTrack = destProject.GetTracks(trackInfo.Name)[0];

                    string volPanValue = destProject.GetTrackParam(destTrack, TrackParams.VOLPAN);
                    destProject.SetTrackParam(destTrack, TrackParams.VOLPAN, volPanValue);

                    string muteSoloValue = destProject.GetTrackParam(destTrack, TrackParams.MUTESOLO);
                    destProject.SetTrackParam(destTrack, TrackParams.MUTESOLO, muteSoloValue);

                    destProject.UpdateTrackFxChain(trackInfo.Name, trackInfo.FxChain);
                }

                destProject.Save();
            }
        }

        public struct TrackInfo
        {
            public string Name;
            public string VolPan;
            public string MuteSolo;
            public Element FxChain;

            public TrackInfo(string name, string volPan, string muteSolo, Element fxChain)
            {
                Name = name;
                VolPan = volPan;
                MuteSolo = muteSolo;
                FxChain = fxChain;
            }
        }
    }
}
