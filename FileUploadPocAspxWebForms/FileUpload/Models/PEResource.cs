using System;
using System.Collections.ObjectModel;
using System.IO;
using static FileUploadPoc.FileUpload.Models.PEFile;

namespace FileUploadPoc.FileUpload.Models
{
    public sealed class PEResource
    {
        public string Language { get; private set; }
        public uint CodePage { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public byte[] Data { get; set; }

        public PEResource(string language)
        {
            Language = language;
        }
    }

    public sealed class PEResourceDirectory
    {
        public uint Characteristics { get; set; }
        public uint Timestamp { get; set; }
        public ushort MajorVersion { get; set; }
        public ushort MinorVersion { get; set; }
        public ushort NumberOfNameEntries { get; set; }
        public ushort NumberOfIdEntries { get; set; }
    }

    public sealed class PEResourceNode
    {
        public string Name { get; private set; }
        public Collection<PEResource> Resources { get; private set; }
        public Collection<PEResourceNode> Children { get; private set; }

        private PEResourceNode(string name)
        {
            Name = name;
            Children = new Collection<PEResourceNode>();
            Resources = new Collection<PEResource>();
        }

        internal static PEResourceNode ReadResources(PESectionHeader sectionHeader)
        {
            if (sectionHeader == null) throw new ArgumentNullException("sectionHeader");

            return ReadResourcesRecursive("Root", 0, sectionHeader.VirtualDataOffset, sectionHeader.Data);
        }

        private static PEResourceNode ReadResourcesRecursive(string name, uint position, uint virtualDataOffset, byte[] resourceData)
        {
            using (MemoryStream Stream = new MemoryStream(resourceData))
            {
                Stream.Position = position;

                BinaryReader Reader = new BinaryReader(Stream);

                PEResourceDirectory Directory = new PEResourceDirectory();

                Directory.Characteristics = Reader.ReadUInt32();
                Directory.Timestamp = Reader.ReadUInt32();
                Directory.MajorVersion = Reader.ReadUInt16();
                Directory.MinorVersion = Reader.ReadUInt16();
                Directory.NumberOfNameEntries = Reader.ReadUInt16();
                Directory.NumberOfIdEntries = Reader.ReadUInt16();

                PEResourceNode Node = new PEResourceNode(name);
                for (int i = 0; i < Directory.NumberOfNameEntries + Directory.NumberOfIdEntries; i++)
                {
                    string Name;
                    uint NameOrIntegerId = Reader.ReadUInt32();
                    if ((NameOrIntegerId & ~0x7FFFFFFF) > 0)
                        Name = ReadResourceName(NameOrIntegerId & 0x7FFFFFFF, resourceData);
                    else
                        Name = NameOrIntegerId.ToString();

                    uint DataOrSubdirectory = Reader.ReadUInt32();
                    if ((DataOrSubdirectory & ~0x7FFFFFFF) > 0)
                        Node.Children.Add(ReadResourcesRecursive(Name, DataOrSubdirectory & 0x7FFFFFFF, virtualDataOffset, resourceData));
                    else
                    {
                        PEResource Resource = new PEResource(Name);
                        ReadResourceDetails(Resource, DataOrSubdirectory, virtualDataOffset, resourceData);
                        Node.Resources.Add(Resource);
                    }
                }
                return Node;
            }
        }

        private static string ReadResourceName(uint nameOffset, byte[] resourceData)
        {
            using (MemoryStream Stream = new MemoryStream(resourceData))
            {
                Stream.Position = nameOffset;

                BinaryReader Reader = new BinaryReader(Stream);

                ushort NameLength = Reader.ReadUInt16();
                byte[] Name = Reader.ReadBytes(NameLength * 2);
                return System.Text.Encoding.Unicode.GetString(Name, 0, Name.Length);
            }
        }

        private static void ReadResourceDetails(PEResource resource, uint dataOffset, uint virtualDataOffset, byte[] resourceData)
        {
            using (MemoryStream Stream = new MemoryStream(resourceData))
            {
                Stream.Position = dataOffset;

                BinaryReader Reader = new BinaryReader(Stream);

                uint OffsetToData = Reader.ReadUInt32() - virtualDataOffset;
                uint SizeOfData = Reader.ReadUInt32();
                uint CodePage = Reader.ReadUInt32();

                resource.Data = PEFile.ReadDataFromOffset(OffsetToData, SizeOfData, resourceData);
                resource.CodePage = CodePage;
            }
        }
    }
}