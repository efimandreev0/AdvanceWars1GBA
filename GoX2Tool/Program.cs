using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace GoX2Tool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args[1].Contains(".txt"))
            {
                Rebuild(args[0], args[1]);
            }
            else
            {
                Extract(args[0]);
            }
        }
        public static void Extract(string game)
        {
            var reader = new BinaryReader(File.OpenRead(game));
            reader.BaseStream.Position = 0x412C58; //end = 0x41C6D8
            int[] offsets = new int[2471];
            string[] str = new string[2471];
            for (int i = 0; i < 2471; i++)
            {
                offsets[i] = reader.ReadInt32() - 0x8000000;
                reader.BaseStream.Position += 12;
            }
            for (int i = 0; i < 2471; i++)
            {
                reader.BaseStream.Position = offsets[i];
                str[i] = Utils.ReadString(reader, Encoding.UTF8).Replace("\r", "<lf>").Replace("\u000f", "<next>").Replace("\u0015", "<player>").Replace("\u0016", "<choice>").Replace("\u001f", "<or>").Replace("\v", "<color>").Replace("\u0019", "<unk1>").Replace("\u001c", "<unk2>").Replace("\u0018", "<unk3>").Replace("<unk4>", "\u001b").Replace("\u0017","</spec>").Replace("\u000e", "<unk5>").Replace("\f","<unk6>").Replace("\n","<br>").Replace("\t", "<tab>");
            }
            File.WriteAllLines(game + ".txt", str);
        }
        public static void Rebuild(string game, string file)
        {
            var writer = new BinaryWriter(File.OpenWrite(game));
            string[] str = File.ReadAllLines(file);
            int[] newOffsets = new int[str.Length];
            writer.BaseStream.Position = 0x5C2362;
            for (int i = 0; i < str.Length; i++)
            {
                newOffsets[i] = (int)writer.BaseStream.Position + 0x8000000;
                writer.Write(Encoding.UTF8.GetBytes(str[i].Replace("<lf>", "\r").Replace("<next>", "\u000f").Replace("<player>", "\u0015").Replace("<choice>", "\u0016").Replace("<or>", "\u001f").Replace("<color>", "\v").Replace("<unk1>", "\u0019").Replace("<unk2>", "\u001c").Replace("<unk3>", "\u0018").Replace("<unk4>", "\u001b").Replace("</spec>", "\u0017").Replace("<unk5>", "\u000e").Replace("<unk6>", "\f").Replace("<br>", "\n").Replace("<tab>", "\t")));
                writer.Write(new byte());
            }
            writer.BaseStream.Position = 0x412C58;
            for (int i = 0; i < str.Length; i++)
            {
                writer.Write(newOffsets[i]);
                writer.BaseStream.Position += 12;
            }
        }
    }
}
