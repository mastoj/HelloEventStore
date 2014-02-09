using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloEventStore.Domain.Services
{
    public static class IdGenerator
    {
        private static Func<Guid> _guidGenerator;

        public static Func<Guid> GuidGenerator
        {
            get
            {
                _guidGenerator = _guidGenerator ?? (Guid.NewGuid);
                return _guidGenerator;
            }
            set
            {
                _guidGenerator = value;
            }
        }

        public static Guid GetId()
        {
            return GuidGenerator();
        }
    }
}
