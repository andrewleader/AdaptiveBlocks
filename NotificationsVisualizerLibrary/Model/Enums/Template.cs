﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model.Enums
{
    internal enum Template
    {
        TileSmall,
        TileMedium,
        TileWide,
        TileLarge,
        ToastGeneric,

#if EXPERIMENTAL
        CortanaGeneric,
        NewsFeedGeneric,
#endif

        Unsupported
    }
}
