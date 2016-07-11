using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace ViewComponentTagHelpers
{
    public class DummyFileInfo : IFileInfo
    {
        public bool Exists
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsDirectory
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTimeOffset LastModified
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public long Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PhysicalPath
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Stream CreateReadStream()
        {
            throw new NotImplementedException();
        }
    }
}
