using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Common
{
    public static class Constant
    {
        public static Guid SystemId = Guid.NewGuid();
        public const string ConnectionString = "MainContext";
        public struct StatusId
        {
            public const string Active = "Active";
            public const string InActive = "Inactive";
        }

        public class BaseProperty
        {
            public const string Id = "Id";
            public const string CreatedDate = "CreatedDate";
            public const string CreatedBy = "CreatedBy";
            public const string ModifiedDate = "ModifiedDate";
            public const string ModifiedBy = "ModifiedBy";
            public const string StatusId = "StatusId";
        }

        public class AuditTrailProperty : BaseProperty
        {
            public const string ItemId = "ItemId";
            public const string TableName = "TableName";
            public const string TrackChange = "TrackChange";
            public const string TransactionId = "TransactionId";
        }

        public struct KendoGrid
        {
            public const int MaximumItem = 100;
        }
    }
}
