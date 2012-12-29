using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using NDesk.Options;
using Substrate;
using Substrate.Core;
using System.IO;

namespace NBToolkit
{
    public class ReplaceOptions : TKOptions, IChunkFilterable, IBlockFilterable
    {
        private OptionSet _filterOpt = null;
        private ChunkFilter _chunkFilter = null;
        private BlockFilter _blockFilter = null;        

        public int? OPT_BEFORE = null;
        public int? OPT_AFTER = null;

        public int? OPT_DATA = null;
        public double? OPT_PROB = null;

        // Block coordinate conditions
        public int? BL_X_GE = null;
        public int? BL_X_LE = null;
        public int? BL_Y_GE = null;
        public int? BL_Y_LE = null;
        public int? BL_Z_GE = null;
        public int? BL_Z_LE = null;

        // Neighbor conditions
        public int? OPT_NEIGHBOR = null;
        public int? OPT_NEIGHBOR_SIDE = null;
        public int? OPT_NEIGHBOR_E = null;
        public int? OPT_NEIGHBOR_W = null;
        public int? OPT_NEIGHBOR_N = null;
        public int? OPT_NEIGHBOR_S = null;
        public int? OPT_NEIGHBOR_T = null;
        public int? OPT_NEIGHBOR_B = null;

        public ReplaceOptions () 
            : base()
        {
            _filterOpt = new OptionSet()
            {
                /*{ "b|before=", "Replace instances of block type {ID} with another block type.  This option is repeatable.",
                    v => _includedBlocks.Add(Convert.ToInt32(v) % 256) },*/
                { "a|after=", "Replace the selected blocks with block type {ID}",
                    v => OPT_AFTER = Convert.ToInt32(v) },
                { "d|data=", "Set the new block's data value to {VAL} (0-15)",
                    v => OPT_DATA = Convert.ToInt32(v) },
                /*{ "p|prob=", "Replace any matching block with probability {VAL} (0.0-1.0)",
                    v => { OPT_PROB = Convert.ToDouble(v, new CultureInfo("en-US")); 
                           OPT_PROB = Math.Max((double)OPT_PROB, 0.0); 
                           OPT_PROB = Math.Min((double)OPT_PROB, 1.0); } },
                { "bxr|BlockXRange=", "Update blocks with X-coord between {0:V1} and {1:V2}, inclusive.  V1 or V2 may be left blank.",
                    (v1, v2) => { 
                        try { BL_X_GE = Convert.ToInt32(v1); } catch (FormatException) { }
                        try { BL_X_LE = Convert.ToInt32(v2); } catch (FormatException) { } 
                    } },
                { "byr|BlockYRange=", "Update blocks with Y-coord between {0:V1} and {1:V2}, inclusive.  V1 or V2 may be left blank",
                    (v1, v2) => { 
                        try { BL_Y_GE = Convert.ToInt32(v1); } catch (FormatException) { }
                        try { BL_Y_LE = Convert.ToInt32(v2); } catch (FormatException) { } 
                    } },
                { "bzr|BlockZRange=", "Update blocks with Z-coord between {0:V1} and {1:V2}, inclusive.  V1 or V2 may be left blank",
                    (v1, v2) => { 
                        try { BL_Z_GE = Convert.ToInt32(v1); } catch (FormatException) { }
                        try { BL_Z_LE = Convert.ToInt32(v2); } catch (FormatException) { } 
                    } },*/
                /*{ "nb=", "Update blocks that have block type {ID} as any neighbor",
                    v => OPT_NEIGHBOR = Convert.ToInt32(v) % 256 },
                { "nbs=", "Update blocks that have block type {ID} as any x/z neighbor",
                    v => OPT_NEIGHBOR_SIDE = Convert.ToInt32(v) % 256 },
                { "nbxa=", "Update blocks that have block type {ID} as their south neighbor",
                    v => OPT_NEIGHBOR_S = Convert.ToInt32(v) % 256 },
                { "nbxb=", "Update blocks that have block type {ID} as their north neighbor",
                    v => OPT_NEIGHBOR_N = Convert.ToInt32(v) % 256 },*/
                /*{ "nbya|BlockAboveEq=", "Update blocks that have block type {ID} as their top neighbor.  This option is repeatable.",
                    v => BlocksAboveEq.Add(Convert.ToInt32(v) % 256) },
                { "nbyb|BlockBelowEq=", "Update blocks that have block type {ID} as their bottom neighbor.  This option is repeatable.",
                    v => BlocksBelowEq.Add(Convert.ToInt32(v) % 256) },*/
                /*{ "nbza=", "Update blocks that have block type {ID} as their west neighbor",
                    v => OPT_NEIGHBOR_W = Convert.ToInt32(v) % 256 },
                { "nbzb=", "Update blocks that have block type {ID} as their east neighbor",
                    v => OPT_NEIGHBOR_E = Convert.ToInt32(v) % 256 },*/
            };

            _chunkFilter = new ChunkFilter();
            _blockFilter = new BlockFilter();
        }

