using System.Reflection;
using System.Windows;
using System.Windows.Markup;

[assembly: AssemblyTitle("NZazu")]
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsPrefix("http://schemas.nzazu.com/wpf/xaml/nzazu", "nz")]
[assembly: XmlnsDefinition("http://schemas.nzazu.com/wpf/xaml/nzazu", "NZazu")]