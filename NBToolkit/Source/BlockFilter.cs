using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;

namespace NBToolkit
{
    public interface IBlockFilterable
    {
        IBlockFilter GetBlockFilter ();
    }

    public interface IBlockFilter
    {
        int? XAboveEq { get; }
        int? XBelowEq { get; }
        int? YAboveEq { get; }
        int? YBelowEq { get; }
        int? ZAboveEq { get; }
        int? ZBelowEq { get; }

        bool InvertXYZ { get; }

        IEnumerable<int> IncludedBlocks { get; } // MatchAny
        IEnumerable<int> ExcludedBlocks { get; } // MatchAny

        int IncludedBlockCount { get; }
        int ExcludedBlockCount { get; }

        double? ProbMatch { get; }

        bool IncludedBlocksContains (int id);
        bool ExcludedBlocksContains (int id);

        IEnumerable<int> BlocksAboveEq { get; } // MatchAny
        IEnumerable<int> BlocksBelowEq { get; } // MatchAny
        IEnumerable<int> BlocksSideEq { get; } // MatchAny

        IEnumerable<int> BlocksAboveNeq { get; } // MatchAny
        IEnumerable<int> BlocksBelowNeq { get; } // MatchAny
        IEnumerable<int> BlocksSideNeq { get; } // MatchAny

        int BlocksAboveCount { get; }
        int BlocksBelowCount { get; }
        int BlocksSideCount { get; }

        int BlocksNAboveCount { get; }
        int BlocksNBelowCount { get; }
        int BlocksNSideCount { get; }

        bool BlocksAboveContains (int id);
        bool BlocksBelowContains (int id);
        bool BlocksSideContains (int id);

        bool BlocksNAboveContains (int id);
        bool BlocksNBelowContains (int id);
        bool BlocksNSideContains (int id);

        IEnumerable<int> IncludedData { get; } // MatchAny
        IEnumerable<int> ExcludedData { get; } // MatchAny

        int IncludedDataCount { get; }
        int ExcludedDataCount { get; }

        bool IncludedDataContains (int id);
        bool ExcludedDataContains (int id);
    }

    public class BlockFilter : IOptions, IBlockFilter
    {
        protected int? _xAboveEq = null;
        protected int? _xBelowEq = null;
        protected int? _yAboveEq = null;
        protected int? _yBelowEq = null;
        protected int? _zAboveEq = null;
        protected int? _zBelowEq = null;

        protected bool _invertXYZ = false;

        protected List<int> _includedBlocks = new List<int>();
        protected List<int> _excludedBlocks = new List<int>();

        protected double? _prob = null;

        protected List<int> _blocksAboveEq = new List<int>();
        protected List<int> _blocksBelowEq = new List<int>();
        protected List<int> _blocksSideEq = new List<int>();

        protected List<int> _blocksAboveNeq = new List<int>();
        protected List<int> _blocksBelowNeq = new List<int>();
        protected List<int> _blocksSideNeq = new List<int>();

        protected List<int> _includedData = new List<int>();
        protected List<int> _excludedData = new List<int>();

        protected OptionSet _options;

        public int? XAboveEq
        {
            get { return _xAboveEq; }
            set { _xAboveEq = value; }
        }

        public int? XBelowEq
        {
            get { return _xBelowEq; }
            set { _xBelowEq = value; }
        }

        public int? YAboveEq
        {
            get { return _yAboveEq; }
        }

        public int? YBelowEq
        {
            get { return _yBelowEq; }
        }

        public int? ZAboveEq
        {
            get { return _zAboveEq; }
        }

        public int? ZBelowEq
        {
            get { return _zBelowEq; }
        }

        public bool InvertXYZ
        {
            get { return _invertXYZ; }
        }

        public IEnumerable<int> IncludedBlocks
        {
            get { return _includedBlocks; }
        }

        public IEnumerable<int> ExcludedBlocks
        {
            get { return _excludedBlocks; }
        }

        public int IncludedBlockCount
        {
            get { return _includedBlocks.Count; }
        }

        public int ExcludedBlockCount
        {
            get { return _excludedBlocks.Count; }
        }

        public double? ProbMatch
        {
            get { return _prob; }
        }

        public IEnumerable<int> IncludedData
        {
            get { return _includedData; }
        }

        public IEnumerable<int> ExcludedData
        {
            get { return _excludedData; }
        }

        public int IncludedDataCount
        {
            get { return _includedData.Count; }
        }

