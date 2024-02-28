﻿using System.Collections.Generic;

public class EtoDefinitions
{
    public class SetConfigurationEto
    {
        public string? ConnectionId { get; set; }
        public int Heartbeat { get; set; }
    }

    public class SnitchingDataEto
    {
        public string? ConnectionId { get; set; }
        public string? MachineIdentifier { get; set; }
        public List<string>? Logs { get; set; }

        public List<string>? Processes { get; set; }
    }

    public class ClientDisconnectEto
    {
        public string? ConnectionId { get; set; }
        public string? Message { get; set; }
    }

    public class KillCommandEto
    {
        public string? ConnectionId { get; set; }
        public List<string>? Processes { get; set; }
    }

    public class ShowMessageEto
    {
        public string? ConnectionId { get; set; }
        public int Duration { get; set; }
        public string? Message { get; set; }
    }
}