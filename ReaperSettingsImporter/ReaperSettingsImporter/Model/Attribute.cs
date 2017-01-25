using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mbise1993.ReaperProjectUtil.Model
{
    public class Attribute
    {
        #region Properties

        public string Key { get; private set; }
        public string Value { get; private set; }

        #endregion

        #region Constructor

        public Attribute(string text)
        {
            ParseText(text);
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"{Key} {Value}{Environment.NewLine}";
        }

        #endregion

        #region Private Methods

        private void ParseText(string text)
        {
            string[] tokens = text.Split(new char[] { ' ' }, 2);
            Key = tokens[0];
            Value = tokens[1];
        }

        #endregion
    }
}