        public ReplaceOptions (string[] args)
            : this()
        {
            Parse(args);
        }

        public override void Parse (string[] args)
        {
            base.Parse(args);

            _filterOpt.Parse(args);
            _chunkFilter.Parse(args);
            _blockFilter.Parse(args);
        }

        public override void PrintUsage ()
        {
            Console.WriteLine("Usage: nbtoolkit replace -a <id> [options]");
            Console.WriteLine();
            Console.WriteLine("Options for command 'replace':");

            _filterOpt.WriteOptionDescriptions(Console.Out);

            Console.WriteLine();
            _chunkFilter.PrintUsage();

            Console.WriteLine();
            _blockFilter.PrintUsage();

            Console.WriteLine();
            base.PrintUsage();
        }

        public override void SetDefaults ()
        {
            base.SetDefaults();

            // Check for required parameters
            if (OPT_AFTER == null) {
                Console.WriteLine("Error: You must specify a replacement Block ID");
                Console.WriteLine();
                PrintUsage();

                throw new TKOptionException();
            }

            if (_blockFilter.XAboveEq != null) {
                int cx = (int)_blockFilter.XAboveEq >> 4;
                _chunkFilter.XAboveEq = Math.Max(_chunkFilter.XAboveEq ?? cx, cx);
            }
            if (_blockFilter.XBelowEq != null) {
                int cx = (int)(_blockFilter.XBelowEq + 15) >> 4;
                _chunkFilter.XBelowEq = Math.Min(_chunkFilter.XBelowEq ?? cx, cx);
            }
            if (_blockFilter.ZAboveEq != null) {
                int cx = (int)_blockFilter.ZAboveEq >> 4;
                _chunkFilter.ZAboveEq = Math.Max(_chunkFilter.ZAboveEq ?? cx, cx);
            }
            if (_blockFilter.ZBelowEq != null) {
                int cx = (int)(_blockFilter.ZBelowEq + 15) >> 4;
                _chunkFilter.ZBelowEq = Math.Min(_chunkFilter.ZBelowEq ?? cx, cx);
            }
        }

        public IChunkFilter GetChunkFilter ()
        {
            return _chunkFilter;
        }

        public IBlockFilter GetBlockFilter ()
        {
            return _blockFilter;
        }
    }

    public class Replace : TKFilter
    {
        private ReplaceOptions opt;

        private static Random rand = new Random();

        //private List<BlockKey>[] _sort = new List<BlockKey>[4096];
        private Dictionary<int, List<BlockKey>> _sort = new Dictionary<int, List<BlockKey>>();

        public Replace (ReplaceOptions o)
        {
            opt = o;

            /*for (int i = 0; i < 4096; i++) {
                _sort[i] = new List<BlockKey>();
            }*/
        }

        public override void Run ()
        {
            if (!Directory.Exists(opt.OPT_WORLD) && !File.Exists(opt.OPT_WORLD)) {
                Console.WriteLine("Error: Could not locate path: " + opt.OPT_WORLD);
                return;
            }

            NbtWorld world = null;
            IChunkManager cm = null;
            FilteredChunkManager fcm = null;

            try {
                world = NbtWorld.Open(opt.OPT_WORLD);
                cm = world.GetChunkManager(opt.OPT_DIM);
                fcm = new FilteredChunkManager(cm, opt.GetChunkFilter());
            }
            catch (Exception e) {
                Console.WriteLine("Error: Failed to open world: " + opt.OPT_WORLD);
                Console.WriteLine("Exception: " + e.Message);
                Console.WriteLine(e.StackTrace);
                return;
            }

            int affectedChunks = 0;
            foreach (ChunkRef chunk in fcm) {
                if (opt.OPT_V) {
                    Console.WriteLine("Processing chunk {0},{1}...", chunk.X, chunk.Z);
                }

                ApplyChunk(world, chunk);

                affectedChunks += fcm.Save() > 0 ? 1 : 0;
            }

            Console.WriteLine("Affected Chunks: " + affectedChunks);
        }

