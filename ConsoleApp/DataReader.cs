using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp
{
    public class DataReader
    {
        public void ImportAndPrintData(string fileToImport, bool printData = true)
        {
            try
            {
                if (string.IsNullOrEmpty(fileToImport))
                {
                    Console.WriteLine("Brak danych we wskazanym pliku");
                    Console.ReadLine();
                    return;
                }

                var ImportedObjects = new List<ImportedObject>() { };

                var importedLines = new List<string>();

                using (var reader = new StreamReader(fileToImport))
                {
                    var line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                            importedLines.Add(line);
                    }
                }

                for (int i = 0; i < importedLines.Count; i++)
                {
                    var values = importedLines[i].Split(';');

                    if (values.Length == 7)
                    {
                        ImportedObjects.Add(new ImportedObject
                        {
                            Type = values[0].Trim().Replace(Environment.NewLine, "").ToUpper(),
                            Name = values[1].Trim().Replace(Environment.NewLine, ""),
                            Schema = values[2].Trim().Replace(Environment.NewLine, ""),
                            ParentName = values[3].Trim().Replace(Environment.NewLine, ""),
                            ParentType = values[4].Trim().Replace(Environment.NewLine, ""),
                            DataType = values[5].Trim().Replace(Environment.NewLine, ""),
                            IsNullable = values[6].Trim().Replace(Environment.NewLine, "")
                        });
                    }
                }

                for (int i = 0; i < ImportedObjects.Count; i++)
                {
                    var importedObject = ImportedObjects.ToArray()[i];
                    foreach (var x in ImportedObjects)
                    {
                        if (x.ParentType == importedObject.Type && x.ParentName == importedObject.Name)
                        {
                            importedObject.NumberOfChildren += 1;
                        }
                    }
                }

                foreach (var database in ImportedObjects.Where(a => a.Type == "DATABASE").ToList())
                {
                    Console.WriteLine("Database '{0}' ({1} tables)", database.Name, database.NumberOfChildren);

                    foreach (var table in ImportedObjects.Where(a => a.ParentType == database.Type.ToUpper() && a.ParentName == database.Name).ToList())
                    {
                        Console.WriteLine("\tTable '{0}.{1}' ({2} columns)", table.Schema, table.Name, table.NumberOfChildren);

                        foreach (var column in ImportedObjects.Where(a => a.ParentType == table.Type.ToUpper() && a.ParentName == table.Name).ToList())
                        {
                            Console.WriteLine("\t\tColumn '{0}' with {1} data type {2}", column.Name, column.DataType, column.IsNullable == "1" ? "accepts nulls" : "with no nulls");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.ReadLine();
        }
    }

    class ImportedObject : ImportedObjectBaseClass
    {
        public string Schema { get; set; }
        public string ParentName { get; set; }
        public string ParentType { get; set; }
        public string DataType { get; set; }
        public string IsNullable { get; set; }
        public double NumberOfChildren { get; set; }
    }

    class ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
