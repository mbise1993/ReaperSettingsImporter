using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mbise1993.ReaperProjectUtil.Model
{
    public class Element
    {
        #region Properties

        public string Tag
        {
            get; private set;
        }

        public string Header
        {
            get; private set;
        }

        public IDictionary<string, string> Fields
        {
            get; private set;
        }

        public IList<Element> Elements
        {
            get; private set;
        }

        #endregion

        #region Constructors

        public Element(string text)
        {
            Fields = new Dictionary<string, string>();
            Elements = new List<Element>();

            ParseText(text);
        }

        #endregion

        #region Private Methods

        private void ParseText(string text)
        {
            text = text.Trim();

            if(!text.StartsWith("<"))
            {
                throw new FormatException("Invalid format: element text must begin with '<'");
            }

            using (StringReader reader = new StringReader(text))
            {
                string line = reader.ReadLine().Trim();

                string[] elementTokens = line.Split(new char[] { ' ' }, 2);
                Tag = elementTokens[0].Remove(0, 1);
                Header = elementTokens[1];

                while((line = reader.ReadLine().Trim()) != null)
                {
                    // If line starts with '<', it is another Element
                    if(line.StartsWith("<"))
                    {
                        Queue<int> tagQueue = new Queue<int>();
                        tagQueue.Enqueue(1);
                        string elementText = line;
                        while(true)
                        {
                            line = reader.ReadLine().Trim();
                            elementText += line;

                            if(line.StartsWith("<"))
                            {
                                tagQueue.Enqueue(1);
                            }
                            else if(line == ">")
                            {
                                tagQueue.Dequeue();
                            }

                            if(tagQueue.Count == 0)
                            {
                                break;
                            }
                        }

                        Elements.Add(new Element(elementText));
                    }
                    else
                    {
                        string[] tokens = line.Split(new char[] { ' ' }, 2);
                        Fields[tokens[0]] = tokens[1];
                    }
                }
            }
        }

        #endregion

        #region Overridden Methods

        public override string ToString()
        {
            string str = string.Format("<{0} {1}", Tag, Header);
            
            foreach(KeyValuePair<string, string> field in Fields)
            {
                str += string.Format("{0} {1}", field.Key, field.Value);
            }

            foreach(Element element in Elements)
            {
                str += element.ToString();
            }

            return str;
        }

        #endregion
    }
}
