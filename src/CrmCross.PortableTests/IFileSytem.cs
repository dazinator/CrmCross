﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmCross.Tests
{
    public interface IFileSystem
    {
        string ReadAllText(string path);
        bool Exists(string filePath);
    }
}
