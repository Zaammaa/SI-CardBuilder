using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes
{
    public class ElementSet
    {
        public enum Element
        {
            None,
            Sun,
            Moon,
            Fire,
            Air,
            Water,
            Earth,
            Plant,
            Animal
        }

        private List<Element> Elements;

        public ElementSet(List<Element> el)
        {
            Elements = el;
        }

        public bool Has(Element e)
        {
            return Elements.Contains(e);
        }

        public List<Element> GetElements()
        {
            return Elements;
        }

        public static List<Element> GetAllElements()
        {
            return new List<Element>()
            {
                Element.Sun,
                Element.Moon,
                Element.Fire,
                Element.Air,
                Element.Water,
                Element.Earth,
                Element.Plant,
                Element.Animal
            };
        }

        /// <summary>
        /// Reads a string of comma-seperated elements and parses it for this class
        /// </summary>
        /// <param name="elementString">a comma seperated list of elements</param>
        public static ElementSet ElementSetFromElementString(string elementString)
        {
            
            string formattedString = elementString.Trim().Replace(" ","");
            string[] elements = formattedString.Split(",");
            List<Element> elList = new List<Element>();
            foreach(string element in  elements)
            {
                if (!element.Equals(""))
                {
                    Element el = (Element)Enum.Parse(typeof(Element), element, true);
                    elList.Add(el);
                }
            }
            return new ElementSet(elList);
        }

        public override string ToString()
        {
            return String.Join(",",Elements.ToArray()).ToLower();
        }
    }
}
