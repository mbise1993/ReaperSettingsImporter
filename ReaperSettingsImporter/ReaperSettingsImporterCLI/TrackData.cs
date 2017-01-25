using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mbise1993.ReaperSettingsImporterCLI
{
    public class TrackData
    {
        #region Properties

        public bool IsMasterTrack { get; set; }
        public string Name { get; set; }
        public string VolPan { get; set; }
        public string MuteSolo { get; set; }
        public string FxChain { get; set; }

        #endregion
    }
}
