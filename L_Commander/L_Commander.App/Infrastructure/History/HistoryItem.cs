﻿using System;

namespace L_Commander.App.Infrastructure.History;

public class HistoryItem
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime DateTime { get; set; }

    public DateTime Date => DateTime.Date;
}