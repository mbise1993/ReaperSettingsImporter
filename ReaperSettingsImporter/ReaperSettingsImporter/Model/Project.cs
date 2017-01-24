using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mbise1993.ReaperProjectUtil.Model
{
    public class Project
    {
        #region Private Fields

        private string _filePath;
        private string _fileText;

        #endregion

        #region Properties

        public Element ProjectRoot
        {
            get; private set;
        }

        #endregion

        #region Constructor

        public Project(string filePath)
        {
            _filePath = filePath;
            _fileText = File.ReadAllText(_filePath);
            ProjectRoot = new Element(_fileText);
        }

        #endregion

        #region Public Methods

        public IList<Element> GetTracks(string trackName = null)
        {
            IList<Element> tracks = ProjectRoot.Elements.Where(e => e.Tag == "TRACK").ToList();
            if (!string.IsNullOrEmpty(trackName))
            {
                return tracks.Where(t => GetTrackParam(t, TrackParams.NAME) == trackName).ToList();
            }
            else
            {
                return tracks;
            }
        }

        public string GetTrackParam(Element track, TrackParams trackParam)
        {
            if(track.Tag != "TRACK")
            {
                throw new ArgumentException("Element must be a track");
            }

            return track.Fields[Enum.GetName(typeof(TrackParams), trackParam)];
        }

        public void SetTrackParam(Element track, TrackParams trackParam, string value)
        {
            if (track.Tag != "TRACK")
            {
                throw new ArgumentException("Element must be a track");
            }

            track.Fields[Enum.GetName(typeof(TrackParams), trackParam)] = value;
        }

        public Element GetTrackFxChain(Element track)
        {
            if(!track.Elements.Any(e => e.Tag == "FXCHAIN"))
            {
                return null;
            }

            return track.Elements.Single(e => e.Tag == "FXCHAIN");
        }

        public void UpdateTrackFxChain(string trackName, Element fxChain)
        {
            IList<Element> tracks = GetTracks(trackName);
            if(tracks.Count == 0)
            {
                throw new ArgumentException("No tracks found with name '" + trackName + "'");
            }

            Element track = tracks[0];
            Element oldFxChain = GetTrackFxChain(track);
            int index = track.Elements.IndexOf(oldFxChain);
            track.Elements.RemoveAt(index);
            track.Elements.Insert(index, fxChain);
        }

        public void Save()
        {
            File.WriteAllText(_filePath, ProjectRoot.ToString());
        }

        #endregion
    }
}
