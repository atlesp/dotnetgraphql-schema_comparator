using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace schema_comparator
{
    public class Result
    {
        public readonly List<Change> changes;


        public Result(List<Change> changes)
        {
            this.changes = changes;

        }


        public Boolean IsIdentical
        {
            get
            {
                return this.changes.Count == 0;
            }
        }
        public Boolean IsBreaking
        {
            get
            {
                return GetBreakingChanges().Count > 0;
            }
        }


        public Boolean IsDangerous
        {
            get
            {
                return GetDangerousChanges().Count > 0;
            }
        }


        public List<Change> GetBreakingChanges()
        {
            return this.changes.Where(c => c.IsBreaking).ToList();
        }


        public List<Change> GetDangerousChanges()
        {

            return this.changes.Where(c => c.IsDangerous).ToList();

        }


        public List<Change> GetNonBreakingChanges()
        {
            return this.changes.Where(c => c.IsNonBreaking).ToList();
        }


    }
}
