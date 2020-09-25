﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AppUtils.Lib.Arch.FileSystem
{

    /// <summary>
    ///  Interfaccia di definizione permessi di una entry file system
    ///  </summary>
    ///  <remarks></remarks>
    public interface IFSPermission
    {
        bool CanRead { get; }
        bool CanWrite { get; }
        bool CanDelete { get; }
        bool CanList { get; }
    }

}
