using System.ComponentModel;

namespace Themis.Geometry.Index.KdTree
{
    public enum AddDuplicateBehavior
    {
        [Description("Skip the newly added HyperPoint & leave existing as-is")]
        Skip = 0,
        [Description("Throw a new DuplicateNodeError exception")]
        Error = 1,
        [Description("Replace the existing HyperPoint with the new one")]
        Update = 2
    }

    public class DuplicateNodeError : Exception
    {
        public DuplicateNodeError()
            : base("Cannot add node whose coordinates are already stored within the KdTree!")
        {
        }
    }
}