        public void ApplyChunk (NbtWorld world, ChunkRef chunk)
        {
            IBlockFilter opt_b = opt.GetBlockFilter();

            //chunk.Blocks.AutoLight = false;
            //chunk.Blocks.AutoTileTick = false;

            int xBase = chunk.X * chunk.Blocks.XDim;
            int zBase = chunk.Z * chunk.Blocks.ZDim;

            // Determine X range
            int xmin = 0;
            int xmax = 15;

            if (opt_b.XAboveEq != null) {
                xmin = (int)opt_b.XAboveEq - xBase;
            }
            if (opt_b.XBelowEq != null) {
                xmax = (int)opt_b.XBelowEq - xBase;
            }

            xmin = (xmin < 0) ? 0 : xmin;
            xmax = (xmax > 15) ? 15 : xmax;

            if (xmin > 15 || xmax < 0 || xmin > xmax) {
                return;
            }

            // Determine Y range
            int ymin = 0;
            int ymax = 127;

            if (opt_b.YAboveEq != null) {
                ymin = (int)opt_b.YAboveEq;
            }
            if (opt_b.YBelowEq != null) {
                ymax = (int)opt_b.YBelowEq;
            }

            if (ymin > ymax) {
                return;
            }

            // Determine X range
            int zmin = 0;
            int zmax = 15;

            if (opt_b.ZAboveEq != null) {
                zmin = (int)opt_b.ZAboveEq - zBase;
            }
            if (opt_b.ZBelowEq != null) {
                zmax = (int)opt_b.ZBelowEq - zBase;
            }

            zmin = (zmin < 0) ? 0 : zmin;
            zmax = (zmax > 15) ? 15 : zmax;

            if (zmin > 15 || zmax < 0 || zmin > zmax) {
                return;
            }

            int xdim = chunk.Blocks.XDim;
            int ydim = chunk.Blocks.YDim;
            int zdim = chunk.Blocks.ZDim;

            // Bin blocks
            for (int y = ymin; y <= ymax; y++) {
                for (int x = xmin; x <= xmax; x++) {
                    for (int z = zmin; z <= zmax; z++) {
                        int id = chunk.Blocks.GetID(x, y, z);
                        if (!_sort.ContainsKey(id))
                            _sort[id] = new List<BlockKey>();

                        _sort[id].Add(new BlockKey(x, y, z));
                    }
                }
            }

            // Process bins
            //for (int i = 0; i < maxBin; i++) {
            foreach (var kv in _sort) {
                if (kv.Value.Count == 0) {
                    continue;
                }

                if (opt_b.IncludedBlockCount > 0 & !opt_b.IncludedBlocksContains(kv.Key)) {
                    continue;
                }

                if (opt_b.ExcludedBlockCount > 0 & opt_b.ExcludedBlocksContains(kv.Key)) {
                    continue;
                }

                foreach (BlockKey key in kv.Value) {
                    // Probability test
                    if (opt_b.ProbMatch != null) {
                        double c = rand.NextDouble();
                        if (c > opt_b.ProbMatch)
                            continue;
                    }

                    if (opt_b.BlocksAboveCount > 0 && key.y < ydim - 1) {
                        int neighborId = chunk.Blocks.GetID(key.x, key.y + 1, key.z);
                        if (!opt_b.BlocksAboveContains(neighborId))
                            continue;
                    }

                    if (opt_b.BlocksBelowCount > 0 && key.y > 0) {
                        int neighborId = chunk.Blocks.GetID(key.x, key.y - 1, key.z);
                        if (!opt_b.BlocksBelowContains(neighborId))
                            continue;
                    }

                    if (opt_b.BlocksSideCount > 0) {
                        bool validNeighbor = false;
                        AlphaBlockRef block1 = GetNeighborBlock(chunk, key.x - 1, key.y, key.z);
                        if (block1.IsValid && opt_b.BlocksSideContains(block1.ID) && !validNeighbor)
                            validNeighbor = true;
                        AlphaBlockRef block2 = GetNeighborBlock(chunk, key.x + 1, key.y, key.z);
                        if (block2.IsValid && opt_b.BlocksSideContains(block2.ID) && !validNeighbor)
                            validNeighbor = true;
                        AlphaBlockRef block3 = GetNeighborBlock(chunk, key.x, key.y, key.z - 1);
                        if (block3.IsValid && opt_b.BlocksSideContains(block3.ID) && !validNeighbor)
                            validNeighbor = true;
                        AlphaBlockRef block4 = GetNeighborBlock(chunk, key.x, key.y, key.z + 1);
                        if (block4.IsValid && opt_b.BlocksSideContains(block4.ID) && !validNeighbor)
                            validNeighbor = true;
                        if (!validNeighbor)
                            continue;
                    }

                    if (opt_b.BlocksNAboveCount > 0 && key.y < ydim - 1) {
                        int neighborId = chunk.Blocks.GetID(key.x, key.y + 1, key.z);
                        if (opt_b.BlocksNAboveContains(neighborId))
                            continue;
                    }

                    if (opt_b.BlocksNBelowCount > 0 && key.y > 0) {
                        int neighborId = chunk.Blocks.GetID(key.x, key.y - 1, key.z);
                        if (opt_b.BlocksNBelowContains(neighborId))
                            continue;
                    }

                    if (opt_b.BlocksNSideCount > 0) {
                        AlphaBlockRef block1 = GetNeighborBlock(chunk, key.x - 1, key.y, key.z);
                        if (block1.IsValid && opt_b.BlocksNSideContains(block1.ID))
                            continue;
                        AlphaBlockRef block2 = GetNeighborBlock(chunk, key.x + 1, key.y, key.z);
                        if (block2.IsValid && opt_b.BlocksNSideContains(block2.ID))
                            continue;
                        AlphaBlockRef block3 = GetNeighborBlock(chunk, key.x, key.y, key.z - 1);
                        if (block3.IsValid && opt_b.BlocksNSideContains(block3.ID))
                            continue;
                        AlphaBlockRef block4 = GetNeighborBlock(chunk, key.x, key.y, key.z + 1);
                        if (block4.IsValid && opt_b.BlocksNSideContains(block4.ID))
                            continue;
                    }

                    if (opt_b.IncludedDataCount > 0 || opt_b.ExcludedDataCount > 0) {
                        int data = chunk.Blocks.GetData(key.x, key.y, key.z);
                        if (opt_b.IncludedDataCount > 0 && !opt_b.IncludedDataContains(data)) {
                            continue;
                        }

                        if (opt_b.ExcludedDataCount > 0 && opt_b.ExcludedDataContains(data)) {
                            continue;
                        }
                    }

                    chunk.Blocks.SetID(key.x, key.y, key.z, (int)opt.OPT_AFTER);

                    if (opt.OPT_VV) {
                        int gx = chunk.X * xdim + key.x;
                        int gz = chunk.Z * zdim + key.z;
                        Console.WriteLine("Replaced block {0} at {1},{2},{3}", kv.Key, gx, key.y, gz);
                    }

                    if (opt.OPT_DATA != null) {
                        chunk.Blocks.SetData(key.x, key.y, key.z, (int)opt.OPT_DATA);
                    }
                }
            }

            // Reset bins
            _sort.Clear();

            // Process Chunk
            /*for (int y = ymin; y <= ymax; y++) {
                for (int x = xmin; x <= xmax; x++) {
                    for (int z = zmin; z <= zmax; z++) {
                        // Probability test
                        if (opt_b.ProbMatch != null) {
                            double c = rand.NextDouble();
                            if (c > opt_b.ProbMatch) {
                                continue;
                            }
                        }

                        int lx = x % xdim;
                        int ly = y % ydim;
                        int lz = z % zdim;

                        // Get the old block
                        int oldBlock = chunk.Blocks.GetID(lx , ly, lz);

                        // Skip block if it doesn't match the inclusion list
                        if (opt_b.IncludedBlockCount > 0) {
                            bool match = false;
                            foreach (int ib in opt_b.IncludedBlocks) {
                                if (oldBlock == ib) {
                                    match = true;
                                    break;
                                }
                            }

                            if (!match) {
                                continue;
                            }
                        }

                        // Skip block if it does match the exclusion list
                        if (opt_b.ExcludedBlockCount > 0) {
                            bool match = false;
                            foreach (int xb in opt_b.ExcludedBlocks) {
                                if (oldBlock == xb) {
                                    match = true;
                                    break;
                                }
                            }

                            if (match) {
                                continue;
                            }
                        }

                        // Replace the block
                        chunk.Blocks.SetID(lx, ly, lz, (int)opt.OPT_AFTER);

                        if (opt.OPT_VV) {
                            int gx = chunk.X * xdim + lx;
                            int gz = chunk.Z * zdim + lz;
                            Console.WriteLine("Replaced block at {0},{1},{2}", gx, ly, gz);
                        }

                        if (opt.OPT_DATA != null) {
                            chunk.Blocks.SetData(lx, ly, lz, (int)opt.OPT_DATA);
                        }
                    }
                }
            }*/
        }

        private AlphaBlockRef GetNeighborBlock (ChunkRef chunk, int x, int y, int z)
        {
            if (chunk == null)
                return new AlphaBlockRef();

            ChunkRef target = chunk;
            if (x < 0) {
                target = chunk.GetNorthNeighbor();
                x += chunk.Blocks.XDim;
            }
            else if (x >= chunk.Blocks.XDim) {
                target = chunk.GetSouthNeighbor();
                x -= chunk.Blocks.XDim;
            }
            else if (z < 0) {
                target = chunk.GetEastNeighbor();
                z += chunk.Blocks.ZDim;
            }
            else if (z >= chunk.Blocks.ZDim) {
                target = chunk.GetWestNeighbor();
                z -= chunk.Blocks.ZDim;
            }
            else
                return target.Blocks.GetBlockRef(x, y, z);

            return GetNeighborBlock(target, x, y, z);
        }
    }
}
