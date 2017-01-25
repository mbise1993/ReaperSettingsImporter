using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mbise1993.ReaperSettingsImporterCLI
{
    public class ReaperProject
    {
        #region Private Fields

        private string _filePath;
        private string _projectText;

        #endregion

        #region Properties

        public string FilePath
        {
            get
            {
                return _filePath;
            }
        }

        #endregion

        #region Constructor

        private ReaperProject(string filePath, string projectText)
        {
            _filePath = filePath;
            _projectText = projectText;
        }

        #endregion

        #region Load 

        public static ReaperProject Load(string filePath)
        {
            string projectText = File.ReadAllText(filePath);
            return new ReaperProject(filePath, projectText);
        }

        #endregion

        #region Read Methods

        public IList<TrackData> ReadTrackData(IList<string> trackNames = null)
        {
            IList<TrackData> trackData = new List<TrackData>();

            using (StringReader reader = new StringReader(_projectText))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Trim().StartsWith("<MASTERFXLIST"))
                    {
                        if(trackNames == null || trackNames.Any(tn => tn.Equals("\"MASTER\"")))
                        {
                            trackData.Add(ReadMasterFxList(line, reader));
                        }
                    }
                    else if (line.Trim().StartsWith("<TRACK"))
                    {
                        TrackData track = ReadTrack(reader);
                        if(trackNames == null || trackNames.Any(tn => tn.Equals(track.Name)))
                        {
                            trackData.Add(track);
                        }
                    }
                }
            }

            return trackData;
        }

        private TrackData ReadMasterFxList(string currentLine, StringReader reader)
        {
            TrackData trackData = new TrackData();
            trackData.IsMasterTrack = true;
            trackData.FxChain = ReadFxChain(currentLine, reader);
            return trackData;
        }

        private TrackData ReadTrack(StringReader reader)
        {
            TrackData trackData = new TrackData();
            trackData.IsMasterTrack = false;
            string line = string.Empty;
            bool insideItemTag = false;

            Queue<int> tagQueue = new Queue<int>();
            tagQueue.Enqueue(1);

            while (tagQueue.Count > 0)
            {
                line = reader.ReadLine();
                string[] tokens = line.Trim().Split(new char[] { ' ' }, 2);

                if(tokens[0] == "NAME" && !insideItemTag)
                {
                    trackData.Name = tokens[1];
                }
                else if(tokens[0] == "VOLPAN" && !insideItemTag)
                {
                    trackData.VolPan = tokens[1];
                }
                else if(tokens[0] == "MUTESOLO" && !insideItemTag)
                {
                    trackData.MuteSolo = tokens[1];
                }
                else if(tokens[0] == "<FXCHAIN" && !insideItemTag)
                {
                    trackData.FxChain = ReadFxChain(line, reader);
                }
                else if (line.Contains("<"))
                {
                    tagQueue.Enqueue(1);
                    if (line.Contains("ITEM"))
                    {
                        insideItemTag = true;
                    }
                }
                else if (line.Contains(">"))
                {
                    tagQueue.Dequeue();
                    if (insideItemTag)
                    {
                        insideItemTag = false;
                    }
                }
            }

            return trackData;
        }

        private string ReadFxChain(string currentLine, StringReader reader)
        {
            string fxChain = currentLine;
            string line = string.Empty;

            Queue<int> tagQueue = new Queue<int>();
            tagQueue.Enqueue(1);

            while(tagQueue.Count > 0)
            {
                line = reader.ReadLine();
                fxChain += Environment.NewLine + line;

                if(line.Contains("<"))
                {
                    tagQueue.Enqueue(1);
                }
                if(line.Contains(">"))
                {
                    tagQueue.Dequeue();
                }
            }

            return fxChain;
        }

        #endregion

        #region Write Methods

        public void WriteTrackData(IList<TrackData> trackData)
        {
            string outputText = string.Empty;
            int masterFxListPosition = 0;

            using (StringReader reader = new StringReader(_projectText))
            {
                string line = string.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    if(line.Trim().StartsWith("MASTER_SEL"))
                    {
                        outputText += line + "\r\n";
                        masterFxListPosition = outputText.Length;
                    }
                    else if (line.Trim().StartsWith("<MASTERFXLIST"))
                    {
                        AdvanceReaderPastElement(reader);
                    }
                    else if (line.Trim().StartsWith("<TRACK"))
                    {
                        outputText += line + Environment.NewLine;
                        WriteTrackData(ref outputText, reader, trackData);
                    }
                    else
                    {
                        outputText += line + Environment.NewLine;
                    }
                }

                if(trackData.Any(td => td.IsMasterTrack))
                {
                    outputText = outputText.Insert(masterFxListPosition, trackData.Single(td => td.IsMasterTrack).FxChain.TrimEnd() + "\r\n");
                }
            }

            //File.WriteAllText(@"C:\Users\Matt\Documents\Recording\REAPER Projects\OUTPUT.rpp", outputText);
            File.WriteAllText(_filePath, outputText);
        }

        private void WriteTrackData(ref string outputText, StringReader reader, IList<TrackData> trackData)
        {
            string line = string.Empty;
            TrackData matchData = null;
            int fxChainPosition = 0;
            bool insideItemTag = false;

            Queue<int> tagQueue = new Queue<int>();
            tagQueue.Enqueue(1);

            while(tagQueue.Count > 0)
            {
                line = reader.ReadLine();

                if(matchData != null)
                {
                    if(line.Trim().StartsWith("VOLPAN") && !insideItemTag)
                    {
                        outputText += "    VOLPAN " + matchData.VolPan + Environment.NewLine;
                    }
                    else if(line.Trim().StartsWith("MUTESOLO") && !insideItemTag)
                    {
                        outputText += "    MUTESOLO " + matchData.MuteSolo + Environment.NewLine;
                    }
                    else if(line.Trim().StartsWith("MAINSEND") || line.Trim().StartsWith("HWOUT"))
                    {
                        outputText += line + "\r\n";
                        fxChainPosition = outputText.Length;
                    }
                    else if(line.Trim().StartsWith("<FXCHAIN") && !insideItemTag)
                    {
                        AdvanceReaderPastElement(reader);
                        line = string.Empty;
                    }
                    else
                    {
                        outputText += line + Environment.NewLine;
                    }
                }
                else
                {
                    if (line.Trim().StartsWith("NAME"))
                    {
                        string name = line.Trim().Split(new char[] { ' ' }, 2)[1];
                        if (name != null && trackData.Any(td => name.Equals(td.Name, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            matchData = trackData.Single(td => name.Equals(td.Name, StringComparison.CurrentCultureIgnoreCase));
                            outputText += line + Environment.NewLine;
                        }
                        else
                        {
                            outputText += line + Environment.NewLine;
                            return;
                        }
                    }
                    else
                    {
                        outputText += line + Environment.NewLine;
                    }
                }

                if(line.Contains("<"))
                {
                    tagQueue.Enqueue(1);
                    if(line.Contains("ITEM"))
                    {
                        insideItemTag = true;
                    }
                }
                else if(line.Contains(">"))
                {
                    tagQueue.Dequeue();
                    if(insideItemTag)
                    {
                        insideItemTag = false;
                    }
                }
            }

            if(matchData != null && !string.IsNullOrEmpty(matchData.FxChain))
            {
                outputText = outputText.Insert(fxChainPosition, matchData.FxChain.TrimEnd() + "\r\n");
            }
        }

        #endregion

        #region Util Methods

        private void AdvanceReaderPastElement(StringReader reader)
        {
            Queue<int> tagQueue = new Queue<int>();
            tagQueue.Enqueue(1);

            string line = string.Empty;
            while (tagQueue.Count > 0)
            {
                line = reader.ReadLine();

                if (line.Contains("<"))
                {
                    tagQueue.Enqueue(1);
                }
                if (line.Contains(">"))
                {
                    tagQueue.Dequeue();
                }
            }
        }

        #endregion
    }
}
