using System.Linq;
using System.Xml.Linq;
using McMaster.Extensions.CommandLineUtils;

namespace EditCsProj
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.HelpOption();

            var addExisting = app.Option("-ae|--addexisting <element>", "Add an xml element to existing ItemGroup", CommandOptionType.MultipleValue);
            var addNew = app.Option("-ai|--add <element>", "Add an xml element to new ItemGroup", CommandOptionType.MultipleValue);
            var addNewToPropertyGroup = app.Option("-ap|--addpropertygroup <element>", "Add an xml element to existing PropertyGroup", CommandOptionType.MultipleValue);

            var filePath = app.Option("-f|--file <csproj>", "The path to the csproj", CommandOptionType.SingleValue).IsRequired().Accepts(builder => builder.ExistingFile());

            app.Command("updateattribute", updateCmd =>
            {
                var nameElementArgument = updateCmd.Argument("elementname", "Name of element of where attribute exists").IsRequired();
                var attributeNameArgument = updateCmd.Argument("attributename", "Name of attribute to identify").IsRequired();
                var attributeNameValueArgument = updateCmd.Argument("attributevalue", "Value of named attribute to filter on").IsRequired();

                var nameArgument = updateCmd.Argument("name", "Name of attribute to update with given value").IsRequired();
                var valueArgument = updateCmd.Argument("value", "Value to set the attribute").IsRequired();

                updateCmd.HelpOption();

                updateCmd.OnExecute(() =>
                {
                    var csProjPath = filePath.Value();
                    var xmlDoc = XDocument.Load(csProjPath);
                    xmlDoc.Descendants(nameElementArgument.Value).FirstOrDefault(x => x.Attribute(attributeNameArgument.Value).Value == attributeNameValueArgument.Value).Attribute(nameArgument.Value)
                        .Value = valueArgument.Value;
                    xmlDoc.Save(csProjPath);
                    return 0;
                });
            });

            app.Command("updateelement", updateCmd =>
            {
                var nameArgument = updateCmd.Argument("name", "Name of element to update with given value").IsRequired();
                var valueArgument = updateCmd.Argument("value", "Value to set the element").IsRequired();

                updateCmd.HelpOption();

                updateCmd.OnExecute(() =>
                {
                    var csProjPath = filePath.Value();
                    var xmlDoc = XDocument.Load(csProjPath);
                    xmlDoc.Descendants(nameArgument.Value).First().Value = valueArgument.Value;
                    xmlDoc.Save(csProjPath);
                    return 0;
                });
            });

            app.OnExecute(() =>
            {
                XDocument xmlDoc = null;

                if (addExisting.HasValue() || addNew.HasValue() || addNewToPropertyGroup.HasValue())
                {
                    xmlDoc = XDocument.Load(filePath.Value());
                }

                var changed = false;
                if (addNew.HasValue())
                {
                    var element = new XElement("ItemGroup");

                    foreach (var newValue in addNew.Values)
                    {
                        var ele = XElement.Parse(newValue);
                        element.Add(ele);
                    }

                    xmlDoc.Root.Add(element);
                    changed = true;
                }

                if (addExisting.HasValue())
                {
                    var element = xmlDoc.Descendants("ItemGroup").Last();

                    foreach (var newValue in addExisting.Values)
                    {
                        var ele = XElement.Parse(newValue);
                        element.Add(ele);
                    }

                    changed = true;
                }
                
                if (addNewToPropertyGroup.HasValue())
                {
                    var element = xmlDoc.Descendants("PropertyGroup").First();

                    foreach (var newValue in addNewToPropertyGroup.Values)
                    {
                        var ele = XElement.Parse(newValue);
                        element.Add(ele);
                    }

                    changed = true;
                }

                if (changed)
                {
                    xmlDoc.Save(filePath.Value());
                }

                return 0;
            });

            app.Execute(args);
        }
    }
}