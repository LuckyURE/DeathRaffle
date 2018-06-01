using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace CelebScraper
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a combined list and collection of Form Elements.
    /// </summary>
    public class FormElementCollection : Dictionary<string, string>
    {
        /// <inheritdoc />
        /// <summary>
        /// Constructor. Parses the HtmlDocument to get all form input elements. 
        /// </summary>
        public FormElementCollection(HtmlDocument htmlDoc)
        {
            var inputs = htmlDoc.DocumentNode.Descendants("input");
            foreach (var element in inputs)
            {
                AddInputElement(element); 
            }

            var menus = htmlDoc.DocumentNode.Descendants("select");
            foreach (var element in menus)
            {
                AddMenuElement(element); 
            }

            var textareas = htmlDoc.DocumentNode.Descendants("textarea");
            foreach (var element in textareas)
            {
                AddTextareaElement(element);
            }
        }

        /// <summary>
        /// Assembles all form elements and values to POST. Also html encodes the values.  
        /// </summary>
        public string AssemblePostPayload()
        {
            var sb = new StringBuilder();
            foreach (var element in this)
            {
                var value = System.Web.HttpUtility.UrlEncode(element.Value);
                sb.Append("&" + element.Key + "=" + value);
            }
            return sb.ToString().Substring(1);
        }
        
        private void AddInputElement(HtmlNode element)
        {
            var name = element.GetAttributeValue("name", "");
            var value = element.GetAttributeValue("value", "");
            var type = element.GetAttributeValue("type", "");            

            if (string.IsNullOrEmpty(name)) return;

            switch (type.ToLower())
            {
                case "checkbox": 
                case "radio":
                    if (!ContainsKey(name)) Add(name, "");
                    var isChecked = element.GetAttributeValue("checked", "unchecked"); 
                    if (!isChecked.Equals("unchecked")) this[name] = value;
                    break; 
                default: 
                    Add(name, value); 
                    break;
            }            
        }
        
        private void AddMenuElement(HtmlNode element)
        {
            var name = element.GetAttributeValue("name", "");
            var options = element.Descendants("option").ToList();

            if (string.IsNullOrEmpty(name)) return;

            // choose the first option as default
            var firstOp = options.First();
            var defaultValue = firstOp.GetAttributeValue("value", firstOp.NextSibling.InnerText); 

            Add(name, defaultValue); 

            // check if any option is selected
            foreach (var option in options)
            {
                var selected = option.GetAttributeValue("selected", "notSelected");
                if (selected.Equals("notSelected")) continue;
                var selectedValue = option.GetAttributeValue("value", option.NextSibling.InnerText);
                this[name] = selectedValue;
            }
        }
        
        private void AddTextareaElement(HtmlNode element)
        {
            var name = element.GetAttributeValue("name", "");
            if (string.IsNullOrEmpty(name)) return;
            Add(name, element.InnerText);
        }
    }
}