        public int ExcludedDataCount
        {
            get { return _excludedData.Count; }
        }

        public BlockFilter ()
        {
            _options = new OptionSet() {
                { "bxr|BlockXRange=", "Include blocks with X-coord between {0:V1} and {1:V2}, inclusive.  V1 or V2 may be left blank.",
                    (v1, v2) => { 
                        try { _xAboveEq = Convert.ToInt32(v1); } catch (FormatException) { }
                        try { _xBelowEq = Convert.ToInt32(v2); } catch (FormatException) { } 
                    } },
                { "byr|BlockYRange=", "Include blocks with Y-coord between {0:V1} and {1:V2}, inclusive.  V1 or V2 may be left blank.",
                    (v1, v2) => { 
                        try { _yAboveEq = Convert.ToInt32(v1); } catch (FormatException) { }
                        try { _yBelowEq = Convert.ToInt32(v2); } catch (FormatException) { } 
                    } },
                { "bzr|BlockZRange=", "Include blocks with Z-coord between {0:V1} and {1:V2}, inclusive.  V1 or V2 may be left blank.",
                    (v1, v2) => { 
                        try { _zAboveEq = Convert.ToInt32(v1); } catch (FormatException) { }
                        try { _zBelowEq = Convert.ToInt32(v2); } catch (FormatException) { } 
                    } },
                { "brv|BlockInvertXYZ", "Inverts the block selection created by --bxr, --byr and --bzr when all three options are used.",
                    v => _invertXYZ = true },
                { "bi|BlockInclude=", "Match blocks of type {ID}.  This option is repeatable.",
                    v => _includedBlocks.Add(Convert.ToInt32(v)) },
                { "bir|BlockIncludeRange=", "Match blocks of type between {0:V1} and {1:V2}, inclusive.  This option is repeatable.",
                    (v1, v2) => {
                        int i1 = Math.Max(0, Convert.ToInt32(v1));
                        int i2 = Math.Max(0, Convert.ToInt32(v2));
                        for (int i = i1; i <= i2; i++) {
                            _includedBlocks.Add(i);
                        }
                    } },
                { "bx|BlockExclude=", "Match all blocks except blocks of type {ID}.  This option is repeatable.",
                    v => _excludedBlocks.Add(Convert.ToInt32(v)) },
                { "ber|BlockExcludeRange=", "Match all blocks except blocks of type between {0:V1} and {1:V2}, inclusive.  This option is repeatable.",
                    (v1, v2) => {
                        int i1 = Math.Max(0, Convert.ToInt32(v1));
                        int i2 = Math.Max(0, Convert.ToInt32(v2));
                        for (int i = i1; i <= i2; i++) {
                            _excludedBlocks.Add(i);
                        }
                    } },
                { "nb|BlockAroundEq=", "Update blocks that have block type {ID} as any neighbor.  This option is repeatable.",
                    v => {
                        _blocksAboveEq.Add(Convert.ToInt32(v));
                        _blocksBelowEq.Add(Convert.ToInt32(v));
                        _blocksSideEq.Add(Convert.ToInt32(v));
                    } },
                { "nbya|BlockAboveEq=", "Update blocks that have block type {ID} as their top neighbor.  This option is repeatable.",
                    v => _blocksAboveEq.Add(Convert.ToInt32(v)) },
                { "nbyb|BlockBelowEq=", "Update blocks that have block type {ID} as their bottom neighbor.  This option is repeatable.",
                    v => _blocksBelowEq.Add(Convert.ToInt32(v)) },
                { "nbs|BlockSideEq=", "Update blocks that have block type {ID} as one or more side neighbors.  This option is repeatable.",
                    v => _blocksSideEq.Add(Convert.ToInt32(v)) },
                { "nbx|BlockAroundNeq=", "Update blocks that don't have block type {ID} as any neighbor.  This option is repeatable.",
                    v => {
                        _blocksAboveNeq.Add(Convert.ToInt32(v));
                        _blocksBelowNeq.Add(Convert.ToInt32(v));
                        _blocksSideNeq.Add(Convert.ToInt32(v));
                    } },
                { "nbyax|BlockAboveNeq=", "Update blocks that don't have block type {ID} as their top neighbor.  This option is repeatable.",
                    v => _blocksAboveNeq.Add(Convert.ToInt32(v)) },
                { "nbybx|BlockBelowNeq=", "Update blocks that don't have block type {ID} as their bottom neighbor.  This option is repeatable.",
                    v => _blocksBelowNeq.Add(Convert.ToInt32(v)) },
                { "nbsx|BlockSideNeq=", "Update blocks that don't have block type {ID} as one or more side neighbors.  This option is repeatable.",
                    v => _blocksSideNeq.Add(Convert.ToInt32(v)) },
                { "bp|BlockProbability=", "Selects a matching block with probability {VAL} (0.0-1.0)",
                    v => _prob = Convert.ToDouble(v) },
                { "di|DataInclude=", "Match qualifying blocks with data value {VAL}.  This opion is repeatable.",
                    var => _includedData.Add(Convert.ToInt32(var)) },
                { "dir|DataIncludeRange=", "Match qualifying blocks with data value between {0:V1} and {1:V2}, inclusive.  This option is repeatable.",
                    (v1, v2) => {
                        int i1 = Math.Max(0, Convert.ToInt32(v1));
                        int i2 = Math.Max(0, Convert.ToInt32(v2));
                        for (int i = i1; i <= i2; i++) {
                            _includedData.Add(i);
                        }
                    } },
                { "dx|DataExclude=", "Match all qualifying blocks except blocks with data value {VAL}.  This opion is repeatable.",
                    var => _excludedData.Add(Convert.ToInt32(var)) },
                { "der|DataExcludeRange=", "Match all qualifying blocks except blocks with data value between {0:V1} and {1:V2}, inclusive.  This option is repeatable.",
                    (v1, v2) => {
                        int i1 = Math.Max(0, Convert.ToInt32(v1));
                        int i2 = Math.Max(0, Convert.ToInt32(v2));
                        for (int i = i1; i <= i2; i++) {
                            _excludedData.Add(i);
                        }
                    } },
            };
        }

