using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Directum
{
    public class ValidationRule
    {
        public string type { get; set; }
    }

    public class Option
    {
        public string value { get; set; }
        public string text { get; set; }
        public bool? selected { get; set; }
    }

    public class Item
    {
        public string value { get; set; }
        public string label { get; set; }
        public bool? @checked { get; set; }
        public string type { get; set; }
        public Attributes attributes { get; set; }
    }

    public class Attributes
    {
        public string message { get; set; }
        public string name { get; set; }
        public string placeholder { get; set; }
        public bool? required { get; set; }
        public string value { get; set; }
        public string label { get; set; }
        public string @class { get; set; }
        public List<ValidationRule> validationRules { get; set; }
        public bool? disabled { get; set; }
        public bool? @checked { get; set; }
        public string text { get; set; }
        public List<Option> options { get; set; }
        public List<Item> items { get; set; }
    }

    public class Form
    {
        public string name { get; set; }
        public string postmessage { get; set; }
        public List<Item> items { get; set; }
    }
    public class Root
    {
        public Form form { get; set; }
    }
    class Program
    {
        public static void ParseToHTML(Item item)
        {
            if (item.type == null)
            {
                Console.WriteLine("Тип элемента не указан, элемент не будет добавлен");
                return;
            }
            switch (item.type)
            {
                case "filler":
                    Filler_element(item);
                    break;
                case "text":
                    Text_element(item);
                    break;
                case "textarea":
                    Textarea_element(item);
                    break;
                case "checkbox":
                    Checkbox_element(item);
                    break;
                case "button":
                    Button_element(item);
                    break;
                case "select":
                    Select_element(item);
                    break;
                case "radio":
                    Radio_element(item);
                    break;
                default:
                    Console.WriteLine($"Тип элемента {item.type} не найден");
                    return;
            }
        }
        private static void Radio_element(Item item)
        {
            Attributes attributes = item.attributes;
            StringBuilder radioinput = new StringBuilder();
            radioinput.Append($"<input type = \"{item.type}\" ");
            if (attributes.name != null)
                radioinput.Append($"name = \"{attributes.name}\" ");
            if (attributes.required != null && attributes.required != false)
                radioinput.Append("required ");
            if (attributes.@class != null)
                radioinput.Append($"class = \"{attributes.@class}\" ");
            if (attributes.disabled != null && attributes.disabled != false)
                radioinput.Append("disabled ");
            var items = attributes.items;
            foreach (var itemofradio in items)
            {
                String attr = "";
                if (itemofradio.value != null)
                    attr += $"value = \"{itemofradio.value}\" ";
                if (itemofradio.@checked != null && itemofradio.@checked != false)
                    attr += "checked ";
                HTMLcode.Append(radioinput.ToString() + attr + ">");
                if (itemofradio.label != null)
                    HTMLcode.Append(itemofradio.label + '\n');
            }
        }
        private static void Select_element(Item item)
        {
            HTMLcode.Append("<select ");
            Attributes attributes = item.attributes;
            if (attributes.name != null)
                HTMLcode.Append($"name = \"{attributes.name}\" ");
            if (attributes.required != null && attributes.required != false)
                HTMLcode.Append("required ");
            if (attributes.value != null)
                HTMLcode.Append($"value = \"{attributes.value}\" ");
            if (attributes.@class != null)
                HTMLcode.Append($"class = \"{attributes.@class}\" ");
            if (attributes.disabled != null && attributes.disabled != false)
                HTMLcode.Append("disabled ");
            HTMLcode.Append(">\n");
            if (attributes.label != null)
                HTMLcode.Append(attributes.label + '\n');
            if (attributes.options.Count != 0)
            {
                List<Option> options = attributes.options;
                for (int i = 0; i < options.Count; i++)
                {
                    HTMLcode.Append("<option ");
                    if (options[i].selected != null && options[i].selected != false)
                        HTMLcode.Append("selected ");
                    if (options[i].value != null)
                        HTMLcode.Append($"value = \"{options[i].value}\" ");
                    if (options[i].text != null)
                        HTMLcode.Append(">" + options[i].text);
                    else
                        HTMLcode.Append(">");
                    HTMLcode.Append("</option>\n");
                }
            }
            HTMLcode.Append("</select>\n");
        }
        private static void Button_element(Item item)
        {
            HTMLcode.Append($"<input type = \"{item.type}\" ");
            Attributes attributes = item.attributes;
            if (attributes.text != null)
                HTMLcode.Append($"value = \"{attributes.text}\" ");
            if (attributes.@class != null)
                HTMLcode.Append($"class = \"{attributes.@class}\" ");
            HTMLcode.Append(">\n");
        }
        private static void Checkbox_element(Item item)
        {
            HTMLcode.Append($"<input type = \"{item.type}\" ");
            Attributes attributes = item.attributes;
            if (attributes.name != null)
                HTMLcode.Append($"name = \"{attributes.name}\" ");
            if (attributes.required != null && attributes.required != false)
                HTMLcode.Append("required ");
            if (attributes.value != null)
                HTMLcode.Append($"value = \"{attributes.value}\" ");
            if (attributes.@class != null)
                HTMLcode.Append($"class = \"{attributes.@class}\" ");
            if (attributes.disabled != null && attributes.disabled != false)
                HTMLcode.Append("disabled ");
            if (attributes.@checked != null && attributes.@checked != false)
                HTMLcode.Append("checked");
            HTMLcode.Append(">");
            if (attributes.label != null)
                HTMLcode.Append(attributes.label + '\n');
            else HTMLcode.Append('\n');
        }

        private static void Textarea_element(Item item)
        {
            HTMLcode.Append("<textarea ");
            Attributes attributes = item.attributes;
            if (attributes.name != null)
                HTMLcode.Append($"name = \"{attributes.name}\" ");
            if (attributes.required != null && attributes.required != false)
                HTMLcode.Append("required ");
            if (attributes.value != null)
                HTMLcode.Append($"value = \"{attributes.value}\" ");
            String pattern = "";
            if (attributes.validationRules.Count != 0)
            {
                switch (attributes.validationRules[0].type)
                {
                    case "tel":
                        pattern = "^(\\s*)?(\\+)?([- _():=+]?\\d[- _():=+]?){10,14}(\\s*)?$";
                        break;
                    case "email":
                        pattern = "[a-zA-Z0-9]+(?:[._+-][a-zA-Z0-9]+)*)@([a-zA-Z0-9]+(?:[.-][a-zA-Z0-9]+)*[.][a-zA-Z]{2,}";
                        break;
                    case "text":
                        pattern = "+";
                        break;
                    default:
                        break;
                }
            }
            if (item.attributes.placeholder != null)
            {
                HTMLcode.Append($"placeholder = \"{attributes.placeholder}\" ");
                HTMLcode.Append($"pattern = \"{pattern} | ^(? !{ attributes.placeholder})\" ");
            }
            else
            {
                HTMLcode.Append($"pattern = \"{pattern}\" ");

            }
            if (attributes.@class != null)
                HTMLcode.Append($"class = \"{attributes.@class}\" ");
            if (attributes.disabled != null && attributes.disabled != false)
                HTMLcode.Append("disabled ");
            HTMLcode.Append(">");
            if (attributes.label != null)
                HTMLcode.Append(attributes.label);
            HTMLcode.Append("</textarea>\n");
        }

        private static void Text_element(Item item)
        {
            Attributes attributes = item.attributes;
            if (attributes.label != null)
                HTMLcode.Append($"<b>{ attributes.label}</b>\n");
            HTMLcode.Append($"<input type = \"{item.type}\" ");
            if (attributes.name != null)
                HTMLcode.Append($"name = \"{attributes.name}\" ");
            if (attributes.required != null && attributes.required != false)
                HTMLcode.Append("required ");
            if (attributes.value != null)
                HTMLcode.Append($"value = \"{attributes.value}\" ");
            String pattern = "";
            if (attributes.validationRules.Count != 0)
            {
                switch (attributes.validationRules[0].type)
                {
                    case "tel":
                        pattern = "^(\\s*)?(\\+)?([- _():=+]?\\d[- _():=+]?){10,14}(\\s*)?$";
                        break;
                    case "email":
                        pattern = "[a-zA-Z0-9]+(?:[._+-][a-zA-Z0-9]+)*)@([a-zA-Z0-9]+(?:[.-][a-zA-Z0-9]+)*[.][a-zA-Z]{2,}";
                        break;
                    case "text":
                        pattern = "+";
                        break;
                    default:
                        break;
                }
            }
            if (item.attributes.placeholder != null)
            {
                HTMLcode.Append($"placeholder = \"{attributes.placeholder}\" ");
                HTMLcode.Append($"pattern = \"{pattern} | ^(? !{attributes.placeholder})\"");
            }
            else
            {
                HTMLcode.Append($"pattern = \"{pattern}\"");
            }
            if (attributes.@class != null)
                HTMLcode.Append($"class = \"{attributes.@class}\" ");
            if (attributes.disabled != null && attributes.disabled != false)
                HTMLcode.Append("disabled ");
            HTMLcode.Append(">\n");
        }

        public static void Filler_element(Item item)
        {
            HTMLcode.Append(item.attributes.message + "\n");
        }
        public static void StartForm(Form form)
        {
            HTMLcode.Append($"<form name = \"{form.name}\">\n");
        }
        public static StringBuilder HTMLcode = new StringBuilder();
        static void Main(string[] args)
        {
            String JsonFilePath = File.ReadAllText("task.json");
            var Root = JsonSerializer.Deserialize<Root>(JsonFilePath);
            var ItemList = Root.form.items;
            StartForm(Root.form);
            for (int i = 0; i < ItemList.Count; i++)
            {
                ParseToHTML(ItemList[i]);
            }
            HTMLcode.Append("</form>\n");
            String BeginDocument = "<html>\n <body>\n";
            String EndDocument = "</body>\n </html>";
            using FileStream fstream = new FileStream("form.html", FileMode.OpenOrCreate);
            String Result = BeginDocument + HTMLcode.ToString() + EndDocument;
            Console.WriteLine(Result);
            byte[] array = System.Text.Encoding.UTF8.GetBytes(Result);
            fstream.Write(array, 0, array.Length);
            fstream.Close();
            Console.WriteLine("\n\n\nHTML документ успешно создан");
        }
    }
}
