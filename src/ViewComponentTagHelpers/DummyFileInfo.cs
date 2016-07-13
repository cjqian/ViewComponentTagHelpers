// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.FileProviders;
using System;
using System.IO;

namespace ViewComponentTagHelpers
{
    /// <summary>
    /// Used to create an empty IFileInfo type; used when compiling something in memory, not in a physical file.
    /// Nothing is actually implemented, LOL.
    /// </summary>
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