        public BlockFilter (string[] args)
            : this()
        {
            Parse(args);
        }

        public void Parse (string[] args)
        {
            _options.Parse(args);
        }

        public void PrintUsage ()
        {
            Console.WriteLine("Block Filtering Options:");
            _options.WriteOptionDescriptions(Console.Out);
        }

        public bool IncludedBlocksContains (int id)
        {
            return _includedBlocks.Contains(id);
        }

        public bool ExcludedBlocksContains (int id)
        {
            return _excludedBlocks.Contains(id);
        }

        public bool IncludedDataContains (int val)
        {
            return _includedData.Contains(val);
        }

        public bool ExcludedDataContains (int val)
        {
            return _excludedData.Contains(val);
        }

        public IEnumerable<int> BlocksAboveEq
        {
            get { return _blocksAboveEq; }
        }

        public IEnumerable<int> BlocksBelowEq
        {
            get { return _blocksBelowEq; }
        }

        public IEnumerable<int> BlocksSideEq
        {
            get { return _blocksSideEq; }
        }

        public IEnumerable<int> BlocksAboveNeq
        {
            get { return _blocksAboveNeq; }
        }

        public IEnumerable<int> BlocksBelowNeq
        {
            get { return _blocksBelowNeq; }
        }

        public IEnumerable<int> BlocksSideNeq
        {
            get { return _blocksSideNeq; }
        }

        public int BlocksAboveCount
        {
            get { return _blocksAboveEq.Count; }
        }

        public int BlocksBelowCount
        {
            get { return _blocksBelowEq.Count; }
        }

        public int BlocksSideCount
        {
            get { return _blocksSideEq.Count; }
        }

        public int BlocksNAboveCount
        {
            get { return _blocksAboveNeq.Count; }
        }

        public int BlocksNBelowCount
        {
            get { return _blocksBelowNeq.Count; }
        }

        public int BlocksNSideCount
        {
            get { return _blocksSideNeq.Count; }
        }

        public bool BlocksAboveContains (int id)
        {
            return _blocksAboveEq.Contains(id);
        }

        public bool BlocksBelowContains (int id)
        {
            return _blocksBelowEq.Contains(id);
        }

        public bool BlocksSideContains (int id)
        {
            return _blocksSideEq.Contains(id);
        }

        public bool BlocksNAboveContains (int id)
        {
            return _blocksAboveNeq.Contains(id);
        }

        public bool BlocksNBelowContains (int id)
        {
            return _blocksBelowNeq.Contains(id);
        }

        public bool BlocksNSideContains (int id)
        {
            return _blocksSideNeq.Contains(id);
        }
    }
}
