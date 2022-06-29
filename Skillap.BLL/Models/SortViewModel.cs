using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillap.BLL.Models
{
    public enum SortType
    {
        NameAsc,
        NameDesc,
        DateAsc,
        DateDesc,
        StatusAsc,
        StatusDesc
    }

    public class SortViewModel
    {
        public SortType NameSort { get; private set; }
        public SortType DateSort { get; private set; }
        public SortType StatusSort { get; private set; }
        public SortType Current { get; private set; }
        public SortViewModel(SortType sortOrder)
        {
            NameSort = sortOrder == SortType.NameAsc ? SortType.NameDesc : SortType.NameAsc;
            DateSort = sortOrder == SortType.DateAsc ? SortType.DateDesc : SortType.DateAsc;
            StatusSort = sortOrder == SortType.StatusAsc ? SortType.StatusDesc : SortType.StatusAsc;
            Current = sortOrder;
        }
    }
}
