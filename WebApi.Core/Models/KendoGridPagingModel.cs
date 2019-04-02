using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Common;

namespace WebApi.Core.Models
{
    public class KendoGridPagingModel
    {
        public int Skip { get; set; }
        private int _take;
        public int Take
        {
            get
            {
                if (_take == 0 || _take > Constant.KendoGrid.MaximumItem)
                    return Constant.KendoGrid.MaximumItem;
                return _take;
            }
            set
            {
                _take = value;
            }
        }

        public List<KendoSortModel> Sort { get; set; }

        public string SortField
        {
            get
            {
                if (Sort != null && Sort.Count > 0)
                    return Sort[0].Field;
                return string.Empty;
            }
        }

        public string SortType
        {
            get
            {
                if (Sort != null && Sort.Count > 0)
                    return Sort[0].Dir;
                return string.Empty;
            }
        }

        public string FilterOperator { get; set; }
        public string FilterField { get; set; }
        public string FilterValue { get; set; }

        private int _page;
        public int Page
        {
            get
            {
                if (_page == 0)
                    return 1;
                return _page;
            }
            set
            {
                _page = value;
            }
        }

        private int _pageSize;
        public int PageSize
        {
            get
            {
                if (_pageSize == 0 || _pageSize > Constant.KendoGrid.MaximumItem)
                    return Constant.KendoGrid.MaximumItem;
                return _pageSize;
            }
            set
            {
                _pageSize = value;
            }
        }
    }

    public class KendoSortModel
    {
        public string Dir { get; set; }
        public string Field { get; set; }
    }
}
