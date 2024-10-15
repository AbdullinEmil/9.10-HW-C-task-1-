using System;
using System.IO;
using System.Reflection;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class IniFileAttribute : Attribute
{
    public string FileName { get; }

    public IniFileAttribute(string fileName)
    {
        FileName = fileName;
    }
}

public class Config
{
    [IniFileAttribute("config1.ini")]
    public string Property1 { get; set; }

    [IniFileAttribute("config2.ini")]
    public int Property2 { get; set; }
}

public class Program
{
    public static void Main(string[] args)
    {
        Config config = new Config();
        LoadIniValues(config);

        Console.WriteLine($"Property1: {config.Property1}");
        Console.WriteLine($"Property2: {config.Property2}");
    }

    public static void LoadIniValues(object obj)
    {
        Type type = obj.GetType();
        foreach (var property in type.GetProperties())
        {
            var iniFileAttribute = property.GetCustomAttribute<IniFileAttribute>();
            if (iniFileAttribute != null)
            {
                string fileName = iniFileAttribute.FileName;
                string value = ReadValueFromIni(fileName, property.Name);
                property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
            }
        }
    }

    public static string ReadValueFromIni(string fileName, string key)
    {
        string value = "";
        if (File.Exists(fileName))
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith(key + "="))
                    {
                        value = line.Substring(key.Length + 1);
                        break;
                    }
                }
            }
        }
        return value;
    }
}