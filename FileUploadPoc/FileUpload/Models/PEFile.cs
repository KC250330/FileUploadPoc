using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FileUploadPoc.FileUpload.Models
{
        public sealed class PEHeader
        {
            public uint Signature { get; set; }
            public ushort Machine { get; set; }
            public ushort NumberOfSections { get; set; }
            public uint Timestamp { get; set; }
            public uint SymbolTableOffset { get; set; }
            public uint NumberOfSymbols { get; set; }
            public ushort SizeOfOptionalHeader { get; set; }
            public ushort Characteristics { get; set; }
        }

        public sealed class PESectionHeader
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
            public char[] Name { get; set; }
            public uint VirtualDataSize { get; set; }
            public uint VirtualDataOffset { get; set; }
            public uint RawDataSize { get; set; }
            public uint RawDataPointer { get; set; }
            public uint RelocationsPointer { get; set; }
            public uint LineNumbersPointer { get; set; }
            public ushort NumberOfRelocations { get; set; }
            public ushort NumberOfLineNumbers { get; set; }
            public uint Characteristics { get; set; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
            public byte[] Data { get; set; }
        }

        public sealed class PEFile
        {
            private PEFile()
            {
                Header = new PEHeader();
                SectionHeaders = new Collection<PESectionHeader>();
            }

            public PEHeader Header { get; private set; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
            public byte[] OptionalHeader { get; private set; }
            public Collection<PESectionHeader> SectionHeaders { get; private set; }

            private PEResourceNode _Resources;
            public PEResourceNode Resources
            {
                get
                {
                    if (_Resources == null)
                        _Resources = ParseResources();
                    return _Resources;
                }
            }

            private PEResourceNode ParseResources()
            {
                foreach (PESectionHeader SectionHeader in SectionHeaders)
                {
                    if (new string(SectionHeader.Name).StartsWith(".rsrc"))
                        return PEResourceNode.ReadResources(SectionHeader);
                }

                throw new InvalidOperationException("File is missing a .rsrc section.");
            }

            public static PEFile Parse(byte[] portableExecutableFileContent)
            {
                PEFile File = new PEFile();

                using (MemoryStream Stream = new MemoryStream(portableExecutableFileContent))
                {
                    BinaryReader Reader = new BinaryReader(Stream);

                    Stream.Position = 0x3C;
                    uint PEHeaderOffset = Reader.ReadUInt32();
                    Stream.Position = PEHeaderOffset;

                    PEHeader Header = File.Header;
                    Header.Signature = Reader.ReadUInt32();
                    if (Header.Signature != 17744) //PE\0\0
                        throw new InvalidOperationException("Invalid PE Signature.");
                    Header.Machine = Reader.ReadUInt16();
                    Header.NumberOfSections = Reader.ReadUInt16();
                    Header.Timestamp = Reader.ReadUInt32();
                    Header.SymbolTableOffset = Reader.ReadUInt32();
                    Header.NumberOfSymbols = Reader.ReadUInt32();
                    Header.SizeOfOptionalHeader = Reader.ReadUInt16();
                    Header.Characteristics = Reader.ReadUInt16();

                    File.OptionalHeader = Reader.ReadBytes(Header.SizeOfOptionalHeader);

                    for (int i = 0; i < Header.NumberOfSections; i++)
                    {
                        PESectionHeader SectionHeader = new PESectionHeader();
                        SectionHeader.Name = Reader.ReadChars(8);
                        SectionHeader.VirtualDataSize = Reader.ReadUInt32();
                        SectionHeader.VirtualDataOffset = Reader.ReadUInt32();
                        SectionHeader.RawDataSize = Reader.ReadUInt32();
                        SectionHeader.RawDataPointer = Reader.ReadUInt32();
                        SectionHeader.RelocationsPointer = Reader.ReadUInt32();
                        SectionHeader.LineNumbersPointer = Reader.ReadUInt32();
                        SectionHeader.NumberOfRelocations = Reader.ReadUInt16();
                        SectionHeader.NumberOfLineNumbers = Reader.ReadUInt16();
                        SectionHeader.Characteristics = Reader.ReadUInt32();

                        SectionHeader.Data = ReadDataFromOffset(SectionHeader.RawDataPointer, SectionHeader.RawDataSize, portableExecutableFileContent);

                        File.SectionHeaders.Add(SectionHeader);
                    }
                }

                return File;
            }

            internal static byte[] ReadDataFromOffset(uint dataOffset, uint sizeOfData, byte[] resourceData)
            {
                using (MemoryStream Stream = new MemoryStream(resourceData))
                {
                    Stream.Position = dataOffset;

                    BinaryReader Reader = new BinaryReader(Stream);

                    byte[] ResourceData = new byte[sizeOfData];

                    for (int i = 0; sizeOfData > 0; i++, sizeOfData--)
                    {
                        ResourceData[i] = Reader.ReadByte();
                    }

                    return ResourceData;
                }
            }
        }
}