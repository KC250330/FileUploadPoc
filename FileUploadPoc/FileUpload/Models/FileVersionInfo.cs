using System;
using System.IO;

namespace FileUploadPoc.FileUpload.Models
{
    public sealed class FileVersionInfo
    {
        private FileVersionInfo() { }

        public Version FileVersion { get; private set; }

        public static FileVersionInfo GetVersionInfo(PEFile file)
        {
            if (file == null) throw new ArgumentNullException("file");

            PEResourceNode VersionNode = null;
            foreach (PEResourceNode Node in file.Resources.Children)
            {
                if (Node.Name == "16")
                {
                    VersionNode = Node;
                    break;
                }
            }

            if (VersionNode == null || (VersionNode != null && VersionNode.Children.Count <= 0))
                throw new InvalidOperationException("Could not find VERSION resource.");

            PEResource VersionResource = VersionNode.Children[0].Resources[0];

            FileVersionInfo Info = new FileVersionInfo();
            Info.FileVersion = ParseVersion(VersionResource);

            return Info;

        }

        private static Version ParseVersion(PEResource versionResource)
        {
            using (MemoryStream Stream = new MemoryStream(versionResource.Data))
            {
                BinaryReader Reader = new BinaryReader(Stream);

                /*ushort StructLength =*/
                Reader.ReadUInt16();
                ushort ValueLength = Reader.ReadUInt16();
                /*ushort DataType =*/
                Reader.ReadUInt16();

                /*byte[] KeyBytes =*/
                Reader.ReadBytes("VS_VERSION_INFO".Length * 2 + 2);
                //string KeyString = System.Text.Encoding.Unicode.GetString(KeyBytes, 0, KeyBytes.Length);

                ushort Padding1 = Reader.ReadUInt16();
                Reader.ReadBytes(Padding1 * 2);

                if (ValueLength > 0)
                {
                    byte[] ValueData = Reader.ReadBytes(ValueLength);
                    return ParseFixedFileInfo(ValueData);
                }

                //ushort Padding2 = Reader.ReadUInt16();
                //Reader.ReadBytes(Padding2 * 2);

                throw new InvalidOperationException("FIXEDFILEINFO does not exist in struct.");
            }
        }

        private static Version ParseFixedFileInfo(byte[] fixedFileInfoData)
        {
            using (MemoryStream Stream = new MemoryStream(fixedFileInfoData))
            {
                BinaryReader Reader = new BinaryReader(Stream);

                uint Signature = Reader.ReadUInt32();
                if (Signature != 0xFEEF04BD)
                    throw new InvalidOperationException("FIXEDFILEINFO has an invalid signature.");
                /*uint StructureVersion =*/
                Reader.ReadUInt32();
                Version FileVersion = Read64BitVersion(Reader);
                /*Version ProductVersion =*/
                Read64BitVersion(Reader);
                /*uint FileFlagsMask =*/
                Reader.ReadUInt32();
                /*uint FileFlags =*/
                Reader.ReadUInt32();
                /*uint FileOS =*/
                Reader.ReadUInt32();
                /*uint FileType =*/
                Reader.ReadUInt32();
                /*uint FileSubType =*/
                Reader.ReadUInt32();
                /*uint FileDateMS =*/
                Reader.ReadUInt32();
                /*uint FileDateLS =*/
                Reader.ReadUInt32();

                return FileVersion;
            }
        }

        private static Version Read64BitVersion(BinaryReader reader)
        {
            ushort Minor = reader.ReadUInt16();
            ushort Major = reader.ReadUInt16();
            ushort Revision = reader.ReadUInt16();
            ushort Build = reader.ReadUInt16();
            return new Version(Major, Minor, Build, Revision);
        }
    }
}