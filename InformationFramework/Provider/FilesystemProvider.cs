using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationFramework.Provider
{
    using Models;
    using System.IO;

    public class FilesystemProvider : ProviderBase, Grabbing
    {
        public const string Directory = "Directory";
        public const string File = "File";

        public FilesystemProvider()
        {
            base.Label = "Dateisystem";
        }

        public IEnumerable<InformationItem> GrabItems()
        {
            return GrabItems(null);
        }
        public IEnumerable<InformationItem> GrabItems(InformationItem item)
        {
            var response = new List<InformationItem>();
            var accessfailure = default(bool);

            //  Erste Ebene (Ermittlung von Laufwerken)
            if (item == null)
            {
                var drives = DriveInfo.GetDrives();
                if (drives != null) {
                    foreach (var drive in drives) {
                        try
                        {
                            response.Add(new InformationItem
                            {
                                Properties = new[]{
                                    new InformationProperty{ ID = InformationProperty.Type, Values = new []{ Directory } },
                                    new InformationProperty{ ID = InformationProperty.Name, Values = new []{ drive.VolumeLabel } },
                                    new InformationProperty{ ID = Directory, Values = new []{ drive.RootDirectory.FullName} }
                                }
                            });
                        }
                        catch (Exception) { }
                    }
                }
            }
            else
            {
                if (item.Properties != null)
                {
                    var property = default(InformationProperty);

                    //  Verzeichnis
                    property = item.Properties.FirstOrDefault(prop => prop.ID == Directory);
                    var directorypath = property == null || property.Values == null ? string.Empty : property.Values.FirstOrDefault() ?? string.Empty;
                    if (!string.IsNullOrEmpty(directorypath))
                    {
                        var directoryinfo = new DirectoryInfo(directorypath);
                        //  Unterverzeichnisse
                        var directories = default(IEnumerable<DirectoryInfo>);
                        try { directories = directoryinfo.GetDirectories(); } 
                        catch(Exception){ accessfailure = true; }

                        if (directories != null)
                        {
                            foreach (var directory in directories)
                            {
                                response.Add(new InformationItem {
                                    Parent = item,
                                    Properties = new[]{
                                        new InformationProperty{ ID = InformationProperty.Type, Values = new []{ Directory } },
                                        new InformationProperty{ ID = Directory, Values = new []{ directory.FullName} }
                                    }
                                });
                            }
                        }

                        //  Dateien
                        var files = default(IEnumerable<FileInfo>);
                        try { files = directoryinfo.GetFiles(); } 
                        catch(Exception){ accessfailure = true;}
                        if (files != null)
                        {
                            foreach (var file in files)
                            {
                                response.Add(new InformationItem
                                {
                                    Parent = item,
                                    Properties = new[]{
                                        new InformationProperty{ ID = InformationProperty.Type, Values = new []{ File } },
                                        new InformationProperty{ ID = Directory, Values = new []{ file.FullName } }
                                    }
                                });
                            }
                        }
                    }
                }
            }

            return accessfailure || !response.Any() ? new[]{ item } : response.ToArray();
        }
    }
}
