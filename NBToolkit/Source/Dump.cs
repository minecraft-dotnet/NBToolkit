using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NDesk.Options;
using Substrate;
using Substrate.Nbt;
using Substrate.Core;

namespace NBToolkit
{
    public class DumpOptions : TKOptions, IChunkFilterable
    {
        private OptionSet _filterOpt = null;
        private ChunkFilter _chunkFilter = null;

        public string _outFile = "";

        public bool _dumpBlocks = false;
        public bool _dumpEntities = false;
        public bool _dumpTileEntities = false;

        public DumpOptions ()
            : base()
        {
            _filterOpt = new OptionSet()
            {
                { "out|OutFile=", "Path of file to write JSON data into",
                    var => _outFile = var },
                { "b|BlockData", "Dump block data (type, data, light arrays)",
                    var => _dumpBlocks = true },
                { "e|Entities", "Dump complete entity data",
                    var => _dumpEntities = true },
                { "t|TileEntities", "Dump complete tile entity data",
                    var => _dumpTileEntities = true },
            };

            _chunkFilter = new ChunkFilter();
        }

        public DumpOptions (string[] args)
            : this()
        {
            Parse(args);
        }

        public override void Parse (string[] args)
        {
            base.Parse(args);

            _filterOpt.Parse(args);
            _chunkFilter.Parse(args);
        }

        public override void PrintUsage ()
        {
            Console.WriteLine("Usage: nbtoolkit dump [options]");
            Console.WriteLine();
            Console.WriteLine("Options for command 'dump':");

            _filterOpt.WriteOptionDescriptions(Console.Out);

            Console.WriteLine();
            _chunkFilter.PrintUsage();

            Console.WriteLine();
            base.PrintUsage();
        }

        public override void SetDefaults ()
        {
            base.SetDefaults();

            if (_outFile.Length == 0) {
                Console.WriteLine("Error: You must specify an output file");
                Console.WriteLine();
                this.PrintUsage();

                throw new TKOptionException();
            }
        }

        public IChunkFilter GetChunkFilter ()
        {
            return _chunkFilter;
        }
    }

    class Dump : TKFilter
    {
        private DumpOptions opt;

        public Dump (DumpOptions o)
        {
            opt = o;
        }

        public override void Run ()
        {
            NbtWorld world = NbtWorld.Open(opt.OPT_WORLD);
            IChunkManager cm = world.GetChunkManager(opt.OPT_DIM);
            FilteredChunkManager fcm = new FilteredChunkManager(cm, opt.GetChunkFilter());

            StreamWriter fstr;
            try {
                fstr = new StreamWriter(opt._outFile, false);
            }
            catch (IOException e) {
                Console.WriteLine(e.Message);
                return;
            }

            fstr.WriteLine("[");

            bool first = true;
            foreach (ChunkRef chunk in fcm) {
                if (!first) {
                    fstr.Write(",");
                }

                IChunk c = chunk.GetChunkRef();

                NbtTree tree = null;
                if (c is AnvilChunk)
                    tree = (c as AnvilChunk).Tree;
                else if (c is AlphaChunk)
                    tree = (c as AlphaChunk).Tree;

                if (tree == null || tree.Root == null)
                    continue;

                if (!opt._dumpBlocks) {
                    tree.Root["Level"].ToTagCompound().Remove("Blocks");
                    tree.Root["Level"].ToTagCompound().Remove("Data");
                    tree.Root["Level"].ToTagCompound().Remove("BlockLight");
                    tree.Root["Level"].ToTagCompound().Remove("SkyLight");
                    tree.Root["Level"].ToTagCompound().Remove("HeightMap");
                }

                if (!opt._dumpEntities) {
                    tree.Root["Level"].ToTagCompound().Remove("Entities");
                }

                if (!opt._dumpTileEntities) {
                    tree.Root["Level"].ToTagCompound().Remove("TileEntities");
                }

                string s = JSONSerializer.Serialize(tree.Root["Level"], 1);
                fstr.Write(s);

                first = false;
            }

            fstr.WriteLine();
            fstr.WriteLine("]");

            fstr.Close();
        }
    }
}
