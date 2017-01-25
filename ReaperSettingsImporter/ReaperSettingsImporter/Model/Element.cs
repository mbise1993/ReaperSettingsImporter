using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mbise1993.ReaperProjectUtil.Model
{
    public class Element
    {
        #region Properties

        public string Tag { get; private set; }
        public IList<Attribute> Attributes { get; private set; }

        #endregion

        #region Constructor

        public Element(string text)
        {
            ParseText(text);
        }

        public Element(string tag, IList<Attribute> attributes)
        {
            Tag = tag;
            Attributes = attributes;
        }

        #endregion

        #region Protected Methods

        protected virtual void ParseText(string text)
        {

        }

        #endregion
    }
}
